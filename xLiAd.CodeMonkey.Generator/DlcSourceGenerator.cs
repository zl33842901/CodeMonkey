using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;
using System.Linq;

namespace xLiAd.CodeMonkey.Generator
{
    [Generator]
    public class DlcSourceGenerator : ISourceGenerator
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new DlcSyntaxReceiver());
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        public void Execute(GeneratorExecutionContext context)
        {
            //Debugger.Launch();
            if (context.SyntaxReceiver is DlcSyntaxReceiver receiver)
            {
                var builders = receiver
                    .GetServiceTypes(context.Compilation)
                    .Select(i => new DlcProxyCodeBuilder(i))
                    .Distinct();

                foreach (var builder in builders)
                {
                    context.AddSource(builder.ClassName, builder.ToSourceText());
                }
            }
        }
    }
}
