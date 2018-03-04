using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TastyScript.Lang.Extensions;

namespace TastyScript.Lang.Token
{
    [Serializable]
    internal abstract class Token<T> : IToken<T>
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
        public object GetValue()
        {
            return Value.Value;
        }
    }
    internal interface IBaseToken
    {
        bool Locked { get; }
        string Name { get; }
        TParameter Arguments { get; set; }
        object GetValue();
        List<IExtension> Extensions { get; set; }
    }

    internal interface IToken<T> : IBaseToken
    {
        BaseValue<T> Value { get; }
    }

    internal class IBaseValue { }
    [Serializable]
    internal class BaseValue<T> : IBaseValue
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
