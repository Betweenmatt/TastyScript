using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Extensions
{
    [Extension("GetIndex")]
    [Serializable]
    internal class ExtensionGetIndex : EDefinition
    {
        public override TParameter Extend(IBaseToken input)
        {
            var args = Arguments.Value.Value;
            if (args == null || args.ElementAtOrDefault(0) == null)
                Compiler.ExceptionListener.Throw($"{this.Name} arguments cannot be null.",
                    ExceptionType.CompilerException, "{0}");
            
            var inputAsTobj = input as TObject;
            if (inputAsTobj == null)
                Compiler.ExceptionListener.Throw($"Cannot find TObject {input.Name}",
                    ExceptionType.CompilerException, "{0}");

            var getParam = inputAsTobj.Value.Value.ToString().Replace("[", "").Replace("]", "").Split(',');
            if (getParam == null)
                Compiler.ExceptionListener.Throw($"Cannot find TParameter in {input.Name}",
                    ExceptionType.CompilerException, "{0}");

            var ele = getParam.FirstOrDefault(f => f == args[0].ToString());
            if (ele == null)
                Compiler.ExceptionListener.Throw($"The element {args[0].ToString()} is not defined in this collection.",
                    ExceptionType.NullReferenceException, "{0}");
            return new TParameter($"AnonArray", new List<IBaseToken>() { new TObject("", Array.IndexOf(getParam,ele)) });

        }
    }
}
