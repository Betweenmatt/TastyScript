using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Token;

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
        public override TParameter Extend(IBaseToken input)
        {
            var args = Arguments.Value.Value;
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
            
            var inputAsTobj = input as TObject;
            if (inputAsTobj == null)
                Compiler.ExceptionListener.Throw($"Cannot find TObject {input.Name}",
                    ExceptionType.CompilerException, "{0}");
            Console.WriteLine(inputAsTobj.Value.Value);
            var getParam = inputAsTobj.Value.Value.ToString().Replace("[", "").Replace("]", "").Split(',');
            if (getParam == null)
                Compiler.ExceptionListener.Throw($"Cannot find TParameter in {input.Name}",
                    ExceptionType.CompilerException, "{0}");
            List<IBaseToken> tempParam = new List<IBaseToken>();
            foreach (var x in getParam)
                tempParam.Add(new TObject("", x));
            if (index == -1)
            {
                tempParam.Add(new TObject("",args[0]));
                return new TParameter("AnonArray", tempParam);
            }
            var ele = tempParam.ElementAtOrDefault(index);
            if (ele == null)
                Compiler.ExceptionListener.Throw($"The element at {index} is null.",
                    ExceptionType.NullReferenceException, "{0}");
            tempParam[index] = new TObject("", args[0]);

            return new TParameter($"AnonArray{index}", tempParam );

        }
    }
}
