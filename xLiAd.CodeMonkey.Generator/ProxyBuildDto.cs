using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.CodeMonkey.Generator
{
    internal class ProxyBuildDto
    {
        public ProxyBuildDto(ClassDeclarationSyntax @class, INamedTypeSymbol service, IEnumerable<CutterConfigDto> cutterConfigDtos)
        {
            Class = @class;
            Service = service;
            CutterConfigs = cutterConfigDtos;
        }

        public ClassDeclarationSyntax Class { get; }

        public INamedTypeSymbol Service { get; }

        public IEnumerable<CutterConfigDto> CutterConfigs { get; }
    }
}
