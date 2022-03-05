using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.CodeMonkey.Sample
{
    public class AopAttribute : Attribute
    {
        public string BeforeBody = "Console.WriteLine(\"要执行方法$ClassName$-->$Name$ 了，参数是：\"+ Newtonsoft.Json.JsonConvert.SerializeObject($ParamsIn$));";

        public string AfterBody => "Console.WriteLine(\"方法$ClassName$-->$Name$ 执行完了，返回值是：\"+ Newtonsoft.Json.JsonConvert.SerializeObject($ResultAndOuts$));";

        public string ExceptionBody => @"Console.WriteLine(""方法$ClassName$-->$Name$ 执行出错了，异常是：""+
            Newtonsoft.Json.JsonConvert.SerializeObject($Exception$));";

        //public string Usings = "using System.Text;";

        //public string Injects = "IKeyResultRepository keyResultRepository";
    }
}
