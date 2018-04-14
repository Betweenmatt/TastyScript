using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Function;
using TastyScript.IFunction.Tokens;

namespace TastyScript.IFunction.Extension
{
    public class CustomExtension:BaseExtension
    {
        public BaseFunction FunctionReference;
        public override string[] Extend(BaseFunction input)
        {
            List<string> temp = new List<string>();
            temp.Add(input.Name);
            temp.AddRange(base.Extend());
            var tfunc = new TFunction(FunctionReference, temp.ToArray());
            return Execute(tfunc.TryParse().ToString());
        }
        public override Token Extend(Token input)
        {
            List<string> temp = new List<string>();
            temp.Add(input.ToString());
            temp.AddRange(base.Extend());
            var tfunc = new TFunction(FunctionReference, temp.ToArray());
            return tfunc.TryParse();
        }
    }
}
