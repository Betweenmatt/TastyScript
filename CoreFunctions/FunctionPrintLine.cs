using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("PrintLine", new string[] { "s" })]
    public class FunctionPrintLine : FunctionDefinition
    {
        private List<string> concatStrings = new List<string>();
        private void Concat()
        {
            var findConcat = Extensions.First("Concat");
            if (findConcat != null)
            {
                var concatList = Extensions.Where("Concat");
                foreach (var x in concatList)
                {
                    var param = x;
                    string[] ext = param.Extend();
                    concatStrings.Add(ext[0].ToString());
                }
            }
        }
        public override bool CallBase()
        {
            Concat();
            var print = "";
            var argsList = ProvidedArgs.First("s");
            if (argsList != null)
                print = argsList.ToString();
            //color extension check
            var findColorExt = Extensions.First("Color");
            var color = ConsoleColor.Gray;
            if (findColorExt != null)
            {
                var param = findColorExt.Extend();
                ConsoleColor newcol = ConsoleColor.Gray;
                var nofail = Enum.TryParse<ConsoleColor>(param[0].ToString(), out newcol);
                if (nofail)
                    color = newcol;
            }
            //try to escape, and if escaping fails fallback on the original string
            string output = print + String.Join("", concatStrings);
            try
            {
                output = System.Text.RegularExpressions.Regex.Unescape(print + String.Join("", concatStrings));
            }
            catch (Exception e)
            {
                if (!(e is ArgumentException) || !(e is ArgumentNullException))
                    Manager.Throw($"Unexpected input: {output}");
                return false;
            }
            Manager.Print(output.UnCleanString(), color);

            //clear extensions after done
            concatStrings = new List<string>();
            Extensions = new IFunction.Containers.ExtensionList();
            return true;
        }
    }
}
