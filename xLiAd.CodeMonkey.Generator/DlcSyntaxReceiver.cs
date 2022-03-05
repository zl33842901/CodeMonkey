using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace xLiAd.CodeMonkey.Generator
{
    internal class DlcSyntaxReceiver : ISyntaxReceiver
    {
        private readonly List<ClassDeclarationSyntax> classSyntaxList = new List<ClassDeclarationSyntax>();

        /// <summary>
        /// 访问语法树 
        /// </summary>
        /// <param name="syntaxNode"></param>
        void ISyntaxReceiver.OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if(syntaxNode is ClassDeclarationSyntax syntaxc)
            {
                this.classSyntaxList.Add(syntaxc);
            }
        }

        private void SetValue(string name, string value, AopAttributeDto dto, bool withAt)
        {
            var v = withAt ? value.Replace("\"\"", "\"") : System.Text.RegularExpressions.Regex.Unescape(value);
            switch (name)
            {
                case "BeforeBody":
                    dto.BeforeBody = v;
                    break;
                case "AfterBody":
                    dto.AfterBody = v;
                    break;
                case "ExceptionBody":
                    dto.ExceptionBody = v;
                    break;
                case "Usings":
                    dto.Usings = value?.Split(new string[] { "\n" }, StringSplitOptions.None).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray() ?? new string[0];
                    break;
                case "Injects":
                    dto.Injects = value?.Split(new string[] { "," }, StringSplitOptions.None).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray() ?? new string[0];
                    break;
            }
        }

        public bool CheckIsAopAttribute(ClassDeclarationSyntax classSyntax, out AopAttributeDto aopAttributeDto)
        {
            var members = classSyntax.Members.Where(x => x.IsKind(SyntaxKind.PropertyDeclaration) || x.IsKind(SyntaxKind.FieldDeclaration));
            aopAttributeDto = new AopAttributeDto();
            foreach (var member in members)
            {
                if(member is PropertyDeclarationSyntax property)
                {
                    var type = property.Type.ToString();
                    if (type != "string")
                        continue;
                    var name = property.Identifier.ValueText;
                    var value = property.ExpressionBody.Expression.ToString();
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                        SetValue(name, value.Substring(1, value.Length - 2), aopAttributeDto, false);
                    else if(value.StartsWith("@\"") && value.EndsWith("\""))
                        SetValue(name, value.Substring(2, value.Length - 3), aopAttributeDto, true);
                }
                else if(member is FieldDeclarationSyntax field)
                {
                    var type = field.Declaration.Type.ToString();
                    if (type != "string")
                        continue;
                    var name = field.Declaration.Variables[0].Identifier.ValueText;
                    var value = field.Declaration.Variables[0].Initializer.Value.ToString();
                    if(value.StartsWith("\"") && value.EndsWith("\""))
                        SetValue(name, value.Substring(1, value.Length - 2), aopAttributeDto, false);
                    else if (value.StartsWith("@\"") && value.EndsWith("\""))
                        SetValue(name, value.Substring(2, value.Length - 3), aopAttributeDto, true);
                }
            }
            if (aopAttributeDto.AfterBody != null || aopAttributeDto.BeforeBody != null || aopAttributeDto.ExceptionBody != null)
            {
                if (classSyntax.AttributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == "ForAllClass" || x.Name.ToString() == "ForAllClassAttribute"))
                    aopAttributeDto.ForAllClass = true;
                return true;
            }
            else
                return false;
        }


        public IEnumerable<ProxyBuildDto> GetServiceTypes(Compilation compilation)
        {
            //Debugger.Launch();
            List<CutterConfigDto> configs = new List<CutterConfigDto>();
            List<ClassDeclarationSyntax> shouldChecks = new List<ClassDeclarationSyntax>();
            foreach (var classSyntax in this.classSyntaxList)
            {
                var cla = compilation.GetSemanticModel(classSyntax.SyntaxTree).GetDeclaredSymbol(classSyntax);
                if (cla.IsAbstract)
                    continue;
                if (cla.IsStatic)
                    continue;
                if(cla.BaseType != null && cla.BaseType.ToDisplayString().EndsWith("Attribute") && CheckIsAopAttribute(classSyntax, out var dto))
                {
                    configs.Add(new CutterConfigDto(classSyntax, dto));
                }
                else if(cla.DeclaredAccessibility == Accessibility.Public)
                {
                    shouldChecks.Add(classSyntax);
                }
            }
            foreach (var classSyntax in shouldChecks)
            {
                if(ExtendMethods.CheckClassIsNeedCutter(classSyntax, configs))
                {
                    var cla = compilation.GetSemanticModel(classSyntax.SyntaxTree).GetDeclaredSymbol(classSyntax);
                    yield return new ProxyBuildDto(classSyntax, cla, configs);
                }
            }
        }
    }
}
