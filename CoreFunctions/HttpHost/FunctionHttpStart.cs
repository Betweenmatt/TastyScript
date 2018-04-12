using Nancy.Hosting.Self;
using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;
using TastyScript.ParserManager.IOStream;

namespace TastyScript.CoreFunctions.HttpHost
{
    [Function("HttpStart", new string[] { "uri" })]
    internal class FunctionHttpStart : FunctionDefinition
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
                Reader.ReadLine(Manager.CancellationTokenSource.Token);
            }
            return true;
        }
    }
    [Function("HttpSetIndexPage", new string[] { "path" })]
    internal class FunctionHttpSetIndexPage : FunctionDefinition
    {
        public override bool CallBase()
        {
            var args = ProvidedArgs.First("path");
            ServiceHandler.IndexPath = args.ToString().UnCleanString();
            return true;
        }
    }
}
