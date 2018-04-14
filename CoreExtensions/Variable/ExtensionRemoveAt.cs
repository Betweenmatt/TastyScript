using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreExtensions.Variable
{
    [Extension("RemoveAt", varExtension: true)]
    public class ExtensionRemoveAt : BaseExtension
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if (args == null || args.ElementAtOrDefault(0) == null)
                Manager.Throw($"{this.Name} arguments cannot be null.");
            int index = 0;
            var nofail = int.TryParse(args[0].ToString(), out index);
            if (!nofail)
                Manager.Throw($"{this.Name} arguments must be a whole number.");

            if (input == null || input.Value == null)
                Manager.Throw($"Extension cannot extend null.");
            var inputAsTobj = new TArray("arr", input.Value);
            inputAsTobj.Remove(index);
            return inputAsTobj;
        }
    }
}
