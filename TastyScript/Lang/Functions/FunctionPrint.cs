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
    internal class FunctionPrint : FDefinition<string>
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
                    TParameter ext = param.Extend();
                    concatStrings.Add(ext.Value.Value[0].ToString());
                }
            }
        }
        public override string CallBase(TParameter args)
        {
            Concat();
            var print = "";
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "s");
            if (argsList != null)
                print = argsList.ToString();
            //color extension check
            var color = ConsoleColor.Gray;
            var findColorExt = Extensions.FirstOrDefault(f => f.Name == "Color") as ExtensionColor;
            if (findColorExt != null)
            {
                var param = findColorExt.Extend();
                ConsoleColor newcol = ConsoleColor.Gray;
                var nofail = Enum.TryParse<ConsoleColor>(param.Value.Value[0].ToString(), out newcol);
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
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Unexpected input: {output}", LineValue));
            }
            IO.Output.Print(output, color, false);


            //clear extensions after done
            concatStrings = new List<string>();
            Extensions = new List<IExtension>();
            return print;
        }
    }
}
