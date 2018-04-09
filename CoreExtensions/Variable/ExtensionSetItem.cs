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
    /// Replaces the extended collections object at the given index with the given object
    /// null index adds the given object to the end of the collection
    /// </summary>
    [Extension("SetItem",new string[] { "arr", "index" }, varExtension:true,alias:new string[] { "Add" })]
    [Serializable]
    public class ExtensionSetItem : BaseExtension
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if(args == null || args.ElementAtOrDefault(0) == null)
                Manager.Throw($"{this.Name} arguments cannot be null.");

            int index = -1;
            if (args.ElementAtOrDefault(1) != null)
            {
                var nofail = int.TryParse(args[1].ToString(), out index);
                if (!nofail)
                    Manager.Throw($"{this.Name} arguments must be a whole number.");
            }

            var inputAsTobj = new TArray("arr", input.Value, input.Line); ;
            if (inputAsTobj == null)
                Manager.Throw($"Cannot find Token [{input.Name}]");
            if (index == -1)
            {
                inputAsTobj.Add(args[0]);
                return inputAsTobj;
            }

            var ele = inputAsTobj.Arguments.ElementAtOrDefault(index);
            if (ele == null)
                Manager.Throw($"The element at [{index}] is null.");
            inputAsTobj.Arguments[index] = args[0];

            return inputAsTobj;

        }
    }
}
