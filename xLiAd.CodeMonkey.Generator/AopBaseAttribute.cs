using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.CodeMonkey.Generator
{
    public abstract class AopBaseAttribute : Attribute
    {
        string BeforeBody { get; }

        string AfterBody { get; }

        string ExceptionBody { get; }

        string Usings { get; }

        string Injects { get; }
    }
}
