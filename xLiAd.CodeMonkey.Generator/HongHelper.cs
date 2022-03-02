using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.CodeMonkey.Generator
{
    internal static class HongHelper
    {
        /// <summary>
        /// 类名
        /// </summary>
        internal const string ClassName = "$ClassName$";
        /// <summary>
        /// 原始类名（被代理类）
        /// </summary>
        internal const string ProxiedClassName = "$ProxiedClassName$";
        /// <summary>
        /// 方法或属性名称
        /// </summary>
        internal const string Name = "$Name$";
        /// <summary>
        /// 入参（不包含out类型参数）object[]
        /// </summary>
        internal const string ParamsIn = "$ParamsIn$";
        /// <summary>
        /// 返回值  void类型方法为 null
        /// </summary>
        internal const string Result = "$Result$";
        /// <summary>
        /// 返回值 和 Out Ref 类型参数 object[]
        /// </summary>
        internal const string ResultAndOuts = "$ResultAndOuts$";
    }
}
