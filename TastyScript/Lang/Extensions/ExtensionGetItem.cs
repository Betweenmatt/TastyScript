using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    /// <summary>
    /// Gets the item from the extended collection at the given index
    /// </summary>
    [Extension("GetItem", new string[] { "index" }, FunctionObsolete: true)]
    [Serializable]
    internal class ExtensionGetItem : EDefinition
    {
        public override string[] Extend(Token input)
        {
            var args = Extend();
            if (args == null || args.ElementAtOrDefault(0) == null)
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
            
            var ele = getParam.ElementAtOrDefault(index);
            if (ele == null)
                Compiler.ExceptionListener.Throw($"The element at {index} is null.",
                    ExceptionType.NullReferenceException, "{0}");
            getParam[index] = args[0];

            return getParam.ToArray<string>();

        }
        /*
        public override TParameter Extend(IBaseToken input)
        {
            var args = Extend();
            if (args == null || args.ElementAtOrDefault(0) == null)
                Compiler.ExceptionListener.Throw($"{this.Name} arguments cannot be null.",
                    ExceptionType.CompilerException, "{0}");

            int index = 0;
            var nofail = int.TryParse(args[0].ToString(), out index);
            if (!nofail)
                Compiler.ExceptionListener.Throw($"{this.Name} arguments must be a whole number.",
                    ExceptionType.CompilerException, "{0}");
            var inputAsTobj = input as TObject;
            if (inputAsTobj == null)
                Compiler.ExceptionListener.Throw($"Cannot find TObject {input.Name}",
                    ExceptionType.CompilerException, "{0}");
            Console.WriteLine(inputAsTobj.Value.Value);
            var getParam = inputAsTobj.Value.Value.ToString().Replace("[", "").Replace("]", "").Split(',');
            if (getParam == null)
                Compiler.ExceptionListener.Throw($"Cannot find TParameter in {input.Name}",
                    ExceptionType.CompilerException, "{0}");

            var ele = getParam.ElementAtOrDefault(index);
            if (ele == null)
                Compiler.ExceptionListener.Throw($"The element at {index} is null.",
                    ExceptionType.NullReferenceException, "{0}");
            return new TParameter($"AnonArray{index}", new List<IBaseToken>() { new TObject("", ele) });

        }*/
    }
}
