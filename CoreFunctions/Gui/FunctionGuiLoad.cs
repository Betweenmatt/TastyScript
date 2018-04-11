using Nancy.Hosting.Self;
using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.Gui
{
    [Function("GuiLoad", new string[] { "uri" })]
    internal class FunctionGuiLoad : FunctionDefinition
    {
        public override bool CallBase()
        {
            //"http://localhost:4321"
            var uri = ProvidedArgs.First("uri");
            using (var host = new NancyHost(new Uri(uri.ToString())))
            {
                if(ServiceHandler.IndexPath == "")
                {
                    Console.WriteLine("index is not set");
                    return true;
                }
                host.Start();
                Console.WriteLine($"Running on {uri}");
                Console.ReadLine();
            }
            return true;
        }
    }
    [Function("GuiSetIndex", new string[] { "path" })]
    internal class FunctionGuiSetIndex : FunctionDefinition
    {
        public override bool CallBase()
        {
            var args = ProvidedArgs.First("path");
            ServiceHandler.IndexPath = args.ToString().UnCleanString();
            return true;
        }
    }
}
