using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("StdIn", new string[] { "Input", "Line", "Id" })]
    public class FunctionStdIn : FunctionDefinition
    {
        public override bool CallBase()
        {
            var input = ProvidedArgs.First("Input")?.ToString();
            var line = ProvidedArgs.First("Line")?.ToString();
            var id = ProvidedArgs.First("Id")?.ToString();
            StreamWriter streamWriter = Manager.GuiInvokeProcess.StandardInput;
            streamWriter.WriteLine(input.ToStreamXml(line == "False" || line == "false" ? false : true, id: id ?? ""));
            return true;
        }
    }
}
