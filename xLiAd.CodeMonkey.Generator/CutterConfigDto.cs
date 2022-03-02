using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xLiAd.CodeMonkey.Generator
{
    internal class CutterConfigDto
    {
        public CutterConfigDto(ClassDeclarationSyntax classDeclaration, AopAttributeDto aopAttribute)
        {
            ClassDeclaration = classDeclaration;
            AopAttribute = aopAttribute;
            List<string> names = new List<string>();
            names.Add(ClassDeclaration.Identifier.ValueText);
            if (ClassDeclaration.Identifier.ValueText.EndsWith("Attribute"))
                names.Add(ClassDeclaration.Identifier.ValueText.Substring(0, ClassDeclaration.Identifier.ValueText.Length - 9));
            this.names = names;
        }

        public ClassDeclarationSyntax ClassDeclaration { get; }

        public AopAttributeDto AopAttribute { get; }

        private readonly IEnumerable<string> names;

        public string Name => ClassDeclaration.Identifier.ValueText;

        public bool CheckAttrIsMe(string name)
        {
            return this.names.Contains(name);
        }
    }
}
