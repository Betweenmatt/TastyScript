﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using TastyScript.Lang.Extensions;

namespace TastyScript.Lang.Tokens
{
    [Serializable]
    internal class Token
    {
        public string Name { get; protected set; }
        protected string _value = "[Type.Token]";
        public virtual string Value
        {
            get
            {
                if (_action != null)
                    return _action();
                return _value;
            }
        }
        public List<EDefinition> Extensions { get; set; }
        public string Line { get; protected set; }
        public bool Locked { get; protected set; }
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
            Locked = locked;
        }
        public Token(string name, Func<string> action, string line, bool locked = false)
        {
            Name = name;
            _action = action;
            _value = "[Type.TAction]";
            Line = line;
            Locked = locked;
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
        public string[] ToArray()
        {
            throw new NotImplementedException();
            var commaRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            return commaRegex.Split(Value);
        }
        public override string ToString()
        {
            //return public to get the action response when used 
            return Value;
        }
    }
    [Serializable]
    internal class TArray : Token
    {
        public string[] Arguments { get; private set; }
        public override string Value {
            get
            {
                return ToString();
            }
        }
        public TArray(string name, string[] val, string line, bool locked = false)
        {
            Name = name;
            _value = "[Type.TArray]";
            Arguments = val;
            Line = line;
            Locked = locked;
        }
        public TArray(string name, string val, string line, bool locked = false)
        {
            Name = name;
            _value = "[Type.TArray]";
            Arguments = ReturnArgsArray(val.Substring(1, val.Length-2));
            Line = line;
            Locked = locked;
        }
        public string[] Add(string s)
        {
            Arguments = (Arguments ?? Enumerable.Empty<string>()).Concat(Enumerable.Repeat(s, 1)).ToArray();
            return Arguments;
        }
        public void Remove(int index)
        {
            var newArgs = Arguments.ToList();
            newArgs.RemoveAt(index);
            Arguments = newArgs.ToArray();
        }
        public override string ToString()
        {
            return "[" + $"{string.Join(",",Arguments)}" + "]";
        }
        public string[] ReturnArgsArray()
        {
            return Arguments;
        }
        private string[] ReturnArgsArray(string args)
        {
            if (args == null)
                return new string[] { };
            //Console.WriteLine(Arguments);
            var output = args;
            var index = 0;
            Dictionary<string, string> stringParts = new Dictionary<string, string>();
            Dictionary<string, string> parenParts = new Dictionary<string, string>();
            if (args.Contains("\""))
            {
                var stringRegex = new Regex("\"([^\"\"]*)\"", RegexOptions.Multiline);
                foreach (var s in stringRegex.Matches(output))
                {
                    var token = "{AutoGeneratedToken" + index + "}";
                    stringParts.Add(token, s.ToString());
                    output = output.Replace(s.ToString(), token);
                    index++;
                }
            }
            if (args.Contains("[") && args.Contains("]"))
            {
                var reg = new Regex(@"(?:(?:\[(?>[^\[\]]+|\[(?<number>)|\](?<-number>))*(?(number)(?!))\])|[^[]])+");
                foreach (var x in reg.Matches(output))
                {
                    var token = "{AutoGeneratedToken" + index + "}";
                    parenParts.Add(token, x.ToString());
                    output = output.Replace(x.ToString(), token);
                    index++;
                }
            }
            var splode = output.Split(',');
            //List<string> returnParens = new List<string>();

            for (var i = 0; i < splode.Length; i++)
            {
                foreach (var p in parenParts)
                {
                    if (splode[i].Contains(p.Key))
                        splode[i] = splode[i].Replace(p.Key, p.Value);
                }
            }
            for (var i = 0; i < splode.Length; i++)
            {
                foreach (var p in stringParts)
                {
                    if (splode[i].Contains(p.Key))
                        splode[i] = splode[i].Replace(p.Key, p.Value).Replace("\"", "");
                }
            }
            return splode;
        }
    }
    [Serializable]
    internal class TFunction : Token
    {
        public IBaseFunction Function { get; private set; }
        public string[] Arguments { get; private set; }
        public IBaseFunction CallingFunction { get; private set; }
        public bool BlindExecute { get; set; }
        public LoopTracer Tracer { get; private set; }
        /// <summary>
        /// callingFunction is the function that is calling(usually `this`), func is the function being called
        /// </summary>
        /// <param name="func"></param>
        /// <param name="ext"></param>
        /// <param name="args"></param>
        /// <param name="callingFunction"></param>
        public TFunction(IBaseFunction func, List<EDefinition> ext, string args, IBaseFunction callingFunction, LoopTracer t = null)
        {
            Name = func.Name;
            Function = func;
            _value = "[Type.TFunction]";
            Extensions = ext;
            Arguments = ReturnArgsArray(args);
            CallingFunction = callingFunction;
            if (t != null)
                Tracer = t;
            else
                if (callingFunction != null)
                Tracer = callingFunction.Tracer;
            else
                Tracer = null;
        }/// <summary>
         /// callingFunction is the function that is calling(usually `this`), func is the function being called
         /// </summary>
         /// <param name="func"></param>
         /// <param name="ext"></param>
         /// <param name="args"></param>
         /// <param name="callingFunction"></param>
        public TFunction(IBaseFunction func, List<EDefinition> ext, string[] args, IBaseFunction callingFunction, LoopTracer t = null)
        {
            Name = func.Name;
            Function = func;
            _value = "[Type.TFunction]";
            Extensions = ext;
            Arguments = args;
            CallingFunction = callingFunction;
            if (t != null)
                Tracer = t;
            else
                if (callingFunction != null)
                Tracer = callingFunction.Tracer;
            else
                Tracer = null;
        }
        public void SetTracer(LoopTracer t)
        {
            Tracer = t;
        }
        public string[] ReturnArgsArray()
        {
            return Arguments;
        }
        private string[] ReturnArgsArray(string args)
        {
            if (args == null)
                return new string[] { };
            //Console.WriteLine(Arguments);
            var output = args;
            var index = 0;
            Dictionary<string, string> stringParts = new Dictionary<string, string>();
            Dictionary<string, string> parenParts = new Dictionary<string, string>();
            if (args.Contains("\""))
            {
                var stringRegex = new Regex("\"([^\"\"]*)\"", RegexOptions.Multiline);
                foreach (var s in stringRegex.Matches(output))
                {
                    var token = "{AutoGeneratedToken" + index + "}";
                    stringParts.Add(token, s.ToString());
                    output = output.Replace(s.ToString(), token);
                    index++;
                }
            }
            if (args.Contains("[") && args.Contains("]"))
            {
                //var reg = new Regex(@"(?:(?:{(?>[^{}]+|{(?<number>)|}(?<-number>))*(?(number)(?!))})|{^{}})+");
                var reg = new Regex(@"(?:(?:\[(?>[^\[\]]+|\[(?<number>)|\](?<-number>))*(?(number)(?!))\])|[^[]])+");
                foreach (var x in reg.Matches(output))
                {
                    var token = "{AutoGeneratedToken" + index + "}";
                    parenParts.Add(token, x.ToString());
                    output = output.Replace(x.ToString(), token);
                    index++;
                }
            }
            var splode = output.Split(',');
            //List<string> returnParens = new List<string>();
            
            for (var i = 0; i < splode.Length; i++) 
            {
                foreach(var p in parenParts)
                {
                    if (splode[i].Contains(p.Key))
                        splode[i] = splode[i].Replace(p.Key, p.Value);//.Substring(1,p.Value.Length-2));
                }
            }
            for (var i = 0; i < splode.Length; i++)
            {
                foreach (var p in stringParts)
                {
                    if (splode[i].Contains(p.Key))
                        splode[i] = splode[i].Replace(p.Key, p.Value).Replace("\"","");
                }
            }
            //foreach (var x in splode)
             //   Console.WriteLine("   " + x);
            return splode;
        }
    }
    //
    //OLD VVVV
    //
    [Serializable]
    internal abstract class Token<T> : IToken<T>
    {
        protected abstract string _name { get; set; }
        public string Name { get { return _name; } }
        protected abstract BaseValue<T> _value { get; set; }
        public virtual BaseValue<T> Value { get { return _value; } }
        //public TParameter Arguments { get; set; }
        public List<EDefinition> Extensions { get; set; }
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
        //TParameter Arguments { get; set; }
        object GetValue();
        List<EDefinition> Extensions { get; set; }
    }
    [Obsolete]
    internal interface IToken<T> : IBaseToken
    {
        BaseValue<T> Value { get; }
    }
    [Obsolete]
    internal class IBaseValue { }
    [Serializable]
    [Obsolete]
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
