using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.TokenOLD
{
    [Serializable]
    internal class TObject : Token<object>
    {
        protected override string _name { get; set; }
        protected override BaseValue<object> _value { get; set; }
        //override value to use action if it is not null
        public override BaseValue<object> Value
        {
            get
            {
                if (_action == null)
                {
                    return _value;
                }
                else
                {
                    return new BaseValue<object>(_action());
                }
            }
        }
        private Func<object> _action;
        public TObject(string name, object val, bool locked = false)
        {
            Locked = locked;
            _name = name;
            _value = new BaseValue<object>(val);
        }
        public TObject(string name, Func<object> action, bool locked = false)
        {
            Locked = locked;
            _name = name;
            _action = action;
        }
        public void SetValue(object val)
        {
            _value = new BaseValue<object>(val);
        }
    }
}
