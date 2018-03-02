using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Token
{
    [Serializable]
    internal class TFunction : Token<IBaseFunction>
    {
        protected override string _name { get; set; }
        protected override BaseValue<IBaseFunction> _value { get; set; }
        public TFunction(string name, IBaseFunction val)
        {
            _name = name;
            _value = new BaseValue<IBaseFunction>(val);
        }
    }
}
