using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    [Extension("ToString",varExtension:true)]
    internal class ExtensionToString : EDefinition
    {
        public override Token Extend(Token input)
        {
            if (input == null || input.Value == null)
                throw new Exception("Variable cannot be null");
            if(input.ToString().Contains("[") && input.ToString().Contains("]"))
            {
                var str = input.ToString().Replace("[","").Replace("]","").CleanString();
                return new Token("tostr", str,input.Line);
            }
            return new Token("tostr", "null", input.Line);
        }
    }
}
