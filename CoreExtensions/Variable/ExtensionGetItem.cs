using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreExtensions.Variable
{
    /// <summary>
    /// Gets the item from the extended collection at the given index
    /// </summary>
    [Extension("GetItem", new string[] { "index" }, varExtension:true)]
    [Serializable]
    public class ExtensionGetItem : BaseExtension
    {
        public override Token Extend(Token input)
        {
            
            var args = Extend();
            if (args == null || args.ElementAtOrDefault(0) == null)
                Manager.Throw($"{this.Name} arguments cannot be null.");

            int index = 0;
            if (args.ElementAtOrDefault(0) != null)
            {
                var nofail = int.TryParse(args[0].ToString(), out index);
                if (!nofail)
                    Manager.Throw($"{this.Name} arguments must be a whole number.");
            }
            
            var inputAsTobj = new TArray("arr", input.Value);
            if (inputAsTobj == null)
                Manager.Throw($"Cannot find Token [{input.Name}]");
                    
            var ele = inputAsTobj.Arguments.ElementAtOrDefault(index);
            if (ele == null)
                Manager.Throw($"The element at [{index}] is null.");
            
            return new Token("arr", ele);
        }
        
    }
}
