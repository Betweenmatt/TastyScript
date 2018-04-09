using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Tokens;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("ImageLocation", new string[] { "path" })]
    public class FunctionImageLocation : FunctionDefinition
    {
        public override bool CallBase()
        {
            if (!Manager.Driver.IsConnected())
            {
                Manager.Throw("Cannot check screen without a connected device");
                return false;
            }
            var path = ProvidedArgs.First("path");
            if (path == null)
            {
                Manager.Throw($"Invoke function cannot be null.");
                return false;
            }
            var prop = CheckProperty();
            try
            {
                var ret = Commands.GetImageCoordinates(path.ToString(), prop);
                if (ret == null)
                {
                    ReturnBubble = new Token("null", "null", "");
                    return false;
                }
                string[] ouput = new string[] { ret[0].ToString(), ret[1].ToString() };
                
                    ReturnBubble = new TArray("arr", ouput, "");
            }
            catch (Exception e)
            {
                if (e is System.IO.FileNotFoundException)
                {
                    Manager.Throw($"File could not be found: {path.ToString()}");
                    return false;
                }
                Console.WriteLine(e);
                Manager.Throw(("[73]Unexpected error with CheckScreen()"));
                return false;
            }
            return true;
        }
        private string[] CheckProperty()
        {
            var prop = Extensions.First("Prop");
            var save = Extensions.First("Save");
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
