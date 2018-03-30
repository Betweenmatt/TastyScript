using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Print", new string[] { "s" })]
    internal class FunctionPrint : FDefinition
    {
        private List<string> concatStrings = new List<string>();
        private void Concat()
        {
            var findConcat = Extensions.FirstOrDefault(f => f.Name == "Concat") as ExtensionConcat;
            if (findConcat != null)
            {
                var concatList = Extensions.Where(f => f.Name == "Concat");
                foreach (var x in concatList)
                {
                    var param = x as ExtensionConcat;
                    string[] ext = param.Extend();
                    concatStrings.Add(ext[0]);
                }
            }
        }
        public override string CallBase()
        {
            Concat();
            var print = "";
            var argsList = ProvidedArgs.First("s");
            if (argsList != null)
                print = argsList.ToString();
            //color extension check
            var color = ConsoleColor.Gray;
            var findColorExt = Extensions.FirstOrDefault(f => f.Name == "Color") as ExtensionColor;
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
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Unexpected input: {output}", LineValue));
                    return null;
                }
            }
            Main.IO.Print(output.UnCleanString(), color, false);


            //clear extensions after done
            concatStrings = new List<string>();
            Extensions = new List<EDefinition>();
            return print;
        }
    }
}
