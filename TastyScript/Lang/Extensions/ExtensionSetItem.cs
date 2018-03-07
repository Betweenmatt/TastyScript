using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    /// <summary>
    /// Replaces the extended collections object at the given index with the given object
    /// null index adds the given object to the end of the collection
    /// </summary>
    [Extension("SetItem",new string[] { "arr", "index" })]
    [Serializable]
    internal class ExtensionSetItem : EDefinition
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if(args == null || args.ElementAtOrDefault(0) == null)
                Compiler.ExceptionListener.Throw($"{this.Name} arguments cannot be null.",
                    ExceptionType.CompilerException, input.Line);

            int index = -1;
            if (args.ElementAtOrDefault(1) != null)
            {
                var nofail = int.TryParse(args[1].ToString(), out index);
                if (!nofail)
                    Compiler.ExceptionListener.Throw($"{this.Name} arguments must be a whole number.",
                        ExceptionType.CompilerException, input.Line);
            }

            var inputAsTobj = new TArray("arr", input.Value, input.Line); ;
            if (inputAsTobj == null)
                Compiler.ExceptionListener.Throw($"Cannot find Token [{input.Name}]",
                    ExceptionType.CompilerException, input.Line);
            if (index == -1)
            {
                inputAsTobj.Add(args[0]);
                return inputAsTobj;
            }

            var ele = inputAsTobj.Arguments.ElementAtOrDefault(index);
            if (ele == null)
                Compiler.ExceptionListener.Throw($"The element at [{index}] is null.",
                    ExceptionType.NullReferenceException, input.Line);
            inputAsTobj.Arguments[index] = args[0];

            return inputAsTobj;

        }
    }
}
