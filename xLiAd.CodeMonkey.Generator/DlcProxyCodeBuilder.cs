using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace xLiAd.CodeMonkey.Generator
{
    internal class DlcProxyCodeBuilder
    {
        private readonly ClassDeclarationSyntax classDeclarationSyntax;
        private readonly INamedTypeSymbol service;
        private readonly IList<CutterConfigDto> cutterConfigs;
        public DlcProxyCodeBuilder(ProxyBuildDto proxyBuildDto)
        {
            this.classDeclarationSyntax = proxyBuildDto.Class;
            this.service = proxyBuildDto.Service;
            this.cutterConfigs = proxyBuildDto.CutterConfigs.ToArray();
        }

        public IEnumerable<string> Usings
        {
            get
            {
                yield return "using System;";
            }
        }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace => this.service.ContainingNamespace.ToString();

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName => this.service.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + "Proxy";

        public SourceText ToSourceText()
        {
            var code = this.ToString();
            return SourceText.From(code, Encoding.UTF8);
        }
        /// <summary>
        /// 开始想让代理类继承原始类的父类，但发现 :base() 这个语法没办法实现。 如果实现就需要继承原始类。所以不继承原始类的父类，只继承所有接口。
        /// 但是这样还是有一个问题， 如果某个接口的实现是在他的父类里实现的怎么办？遍历接口实现所有方法和属性？太复杂了，先不这么做的，先约定不要继承
        /// 
        /// 新的sourcegenorator 代理类方案：继承原始类的父类、所有接口，抄过来构造方法，自己 new 一个原始类实例， 类的主体不变。
        /// 特性定义需要在引用分析器的项目中使用。类和接口需要在同一个项目中。
        /// </summary>
        private string BaseType => service.BaseType.ToDisplayString() == "object" ? string.Empty : (service.BaseType.ToDisplayString());

        private IEnumerable<string> GetParametersOfContructor(ConstructorDeclarationSyntax constructor, bool withType)
        {
            foreach(var para in constructor.ParameterList.Parameters)
            {
                if (!withType)
                {
                    yield return para.Identifier.ValueText;
                    continue;
                }
                if (para.Type is IdentifierNameSyntax paraIden)
                    yield return $"{paraIden.Identifier.ValueText} {para.Identifier.ValueText}";
                else if (para.Type is PredefinedTypeSyntax paraPred)
                    yield return para.ToString();
                else
                    yield return para.ToString();
            }
        }
        private static bool TryGetParentSyntax<T>(SyntaxNode syntaxNode, out T result) where T : SyntaxNode
        {
            result = null;
            if (syntaxNode == null)
                return false;
            try
            {
                syntaxNode = syntaxNode.Parent;
                if (syntaxNode == null)
                    return false;
                if (syntaxNode.GetType() == typeof(T))
                {
                    result = syntaxNode as T;
                    return true;
                }
                return TryGetParentSyntax<T>(syntaxNode, out result);
            }
            catch
            {
                return false;
            }
        }

        public override string ToString()
        {
            //Debugger.Launch();
            var cutters = ExtendMethods.GetClassAllCutter(classDeclarationSyntax, cutterConfigs);
            var builder = new StringBuilder();
            foreach (var item in this.Usings)
            {
                builder.AppendLine(item);
            }
            var cutterUsings = cutters.SelectMany(x => x.AopAttribute.Usings).Select(x => x.Trim()).Distinct().ToArray();
            foreach (var item in cutterUsings)
            {
                if (!this.Usings.Contains(item))
                    builder.AppendLine(item);
            }
            CompilationUnitSyntax compilationUnitSyntax;
            if(TryGetParentSyntax(classDeclarationSyntax,out compilationUnitSyntax))
            {
                foreach(var usi in compilationUnitSyntax.Usings)
                {
                    if (!this.Usings.Contains(usi.ToString()) && !cutterUsings.Contains(usi.ToString()))
                        builder.AppendLine(usi.ToString());
                }
            }
            builder.AppendLine();

            builder.AppendLine($"namespace {this.Namespace}");
            builder.AppendLine("{");
            builder.AppendLine($"\t{GetAccessibility(service)} class {this.ClassName}:{string.Join(",", new string[] { BaseType }.Concat(this.service.Interfaces.Select(x => x.ToDisplayString())).Where(x => !string.IsNullOrEmpty(x)))}");
            builder.AppendLine("\t{");

            builder.AppendLine($"\t\tprivate readonly {this.service.Name} svc;");
            var cutterInjects = cutters.SelectMany(x => x.AopAttribute.Injects).Select(x => x.Trim()).Distinct().ToArray();
            foreach (var cutterInject in cutterInjects)
                builder.AppendLine($"\t\tprivate readonly {cutterInject};");
            //构造方法
            var constructors = classDeclarationSyntax.Members.Where(x => x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ConstructorDeclaration)).OfType<ConstructorDeclarationSyntax>();
            foreach (var constructor in constructors)
            {
                string b = null;
                if (constructor.Initializer != null)
                {
                    b = $" :base({string.Join(",", constructor.Initializer.ArgumentList.Arguments)})";
                }
                var parameterWithTypes = GetParametersOfContructor(constructor, true);
                builder.AppendLine($"\t\tpublic {this.ClassName}({string.Join(",", parameterWithTypes.Concat(cutterInjects).Distinct())}){b}");
                builder.AppendLine("\t\t{");
                builder.AppendLine($"\t\t\tthis.svc = new {service.Name}({string.Join(",", GetParametersOfContructor(constructor, false))});");
                foreach(var item in cutterInjects)
                {
                    var i = item.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    if (i.Length == 2)
                        builder.AppendLine($"\t\t\tthis.{i[1]} = {i[1]};");
                }
                builder.AppendLine("$(OtherFieldInit)$");
                builder.AppendLine("\t\t}");
            }
            if (!constructors.Any())
            {
                builder.AppendLine($"\t\tpublic {this.ClassName}()");
                builder.AppendLine("\t\t{");
                builder.AppendLine($"\t\t\tthis.svc = new {service.Name}();");
                builder.AppendLine("$(OtherFieldInit)$");
                builder.AppendLine("\t\t}");
            }

            var allProperties = service.GetMembers().OfType<IPropertySymbol>().Select(x => x.Name).ToArray();

            foreach (var member in service.GetMembers())
            {
                if (member.DeclaredAccessibility != Accessibility.Public)
                    continue;
                if (member is IMethodSymbol method)
                {
                    if ((method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) && allProperties.Contains(method.Name.Substring(4)))//属性
                        continue;
                    if (method.Name == ".ctor")
                        continue;
                    var methodCode = this.BuildMethod(method);
                    builder.AppendLine(methodCode);
                }
                else if(member is IPropertySymbol property)
                {
                    var propertyCode = this.BuildProperty(property);
                    builder.AppendLine(propertyCode);
                }
                else if(member is IFieldSymbol field)
                {
                    var fieldCode = this.BuildField(field);
                    builder.AppendLine(fieldCode);
                }
            }

            builder.AppendLine("\t}");
            builder.AppendLine("}");
            var result = builder.ToString().Replace("$(OtherFieldInit)$", FieldsInitStringBuilder.ToString());
            //Debugger.Launch();
            return result;
        }
        private string GetRefKindString(RefKind kind)
        {
            switch (kind)
            {
                case RefKind.Ref:
                    return "ref ";
                case RefKind.Out:
                    return "out ";
                case RefKind.RefReadOnly:
                    return "ref readonly ";
                default:
                    return null;
            }
        }
        /// <summary>
        /// 构建方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string BuildMethod(IMethodSymbol method)
        {
            //if (method.Name == "VoidMethod")
            //    Debugger.Launch();
            var builder = new StringBuilder();
            var parametersString = string.Join(",", method.Parameters.Select(item => $"{GetRefKindString(item.RefKind)}{item.Type} {item.Name}"));
            var parameterNamesString = string.Join(",", method.Parameters.Select(item => GetRefKindString(item.RefKind) + item.Name));
            bool isvoid = method.ReturnType.ToString() == "void" || (method.IsAsync && method.ReturnType.ToString() == "System.Threading.Tasks.Task");

            var cutters = ExtendMethods.GetMemberAllCutter(this.classDeclarationSyntax, this.cutterConfigs, method);

            var hongParamInNames = $"new object[] {{{string.Join(",", method.Parameters.Where(x => x.RefKind == RefKind.None).Select(x => x.Name))}}}";

            builder.AppendLine($"\t\t{GetAccessibility(method)}{(method.IsStatic ? " static": null)} {(method.IsAsync ? "async ": String.Empty)}{method.ReturnType} {method.Name}({parametersString})");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t\ttry{");

            foreach (var cutter in cutters)
                builder.AppendLine($"\t\t\t\t{ReplaceNames(cutter.AopAttribute.BeforeBody, method.Name, hongParamInNames, "null", "null", "null")}");

            builder.AppendLine($"\t\t\t\t{(isvoid ? string.Empty : "var result = ")}{(method.IsAsync ? "await " : String.Empty)}{GetServiceOrClassName(method)}.{method.Name}({parameterNamesString});");

            var hongResult = isvoid ? "new object[0]" : "new object[]{ result }";
            var hongTmpRO = new List<string>();
            if (!isvoid)
                hongTmpRO.Add("result");
            hongTmpRO.AddRange(method.Parameters.Where(x => x.RefKind == RefKind.Out || x.RefKind == RefKind.Ref).Select(x => x.Name));
            var hongResultAndOut = $"new object[] {{{string.Join(",", hongTmpRO)}}}";

            foreach(var cutter in cutters.Reverse())
                builder.AppendLine($"\t\t\t\t{ReplaceNames(cutter.AopAttribute.AfterBody, method.Name, hongParamInNames, hongResult, hongResultAndOut, "null")}");

            builder.AppendLine($"\t\t\t\treturn{(isvoid ? string.Empty : " result")};");
            builder.AppendLine("\t\t\t}");
            builder.AppendLine("\t\t\tcatch(Exception ex){");

            foreach (var cutter in cutters)
                builder.AppendLine($"\t\t\t\t{ReplaceNames(cutter.AopAttribute.ExceptionBody, method.Name, hongParamInNames, "null", "null", "ex")}");

            builder.AppendLine("\t\t\t\tthrow;");
            builder.AppendLine("\t\t\t}");
            builder.AppendLine("\t\t}");
            return builder.ToString();
        }

        private string ReplaceNames(string source, string name, string paramsIn, string result, string resultAndOuts, string exception)
        {
            var r = source?.Replace(HongHelper.ClassName, this.ClassName).Replace(HongHelper.ProxiedClassName, service.Name).Replace(HongHelper.Name, name)
                .Replace(HongHelper.ParamsIn, paramsIn).Replace(HongHelper.Result, result).Replace(HongHelper.ResultAndOuts, resultAndOuts).Replace(HongHelper.Exception, exception);
            return r;
        }

        private string GetAccessibility(ISymbol sth)
        {
            switch (sth.DeclaredAccessibility)
            {
                case Accessibility.Public:
                    return "public";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "internal";
                case Accessibility.Private:
                    return "private";
                case Accessibility.ProtectedAndInternal:
                    return "protected internal";
                default:
                    return "private";
            }
        }

        private string GetServiceOrClassName(ISymbol member)
        {
            if (member.IsStatic)
                return service.Name;
            else
                return "this.svc";
        }

        private string BuildProperty(IPropertySymbol property)
        {
            var builder = new StringBuilder();

            var cutters = ExtendMethods.GetMemberAllCutter(this.classDeclarationSyntax, this.cutterConfigs, property);

            builder.AppendLine($"\t\t{GetAccessibility(property)}{(property.IsStatic ? " static" : null)} {property.Type} {property.Name}");
            builder.AppendLine($"\t\t{{");
            if(property.GetMethod != null)
            {
                builder.AppendLine($"\t\t\tget {{");
                builder.AppendLine("\t\t\t\ttry{");

                foreach (var cutter in cutters)
                    builder.AppendLine($"\t\t\t\t{ReplaceNames(cutter.AopAttribute.BeforeBody, property.Name, "new object[0]", "null", "null", "null")}");

                builder.AppendLine($"\t\t\t\t\tvar result = {GetServiceOrClassName(property)}.{property.Name};");

                foreach (var cutter in cutters.Reverse())
                    builder.AppendLine($"\t\t\t\t{ReplaceNames(cutter.AopAttribute.AfterBody, property.Name, "new object[0]", "new object[] { result }", "new object[] { result }", "null")}");

                builder.AppendLine($"\t\t\t\t\treturn result;");
                builder.AppendLine($"\t\t\t\t}}");
                builder.AppendLine("\t\t\t\tcatch(Exception ex){");

                foreach (var cutter in cutters)
                    builder.AppendLine($"\t\t\t\t{ReplaceNames(cutter.AopAttribute.ExceptionBody, property.Name, "new object[0]", "new object[] { result }", "new object[] { result }", "ex")}");

                builder.AppendLine("\t\t\t\tthrow;");
                builder.AppendLine("\t\t\t\t}");
                builder.AppendLine($"\t\t\t}}");
            }
            if(property.SetMethod != null)
            {
                builder.AppendLine($"\t\t\tset {{");
                builder.AppendLine("\t\t\t\ttry{");

                foreach (var cutter in cutters)
                    builder.AppendLine($"\t\t\t{ReplaceNames(cutter.AopAttribute.BeforeBody, property.Name, "new object[] { value }", "null", "null", "null")}");

                builder.AppendLine($"\t\t\t\t{GetServiceOrClassName(property)}.{property.Name} = value;");

                foreach (var cutter in cutters.Reverse())
                    builder.AppendLine($"\t\t\t{ReplaceNames(cutter.AopAttribute.AfterBody, property.Name, "new object[] { value }", "new object[0]", "new object[0]", "null")}");

                builder.AppendLine($"\t\t\t\t}}");
                builder.AppendLine("\t\t\t\tcatch(Exception ex){");

                foreach (var cutter in cutters)
                    builder.AppendLine($"\t\t\t\t{ReplaceNames(cutter.AopAttribute.ExceptionBody, property.Name, "new object[] { value }", "new object[0]", "new object[0]", "ex")}");

                builder.AppendLine("\t\t\t\tthrow;");
                builder.AppendLine("\t\t\t\t}");
                builder.AppendLine($"\t\t\t}}");
            }
            builder.AppendLine("\t\t}");
            return builder.ToString();
        }
        /// <summary>
        /// 非静态 public 字段应该在构造方法里赋值 所以需要有个临时存放在的方
        /// </summary>
        private StringBuilder FieldsInitStringBuilder = new StringBuilder();

        private string BuildField(IFieldSymbol field)
        {
            //Debugger.Launch();
            var builder = new StringBuilder();
            if (field.IsStatic || field.IsConst)
            {
                builder.AppendLine($"\t\t{GetAccessibility(field)} {(field.IsStatic ? "static" : "const")} {field.Type} {field.Name} = {GetServiceOrClassName(field)}.{field.Name};");
            }
            else
            {
                builder.AppendLine($"\t\t{GetAccessibility(field)}{(field.IsReadOnly ? " readonly" : null)} {field.Type} {field.Name};");
                FieldsInitStringBuilder.AppendLine($"\t\t\tthis.{field.Name} = svc.{field.Name};");
            }
            return builder.ToString();
        }
    }
}
