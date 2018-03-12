using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    [Extension("Length", varExtension: true)]
    [Serializable]
    internal class ExtensionLength : EDefinition
    {
        public override Token Extend(Token input)
        {
            var inputAsTobj = new TArray("arr", input.Value, input.Line);
            return new Token("leng", inputAsTobj.Arguments.Length.ToString(), input.Line);
        }
    }
}
