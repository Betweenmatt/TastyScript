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
    [Extension("SetItem",new string[] { "arr", "index" }, FunctionObsolete: true)]
    [Serializable]
    internal class ExtensionSetItem : EDefinition
    {
        public override string[] Extend(Token input)
        {
            var args = Extend();
            if(args == null || args.ElementAtOrDefault(0) == null)
                Compiler.ExceptionListener.Throw($"{this.Name} arguments cannot be null.",
                    ExceptionType.CompilerException, "{0}");

            int index = -1;
            if (args.ElementAtOrDefault(1) != null)
            {
                var nofail = int.TryParse(args[0].ToString(), out index);
                if (!nofail)
                    Compiler.ExceptionListener.Throw($"{this.Name} arguments must be a whole number.",
                        ExceptionType.CompilerException, "{0}");
            }

            var inputAsTobj = input;
            if (inputAsTobj == null)
                Compiler.ExceptionListener.Throw($"Cannot find TObject {input.Name}",
                    ExceptionType.CompilerException, "{0}");

            var getParam = inputAsTobj.ToArray().ToList<string>();
            if (getParam == null)
                Compiler.ExceptionListener.Throw($"Cannot find TParameter in {input.Name}",
                    ExceptionType.CompilerException, "{0}");
            
            if (index == -1)
            {
                getParam.Add(args[0]);
                return getParam.ToArray<string>();
            }

            var ele = getParam.ElementAtOrDefault(index);
            if (ele == null)
                Compiler.ExceptionListener.Throw($"The element at {index} is null.",
                    ExceptionType.NullReferenceException, "{0}");
            getParam[index] = args[0];

            return getParam.ToArray<string>();

        }
    }
}
