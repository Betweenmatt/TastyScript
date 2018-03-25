using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    [Extension("RemoveAt", varExtension: true)]
    internal class ExtensionRemoveAt : EDefinition
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if (args == null || args.ElementAtOrDefault(0) == null)
                Compiler.ExceptionListener.Throw($"{this.Name} arguments cannot be null.",
                    ExceptionType.CompilerException, input.Line);
            int index = 0;
            var nofail = int.TryParse(args[0].ToString(), out index);
            if (!nofail)
                Compiler.ExceptionListener.Throw($"{this.Name} arguments must be a whole number.",
                    ExceptionType.CompilerException, input.Line);

            if (input == null || input.Value == null)
                Compiler.ExceptionListener.Throw($"Extension cannot extend null.");
            var inputAsTobj = new TArray("arr", input.Value, input.Line);
            inputAsTobj.Remove(index);
            return inputAsTobj;
        }
    }
}
