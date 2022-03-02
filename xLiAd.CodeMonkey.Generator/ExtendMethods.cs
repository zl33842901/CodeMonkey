using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xLiAd.CodeMonkey.Generator
{
    internal static class ExtendMethods
    {
        internal static bool CheckClassIsNeedCutter(ClassDeclarationSyntax classSyntax, IList<CutterConfigDto> configs)
        {
            return GetClassAllCutterDo(classSyntax, configs).Any();
        }

        private static IEnumerable<CutterConfigDto> GetClassCutterDo(ClassDeclarationSyntax classSyntax, IList<CutterConfigDto> configs)
        {
            foreach(var config in configs.Where(x => x.AopAttribute.ForAllClass))
                yield return config;
            var attrs = classSyntax.AttributeLists.SelectMany(x => x.Attributes);
            foreach (var attr in attrs)
            {
                var config = configs.Where(x => x.CheckAttrIsMe(attr.Name.ToString())).FirstOrDefault();
                if (config != null)
                    yield return config;
            }
        }

        private static IEnumerable<CutterConfigDto> GetClassAllCutterDo(ClassDeclarationSyntax classSyntax, IList<CutterConfigDto> configs, MemberDeclarationSyntax member = null)
        {
            foreach (var config in GetClassCutterDo(classSyntax, configs))
                yield return config;
            IEnumerable<MemberDeclarationSyntax> members;
            if (member != null)
                members = new MemberDeclarationSyntax[] { member };
            else
                members = classSyntax.Members.Where(x => x.IsKind(SyntaxKind.PropertyDeclaration) || x.IsKind(SyntaxKind.MethodDeclaration));
            foreach (var mem in members)
            {
                var memberAttrs = mem.AttributeLists.SelectMany(x => x.Attributes);
                foreach(var attr in memberAttrs)
                {
                    var config = configs.Where(x => x.CheckAttrIsMe(attr.Name.ToString())).FirstOrDefault();
                    if (config != null)
                        yield return config;
                }
            }
        }

        private static IEnumerable<CutterConfigDto> GetClassAllCutterDo(ClassDeclarationSyntax classSyntax, IList<CutterConfigDto> configs, ISymbol member)
        {
            foreach (var config in GetClassCutterDo(classSyntax, configs))
                yield return config;
            var memberAttrs = member.GetAttributes();
            foreach (var attr in memberAttrs)
            {
                var config = configs.Where(x => x.CheckAttrIsMe(attr.AttributeClass.Name)).FirstOrDefault();
                if (config != null)
                    yield return config;
            }
        }

        public static IEnumerable<CutterConfigDto> GetClassAllCutter(ClassDeclarationSyntax classSyntax, IList<CutterConfigDto> configs)
        {
            return GetClassAllCutterDo(classSyntax, configs).Distinct();
        }
        public static IEnumerable<CutterConfigDto> GetMemberAllCutter(ClassDeclarationSyntax classSyntax, IList<CutterConfigDto> configs, MemberDeclarationSyntax member)
        {
            return GetClassAllCutterDo(classSyntax, configs, member).Distinct();
        }
        public static IEnumerable<CutterConfigDto> GetMemberAllCutter(ClassDeclarationSyntax classSyntax, IList<CutterConfigDto> configs, ISymbol member)
        {
            return GetClassAllCutterDo(classSyntax,configs,member).Distinct();
        }
    }
}
