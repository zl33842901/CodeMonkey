using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.CodeMonkey.Generator
{
    internal class AopAttributeDto
    {
        public string BeforeBody { get; set; }

        public string AfterBody { get; set; }

        public IEnumerable<string> Usings { get; set; } = new string[0];

        public IEnumerable<string> Injects { get; set; } = new string[0];

        public bool ForAllClass { get; set; }
    }
}
