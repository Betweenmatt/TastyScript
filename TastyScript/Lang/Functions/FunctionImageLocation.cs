using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("ImageLocation", new string[] { "path" })]
    internal class FunctionImageLocation : FDefinition
    {
        public override string CallBase()
        {
            if (Main.AndroidDriver == null)
                Compiler.ExceptionListener.Throw("Cannot check screen without a connected device");
            var path = ProvidedArgs.First("path");
            if (path == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, $"Invoke function cannot be null.", LineValue));
            }
            var prop = CheckProperty();
            try
            {
                var ret = Commands.GetImageCoordinates(path.ToString(), prop);
                if (ret == null)
                {
                    ReturnBubble = new Tokens.Token("null", "null", "");
                    return "";
                }
                string[] ouput = new string[] { ret[0].ToString(), ret[1].ToString() };
                
                    ReturnBubble = new TArray("arr", ouput, "");
            }
            catch (Exception e)
            {
                if (e is System.IO.FileNotFoundException)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                        $"File could not be found: {path.ToString()}"));
                }
                Console.WriteLine(e);
                Compiler.ExceptionListener.Throw(new ExceptionHandler("[73]Unexpected error with CheckScreen()"));
            }
            return "";
        }
        private string[] CheckProperty()
        {
            var prop = Extensions.FirstOrDefault(f => f.Name == "Prop") as ExtensionProp;
            var save = Extensions.FirstOrDefault(f => f.Name == "Save") as ExtensionSave;
            if(prop != null)
            {
                var props = prop.Extend().ToList();
                if (save != null && save.Extend().ElementAtOrDefault(0) != null)
                    props.Add(save.Extend().ElementAtOrDefault(0));
                return props.ToArray();
            }
            else
            {
                return null;
            }
        }
    }
}
