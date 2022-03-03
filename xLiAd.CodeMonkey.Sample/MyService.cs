using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.CodeMonkey.Sample
{
    [Aop]
    public class MyService : IMyService
    {
        public MyService() { }
        public string GetAString()
        {
            return "a string";
        }
    }

    internal interface IMyService
    {
        string GetAString();
    }
}
