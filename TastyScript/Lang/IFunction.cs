using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang
{
    internal interface IBaseFunction : IBaseToken
    {
        List<Token> ProvidedArgs { get; }
        string[] ExpectedArgs { get; }
        IBaseFunction Base { get; }
        string LineValue { get; }
        bool BlindExecute { get; set; }
        LoopTracer Tracer { get; }
        bool Obsolete { get; }
        bool Invoking { get; }
        string Value { get; }
        string[] GetInvokeProperties();
        void SetInvokeProperties(string[] args);
        void TryParse(TFunction caller);
        void SetProperties(string name, string[] args, bool invoking, bool isSealed, bool obsolete);
        bool Sealed { get; }
        List<Token> LocalVariables { get; set; }
        Token ReturnBubble { get; set; }
        bool ReturnFlag { get; set; }
    }
    internal interface IFunction : IBaseFunction
    {
        string Value { get; }
        string Parse();
    }
    internal interface IOverride : IFunction
    {
        string CallBase();
    }
    internal class AnonymousFunction : IFunction
    {
        public string Name { get; protected set; }
        public string Value { get; private set; }
        public string[] ExpectedArgs { get; protected set; }
        public bool Locked { get; protected set; }
        public List<Token> ProvidedArgs { get; protected set; }
        public string Arguments { get; set; }
        public List<EDefinition> _extensions;
        public List<EDefinition> Extensions
        {
            get
            {
                if(_extensions == null)
                    _extensions = new List<EDefinition>();
                return _extensions;
            }
            set
            {
                if (_extensions == null)
                    _extensions = new List<EDefinition>();
                _extensions = value;
            }
        }
        public List<Line> Lines { get; set; }
        public string LineValue { get; protected set; }
        public bool Obsolete { get; private set; }
        public IBaseFunction Base { get; protected set; }
        public bool BlindExecute { get; set; }
        protected string[] invokeProperties;
        public LoopTracer Tracer { get; protected set; }
        private int _generatedTokensIndex = -1;
        public bool Invoking { get; protected set; }
        public bool Sealed { get; private set; }
        public List<Token> LocalVariables { get; set; }
        public TFunction Caller { get; protected set; }
        public Token ReturnBubble { get;  set; }
        public bool ReturnFlag { get;  set; }
        public object GetValue()
        {
            throw new NotImplementedException();
        }
        public int GeneratedTokensIndex
        {
            get
            {
                _generatedTokensIndex++;
                return _generatedTokensIndex;
            }
        }
        public void SetInvokeProperties(string[] args)
        {
            invokeProperties = args;
        }
        public string[] GetInvokeProperties()
        {
            if (invokeProperties == null)
                return new string[] { };
            else
                return invokeProperties;
        }
        public void SetProperties(string name, string[] args, bool invoking, bool isSealed, bool obsolete)
        {
            Name = name;
            ExpectedArgs = args;
            Invoking = invoking;
            Sealed = isSealed;
            Obsolete = obsolete;
        }
        public AnonymousFunction() { }
        //standard constructor
        public AnonymousFunction(string value)
        {
            ProvidedArgs = new List<Token>();
            LocalVariables = new List<Token>();
            
            Name = value.Split('.')[1].Split('(')[0];
            var b = Compiler.PredefinedList.FirstOrDefault(f => f.Name == Name);
            if (b != null)
                if (b.Sealed == true)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                        $"Invalid Operation. Cannot create a new instance of a Sealed function: {Name}.", value));
                }
            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new AnonymousFunction(a.ToString(), true);
                func.Base = Base;
                TokenParser.FunctionList.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
        }
        //this constructor is when function is anonomysly named
        public AnonymousFunction(string value, bool anon)
        {
            ProvidedArgs = new List<Token>();
            LocalVariables = new List<Token>();
            //get top level anonymous functions before everything else
            value = value.Substring(1);
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new AnonymousFunction(a.ToString(), true);
                func.Base = Base;
                TokenParser.FunctionList.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            Value = value;
            Name = "AnonymousFunction"+Compiler.AnonymousFunctionIndex;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            //Name = value.Split('.')[1].Split('(')[0];
        }
        //this is the constructor used when function is an override
        public AnonymousFunction(string value, List<IBaseFunction> predefined)
        {
            ProvidedArgs = new List<Token>();
            LocalVariables = new List<Token>();
            
            
            Name = value.Split('.')[1].Split('(')[0];
            
            var b = predefined.FirstOrDefault(f => f.Name == Name);
            if (b == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Unexpected error. Function failed to override: {Name}.", value));
            if (b.Sealed == true)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"Invalid Operation. Cannot override Sealed function: {Name}.", value));
            }
            Base = b;
            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new AnonymousFunction(a.ToString(), true);
                func.Base = Base;
                TokenParser.FunctionList.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
        }
        public virtual void TryParse(TFunction caller)
        {
            if (caller != null)
            {
                BlindExecute = caller.BlindExecute;
                Tracer = caller.Tracer;
                Caller = caller;
            }
            var findFor = Extensions.FirstOrDefault(f => f.Name == "For") as ExtensionFor;
            if (findFor != null)
            {
                //if for extension exists, reroutes this tryparse method to the loop version without the for check
                ForExtension(caller, findFor);
                return;
            }
            //combine expected args and given args and add them to variabel pool
            if (caller != null && caller.Arguments != null && ExpectedArgs != null && ExpectedArgs.Length > 0)
            {
                ProvidedArgs = new List<Token>();
                var args = caller.ReturnArgsArray();
                if (ExpectedArgs.Length > 0)
                {
                    for (var i = 0; i < ExpectedArgs.Length; i++)
                    {
                        var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                        if (args.ElementAtOrDefault(i) == null)
                            ProvidedArgs.Add(new Token(exp, "null", caller.Line));
                        else
                            ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                    }
                }

            }
            var guts = Value.Split('{')[1].Split('}');
            var lines = guts[0].Split(';');
            foreach (var l in lines)
            {
                new Line(l, this);
            }
        }
        //this overload is when the function is called with the for extension
        public virtual void TryParse(TFunction caller, bool forFlag)
        {
            if (caller != null)
            {
                BlindExecute = caller.BlindExecute;
                Tracer = caller.Tracer;
                Caller = caller;
            }
            //combine expected args and given args and add them to variabel pool
            if (caller != null && caller.Arguments != null && ExpectedArgs != null && ExpectedArgs.Length > 0)
            {
                ProvidedArgs = new List<Token>();
                var args = caller.ReturnArgsArray();
                if (ExpectedArgs.Length > 0)
                {
                    for (var i = 0; i < ExpectedArgs.Length; i++)
                    {
                        var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                        if (args.ElementAtOrDefault(i) == null)
                            ProvidedArgs.Add(new Token(exp, "null", caller.Line));
                        else
                            ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                    }
                }

            }
            var guts = Value.Split('{')[1].Split('}');
            var lines = guts[0].Split(';');
            foreach (var l in lines)
                new Line(l, this);
        }
        public virtual string Parse()
        {
            //this was moved to Line.cs
            return "";
        }
        private void TryParseMember(TFunction t)
        {
            //this was moved to Line.cs
        }
        /// <summary>
        /// Override this method if you do not want to include it as a function.
        /// Overriding this must contain {TryParse(args,true)} or you won't get anywhere!
        /// </summary>
        /// <param name="args"></param>
        /// <param name="findFor"></param>
        protected virtual void ForExtension(TFunction caller, ExtensionFor findFor)
        {
            string[] forNumber = findFor.Extend();
            int forNumberAsNumber = int.Parse(forNumber[0]);
            LoopTracer tracer = new LoopTracer();
            Compiler.LoopTracerStack.Add(tracer);
            Tracer = tracer;
            if (forNumberAsNumber <= 0)
                forNumberAsNumber = int.MaxValue;
            for (var x = 0; x < forNumberAsNumber; x++)
            {
                if (!TokenParser.Stop)
                {
                    if (tracer.Break)
                    {
                        break;
                    }
                    if (tracer.Continue)
                    {
                        tracer.SetContinue(false);//reset continue
                        continue;
                    }

                    TryParse(caller, true);
                }
                else
                {
                    break;
                }
            }
            Compiler.LoopTracerStack.Remove(tracer);
            tracer = null;
        }
    }
}
