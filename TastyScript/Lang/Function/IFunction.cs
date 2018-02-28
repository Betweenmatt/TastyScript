using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Func
{
    public interface IBaseFunction : IBaseToken
    {
        List<IBaseToken> GeneratedTokens { get; set; }
        int GeneratedTokensIndex { get; }
        List<IBaseToken> VariableTokens { get; set; }
        List<IBaseToken> ProvidedArgs { get; set; }
        string[] ExpectedArgs { get; set; }
        IBaseFunction Base { get; }
        string LineValue { get; }
        bool BlindExecute { get; set; }
        bool Invoking { get; }
        TParameter GetInvokeProperties();
        void SetInvokeProperties(TParameter args);
        void TryParse(TParameter args, string lineval = "{0}");
        void SetProperties(string name, string[] args, bool invoking, bool isSealed);
        bool Sealed { get; }
    }
    public interface IFunction<T> : IBaseFunction
    {
        string Value { get; }
        T Parse(TParameter args);
    }
    public interface IOverride<T> : IFunction<T>
    {
        IFunction<T> CallBase(TParameter args);
        void TryCallBase(TParameter args);
    }
    public class AnonymousFunction<T> : IFunction<T>
    {
        public string Name { get; protected set; }
        public string Value { get; private set; }
        public string[] ExpectedArgs { get; set; }
        public List<IBaseToken> ProvidedArgs { get; set; }
        public List<IBaseToken> GeneratedTokens { get; set; }
        public TParameter Arguments { get; set; }
        public List<IExtension> _extensions = new List<IExtension>();
        public List<IExtension> Extensions { get { return _extensions; } set { _extensions = value; } }
        public List<IBaseToken> VariableTokens { get; set; }
        public List<Line> Lines { get; set; }
        public string LineValue { get; protected set; }
        public IBaseFunction Base { get; protected set; }
        public bool BlindExecute { get; set; }
        protected TParameter invokeProperties;
        private int _generatedTokensIndex = -1;
        public bool Invoking { get; protected set; }
        public bool Sealed { get; private set; }
        public int GeneratedTokensIndex
        {
            get
            {
                _generatedTokensIndex++;
                return _generatedTokensIndex;
            }
        }
        public void SetInvokeProperties(TParameter args)
        {
            invokeProperties = args;
        }
        public TParameter GetInvokeProperties()
        {
            return invokeProperties;
        }
        public void SetProperties(string name, string[] args, bool invoking, bool isSealed)
        {
            Name = name;
            ExpectedArgs = args;
            Invoking = invoking;
            Sealed = isSealed;
        }
        public AnonymousFunction() { }
        //standard constructor
        public AnonymousFunction(string value)
        {
            GeneratedTokens = new List<IBaseToken>();
            VariableTokens = new List<IBaseToken>();
            ProvidedArgs = new List<IBaseToken>();

            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach(var a in anonRegexMatches)
            {
                var func = new AnonymousFunction<object>(a.ToString(), true);
                TokenParser.FunctionList.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            Name = value.Split('.')[1].Split('(')[0];
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
        }
        //this constructor is when function is anonomysly named
        public AnonymousFunction(string value, bool anon)
        {
            GeneratedTokens = new List<IBaseToken>();
            VariableTokens = new List<IBaseToken>();
            ProvidedArgs = new List<IBaseToken>();

            //get top level anonymous functions before everything else
            value = value.Substring(1);
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new AnonymousFunction<object>(a.ToString(), true);
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
            GeneratedTokens = new List<IBaseToken>();
            VariableTokens = new List<IBaseToken>();

            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new AnonymousFunction<object>(a.ToString(), true);
                TokenParser.FunctionList.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            Name = value.Split('.')[1].Split('(')[0];
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            var b = predefined.FirstOrDefault(f => f.Name == Name);
            if (b == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Unexpected error. Function failed to override: {Name}.", value));
            if (b.Sealed == true)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"Invalid Operation. Cannot override Sealed function: {Name}.", value));
            }
            Base = b;
        }
        public virtual void TryParse(TParameter args, string lineval = "{0}")
        {
            LineValue = lineval;
            var findFor = Extensions.FirstOrDefault(f => f.Name == "For") as ExtensionFor;
            if (findFor != null)
            {
                //if for extension exists, reroutes this tryparse method to the loop version without the for check
                ForExtension(args, findFor, lineval);
                return;
            }
            //combine expected args and given args and add them to variabel pool
            if (args != null)
            {
                ProvidedArgs = new List<IBaseToken>();
                for (var i = 0; i < args.Value.Value.Count; i++)
                {
                    var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                    ProvidedArgs.Add(new TVariable(exp, args.Value.Value[i]));
                }
            }
            var guts = Value.Split('{')[1].Split('}');
            var lines = guts[0].Split(';');
            Lines = new List<Line>();
            foreach (var l in lines)
                Lines.Add(new Line(l, this));
            Parse(args);
        }
        //this override is when the function is called with the for extension
        public virtual void TryParse(TParameter args, bool forFlag, string lineval = "{0}")
        {
            LineValue = lineval;
            if (args != null)
            {
                ProvidedArgs = new List<IBaseToken>();
                for (var i = 0; i < args.Value.Value.Count; i++)
                {
                    var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                    ProvidedArgs.Add(new TVariable(exp, args.Value.Value[i]));
                }
            }
            var guts = Value.Split('{')[1].Split('}');
            var addScolons = guts[0].Replace("\r", ";").Replace("\n", ";");
            addScolons = addScolons.Replace(";;;", ";").Replace(";;", ";");
            var lines = addScolons.Split(';');
            Lines = new List<Line>();
            foreach (var l in lines)
                Lines.Add(new Line(l, this));
            Parse(args);
        }
        public virtual T Parse(TParameter args)
        {
            foreach (var line in Lines)
            {
                foreach (var token in line.Tokens)
                {
                    if (!TokenParser.Stop)
                        TryParseMember(token, line.Value);
                    else if (TokenParser.Stop && BlindExecute)
                        TryParseMember(token, line.Value);
                }
            }
            return default(T);
        }
        private void TryParseMember(IBaseToken t, string lineval)
        {
            if (t.Name.Contains("{AnonGeneratedToken"))
            {
                var trystring = t as TString;
                //check for anonymous function alone
                if(trystring != null)
                {
                    var tryobj = TokenParser.FunctionList.FirstOrDefault(f => f.Name == trystring.ToString());
                    if(tryobj != null)
                    {
                        tryobj.TryParse(null,lineval);
                        return;
                    }
                }
                return;
            }
            if (t.Name == "Base")
            {
                var b = Base;
                b.Extensions = new List<IExtension>();
                if (t.Extensions != null)
                    b.Extensions = t.Extensions;

                ///This is the whitelist for passing extensions to the Base function
                ///
                if (Extensions != null)
                {
                    foreach (var x in Extensions)
                    {
                        if (x.Name == "Concat" ||
                            x.Name == "Color" ||
                            x.Name == "Threshold")
                            b.Extensions.Add(x);
                    }
                }
                b.TryParse(t.Arguments, lineval);
                return;
            }
            var z = t as TFunction;
            if (t.Extensions != null)
            {
                z.Value.Value.Extensions = t.Extensions;
            }
            z.Value.Value.TryParse(t.Arguments, lineval);
            return;
        }
        /// <summary>
        /// Override this method if you do not want to include it as a function.
        /// Overriding this must contain {TryParse(args,true)} or you won't get anywhere!
        /// </summary>
        /// <param name="args"></param>
        /// <param name="findFor"></param>
        protected virtual void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            TParameter forNumber = findFor.Extend();
            int forNumberAsNumber = int.Parse(forNumber.Value.Value[0].ToString());
            if (forNumberAsNumber != 0)
            {
                for (var x = 0; x < forNumberAsNumber; x++)
                {
                    if (!TokenParser.Stop)
                        TryParse(args, true, lineval);
                }
            }
            else
            {
                while (!TokenParser.Stop)
                {
                    TryParse(args, true, lineval);
                }
            }
        }
        public string ValueToString()
        {
            //change this to return value at some point
            return "function." + Name.ToString();
        }
        public System.Type GetMemberType()
        {
            return typeof(object);
        }
    }
}
