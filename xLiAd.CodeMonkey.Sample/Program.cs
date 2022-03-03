using System;
using System.Linq;

namespace xLiAd.CodeMonkey.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var type = typeof(MyService).Assembly.GetTypes().Where(x => x.Name == "MyServiceProxy").FirstOrDefault();
            IMyService myService;
            if (type != null)
                myService = System.Activator.CreateInstance(type) as IMyService;
            else
                myService = new MyService();
            var s = myService.GetAString();
            Console.WriteLine(s);
        }
    }
}
