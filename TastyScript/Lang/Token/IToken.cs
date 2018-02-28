using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TastyScript.Lang.Func;

namespace TastyScript.Lang.Token
{
    public enum Type { String, Number, Member, Extension, Parameters, Variable }
    public enum SubType { Single, Array }
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
    public class TString : Token<string>
    {
        protected override string _name { get; set; }
        protected override BaseValue<string> _value { get; set; }
        //override value to use action if it is not null
        public override BaseValue<string> Value
        {
            get
            {
                if (_action == null)
                {
                    return _value;
                }
                else
                {
                    return new BaseValue<string>(_action());
                }
            }
        }
        private Func<string> _action;
        public TString(string name, string val)
        {
            _name = name;
            _value = new BaseValue<string>(val);
        }
        public TString(string name, Func<string> action)
        {
            _name = name;
            _action = action;
        }
    }
    [Serializable]
    public class TNumber : Token<double>
    {
        protected override string _name { get; set; }
        protected override BaseValue<double> _value { get; set; }
        public TNumber(string name, double val)
        {
            _name = name;
            _value = new BaseValue<double>(val);
        }
    }
    [Serializable]
    public class TVariable : Token<IBaseToken>
    {
        protected override string _name { get; set; }
        protected override BaseValue<IBaseToken> _value { get; set; }
        public TVariable(string name, IBaseToken value)
        {
            _name = name;
            _value = new BaseValue<IBaseToken>(value);
        }
        public void SetValue(IBaseToken value)
        {
            _value = new BaseValue<IBaseToken>(value);
        }
    }
    [Serializable]
    public class TAction : Token<string>
    {
        protected override string _name { get; set; }
        protected override BaseValue<string> _value { get; set; }
        public TAction(string name, Func<string> value)
        {

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
        public override string ToString()
        {
            return Value.ToString();
        }
        public System.Type GetMemberType()
        {
            return typeof(T);
        }
    }
    public interface IBaseToken { string Name { get; } TParameter Arguments { get; set; } System.Type GetMemberType(); List<IExtension> Extensions { get; set; } }

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
