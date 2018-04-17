using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;

namespace TastyScript.CoreFunctions.HttpHost
{
    [Function("StdOut", new string[] { "Text", "Line", "Color", "Id" })]
    public class FunctionStdOut : FunctionDefinition
    {
        public override bool CallBase()
        {
            var color = ProvidedArgs.First("Color");
            var line = ProvidedArgs.First("Line");
            string linetext = "";
            var text = ProvidedArgs.First("Text");
            if (line == null)
                linetext = "True";
            else
                linetext = line.ToString();
            var checkcol = Enum.TryParse<ConsoleColor>(color.ToString(), out ConsoleColor outcol);
            if (!checkcol)
                Print(text?.ToString() ?? "", (linetext == "true" || linetext == "True") ? true : false);
            else
                Print(text?.ToString() ?? "", outcol, (linetext == "true" || linetext == "True") ? true : false);

            return true;
        }
    }
}
