using System;
using SimpleIOCContainer.IServices;
using SimpleIOCContainer.Library;

namespace SimpleIOCContainer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //SimpleContainer.Container.Register<ITest1, Test1>();   //Can be registered as this also
            SimpleContainer.Container.RegisterAssembly("SimpleIOCContainer.Services");
            var instance = SimpleContainer.Container.Resolve<ITest>();
            Console.ReadLine();
        }
    }


   
}
