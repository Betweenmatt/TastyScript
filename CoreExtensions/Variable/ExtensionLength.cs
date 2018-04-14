using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreExtensions.Variable
{
    [Extension("Length", varExtension: true)]
    [Serializable]
    public class ExtensionLength : BaseExtension
    {
        public override Token Extend(Token input)
        {
            var inputAsTobj = new TArray("arr", input.Value);
            return new Token("leng", inputAsTobj.Arguments.Length.ToString());
        }
    }
}
