using Nancy.Hosting.Self;
using System;
using System.IO;
using System.Threading;
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
            //create Content directory if it doesnt exist
            var contentdir = AppDomain.CurrentDomain.BaseDirectory + "Content/";
            Directory.CreateDirectory(contentdir);
            var uri = ProvidedArgs.First("uri");
            if (uri == null)
            {
                Throw($"URI Cannot be null");
                return false;
            }
            using (var host = new NancyHost(new Uri(uri.ToString())))
            {
                if(HttpServiceHandler.IndexPath == "")
                {
                    Throw($"Index path cannot be empty. Must set with `HttpSetIndexPage` first.");
                    return true;
                }
                host.Start();
                Print($"Running on {uri}");
                while (!Manager.CancellationTokenSource.IsCancellationRequested)
                {
                    Thread.Sleep(500);
                }
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
            if(args == null)
            {
                Throw("Path cannot be null");
                return false;
            }
            HttpServiceHandler.IndexPath = args.ToString().UnCleanString();
            return true;
        }
    }
}
