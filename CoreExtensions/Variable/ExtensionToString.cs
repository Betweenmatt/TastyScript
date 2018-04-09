using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreExtensions.Variable
{
    [Extension("ToString",varExtension:true)]
    public class ExtensionToString : BaseExtension
    {
        public override Token Extend(Token input)
        {
            if (input == null || input.Value == null)
            {
                Manager.Throw("Variable cannot be null");
                return null;
            }
            if(input.ToString().Contains("[") && input.ToString().Contains("]"))
            {
                var str = input.ToString().Replace("[","").Replace("]","").CleanString();
                return new Token("tostr", str,input.Line);
            }
            return new Token("tostr", "null", input.Line);
        }
    }
}
