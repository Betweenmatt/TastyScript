using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TastyScript.Lang.Func;

namespace TastyScript.Lang.Token
{
    [Serializable]
    public class TFunction : Token<IBaseFunction>
    {
        protected override string _name { get; set; }
        protected override BaseValue<IBaseFunction> _value { get; set; }
        public TFunction(string name, IBaseFunction val)
        {
            _name = name;
            _value = new BaseValue<IBaseFunction>(val);
        }
    }
    [Serializable]
    public class TObject : Token<object>
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
        public void SetValue(string val)
        {
            _value = new BaseValue<object>(val);
        }
    }
    [Serializable]
    public class TParameter : Token<List<IBaseToken>>
    {
        protected override string _name { get; set; }
        protected override BaseValue<List<IBaseToken>> _value { get; set; }
        public TParameter(string name, List<IBaseToken> value)
        {
            _name = name;
            _value = new BaseValue<List<IBaseToken>>(value);
        }
    }

    [Serializable]
    public abstract class Token<T> : IToken<T>
    {
        protected abstract string _name { get; set; }
        public string Name { get { return _name; } }
        protected abstract BaseValue<T> _value { get; set; }
        public virtual BaseValue<T> Value { get { return _value; } }
        public TParameter Arguments { get; set; }
        public List<IExtension> Extensions { get; set; }
        public bool Locked { get; protected set; }
        public override string ToString()
        {
            return Value.ToString();
        }
        public System.Type GetMemberType()
        {
            return typeof(T);
        }
    }
    public interface IBaseToken { bool Locked { get; } string Name { get; } TParameter Arguments { get; set; } System.Type GetMemberType(); List<IExtension> Extensions { get; set; } }

    public interface IToken<T> : IBaseToken
    {
        BaseValue<T> Value { get; }
    }
    public class IBaseValue { }
    [Serializable]
    public class BaseValue<T> : IBaseValue
    {
        public T Value { get; private set; }

        public BaseValue(T val)
        {
            Value = val;
        }
        public void SetValue(T val)
        {
            Value = val;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
