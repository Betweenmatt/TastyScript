using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TastyScript.IFunction.Containers;

namespace TastyScript.IFunction.Tokens
{
    public class Token
    {
        public string Name { get; protected set; }
        protected string _value = "<Type.Token>";
        public virtual string Value
        {
            get
            {
                if (_action != null)
                    return _action();
                return _value;
            }
        }
        public ExtensionList Extensions { get; set; }
        public string Line { get; protected set; }
        public bool IsLocked { get; protected set; }
        protected Func<string> _action;
        /// <summary>
        /// line param is the line reference for the exception handler to track down
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="line"></param>
        public Token() { }
        public Token(string name, string val, string line, bool locked = false)
        {
            Name = name;
            _value = val;
            Line = line;
            IsLocked = locked;
        }
        public Token(string name, Func<string> action, string line, bool locked = false)
        {
            Name = name;
            _action = action;
            _value = "<Type.TAction>";
            Line = line;
            IsLocked = locked;
        }
        public void SetValue(string value)
        {
            _value = value;
        }
        public void SetLine(string line)
        {
            Line = line;
        }
        public void SetName(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            if (Value == null)
                return "null";
            //return public to get the action response when used 
            return Value;
        }
    }
}
