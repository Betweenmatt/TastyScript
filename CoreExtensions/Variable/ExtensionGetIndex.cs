using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreExtensions.Variable
{
    [Extension("GetIndex",new string[] { "item" },varExtension:true)]
    [Serializable]
    public class ExtensionGetIndex : BaseExtension
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if (args == null || args.ElementAtOrDefault(0) == null)
                Manager.Throw($"{this.Name} arguments cannot be null.");

            var inputAsTobj = new TArray("arr", input.Value, input.Line);
            if (inputAsTobj == null)
                Manager.Throw($"Cannot find Token [{input.Name}]");

            var ele = inputAsTobj.Arguments.FirstOrDefault(f => f == args[0]);
            if (ele == null)
                Manager.Throw($"The element [{args[0]}] does not exist in this collection.");

            return new Token("index", Array.IndexOf(inputAsTobj.Arguments,args[0]).ToString(), input.Line);
        }
    }
}
