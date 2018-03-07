using System;
using System.Collections.Generic;
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
        bool Invoking { get; }
        string Value { get; }
        string[] GetInvokeProperties();
        void SetInvokeProperties(string[] args);
        void TryParse(TFunction caller);
        void SetProperties(string name, string[] args, bool invoking, bool isSealed);
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
        public List<EDefinition> _extensions = new List<EDefinition>();
        public List<EDefinition> Extensions { get { return _extensions; } set { _extensions = value; } }
        public List<Line> Lines { get; set; }
        public string LineValue { get; protected set; }
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
            ProvidedArgs = new List<Token>();
            LocalVariables = new List<Token>();

            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach(var a in anonRegexMatches)
            {
                var func = new AnonymousFunction(a.ToString(), true);
                TokenParser.FunctionList.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            Name = value.Split('.')[1].Split('(')[0];
            var b = Compiler.PredefinedList.FirstOrDefault(f => f.Name == Name);
            if (b != null)
                if (b.Sealed == true)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                        $"Invalid Operation. Cannot create a new instance of a Sealed function: {Name}.", value));
                }
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
            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new AnonymousFunction(a.ToString(), true);
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
                    if (args.Length > 0)
                    {
                        for (var i = 0; i < args.Length; i++)
                        {
                            var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                            ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                        }
                    }
                
            }
            var guts = Value.Split('{')[1].Split('}');
            var lines = guts[0].Split(';');
            //Lines = new List<Line>();
            foreach (var l in lines)
                new Line(l, this);
                //Lines.Add(new Line(l, this));
            //Parse();
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
                if (args.Length > 0)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                        ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                    }
                }
            }
            var guts = Value.Split('{')[1].Split('}');
            var lines = guts[0].Split(';');
            //Lines = new List<Line>();
            foreach (var l in lines)
                new Line(l, this);
                //Lines.Add(new Line(l, this));
            //Parse();
        }
        public virtual string Parse()
        {
            /*
            foreach (var line in Lines)
            {
                if (!TokenParser.Stop)
                {
                    if (Tracer == null || (!Tracer.Continue && !Tracer.Break))
                        TryParseMember(line.Token);
                }
                else if (TokenParser.Stop && BlindExecute)
                {
                    //Console.WriteLine($"\t{DateTime.Now.ToString("HH:mm:ss.fff")}:\t{line.Token.Name}");
                    TryParseMember(line.Token);
                }
            }
            return "";
            */
            return "";
        }
        private void TryParseMember(TFunction t)
        {
            /*
            if (t == null)
                return;
            if (BlindExecute)
                t.BlindExecute = true;
            if (t.Name == "Base")
            {
                var b = Base;
                b.Extensions = new List<EDefinition>();
                if (t.Extensions != null)
                    b.Extensions = t.Extensions;
                if (t.Function.BlindExecute)
                    b.BlindExecute = true;

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
                b.TryParse(t);
                return;
            }
            //change this plz
           
            var z = t.Function;
            if (t.Extensions != null)
            {
                z.Extensions = t.Extensions;
            }
            
            z.TryParse(t);
            return;
            */
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
