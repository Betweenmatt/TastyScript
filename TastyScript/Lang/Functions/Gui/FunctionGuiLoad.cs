using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Functions.Gui
{
    [Function("GuiLoad", new string[] { "uri" })]
    internal class FunctionGuiLoad : FDefinition
    {
        public override string CallBase()
        {
            //"http://localhost:4321"
            var uri = ProvidedArgs.First("uri");
            using (var host = new NancyHost(new Uri(uri.ToString())))
            {
                host.Start();
                Console.WriteLine($"Running on {uri}");
                Console.ReadLine();
            }
            //end the script when host is killed
            TokenParser.Stop = true;
            return null;
        }
    }
}
