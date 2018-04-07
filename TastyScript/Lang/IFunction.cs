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
    public interface IBaseFunction : IBaseToken
    {
        int UID { get; }
        TokenStack ProvidedArgs { get; }
        string[] ExpectedArgs { get; }
        IBaseFunction Base { get; }
        bool Async { get; }
        string LineValue { get; }
        bool BlindExecute { get; set; }
        LoopTracer Tracer { get; }
        bool Obsolete { get; }
        string[] Alias { get; }
        bool Invoking { get; }
        string Value { get; }
        bool IsAnonymous { get; }
        string[] GetInvokeProperties();
        void SetInvokeProperties(string[] args, List<Token> vars, List<Token> oldargs);
        void TryParse(TFunction caller);
        void SetProperties(string name, string[] args, bool invoking, bool isSealed, bool obsolete, string[] alias, bool anon);
        bool Sealed { get; }
        bool IsLoop { get; }
        TokenStack LocalVariables { get; set; }
        Token ReturnBubble { get; set; }
        bool ReturnFlag { get; set; }
        Dictionary<string, string> Directives { get; }
        bool Override { get; }
        void SetBase(IBaseFunction func);
        void SetSealed(bool flag);
        TFunction Caller { get; }
        //this method sets the return flag and value, and tries to return
        //from the calling function if it is also anonymous
        void ReturnToTopOfBubble(Token value);
        string Parse();
    }
    public interface IOverride
    {
        string CallBase();
    }
    public class AnonymousFunction : IBaseFunction
    {
        public int UID { get; protected set; }
        private static int _uidIndex = -1;
        public string Name { get; protected set; }
        public string Value { get; private set; }
        public string[] ExpectedArgs { get; protected set; }
        public bool Locked { get; protected set; }
        public bool Async { get; set; }
        public bool IsAnonymous { get; private set; }
        public TokenStack ProvidedArgs { get; protected set; }
        public string Arguments { get; set; }
        public Dictionary<string, object> DynamicDictionary { get; set; }
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
        private IBaseFunction _base;
        public IBaseFunction Base {
            get
            {
                if (IsAnonymous)
                    return Caller.CallingFunction.Base;
                return _base;
            }
            protected set
            {
                _base = value;
            }
        }
        public string[] Alias { get; protected set; }
        public bool BlindExecute { get; set; }
        protected string[] invokeProperties;
        public LoopTracer Tracer { get; protected set; }
        private int _generatedTokensIndex = -1;
        public bool Invoking { get; protected set; }
        public bool Sealed { get; private set; }
        public TokenStack LocalVariables { get; set; }
        public TFunction Caller { get; protected set; }
        public Token ReturnBubble { get;  set; }
        public bool ReturnFlag { get;  set; }
        public Dictionary<string,string> Directives { get; protected set; }
        public bool Override { get; protected set; }
        public bool IsLoop { get; protected set; }
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
        public void SetInvokeProperties(string[] args, List<Token> vars, List<Token> oldargs)
        {
            invokeProperties = args;
            //if (LocalVariables == null)
            //trying emptying local vars each invoke, it might break things tho
            LocalVariables = new TokenStack();
            vars.RemoveAll(r => r.Name == "");
            oldargs.RemoveAll(r => r.Name == "");
            LocalVariables.AddRange(vars);
            LocalVariables.AddRange(oldargs);//add the parameters from the calling function to this functions local var stack
        }
        public void SetBase(IBaseFunction func)
        {
            if(func.Sealed)
                Compiler.ExceptionListener.Throw(
                            $"Cannot override function [{func.Name}] because it is sealed.");
            Base = func;
        }
        public void SetSealed(bool flag)
        {
            Sealed = flag;
        }
        public string[] GetInvokeProperties()
        {
            if (invokeProperties == null)
                return new string[] { };
            else
                return invokeProperties;
        }
        public void SetProperties(string name, string[] args, bool invoking, bool isSealed, bool obsolete, string[] alias, bool anon)
        {
            Name = name;
            ExpectedArgs = args;
            Invoking = invoking;
            Sealed = isSealed;
            Obsolete = obsolete;
            Alias = alias;
            IsAnonymous = anon;
        }
        //sets the return flag and the return value to bubble up.
        //if this function is anonymously named, then it calls this method
        //in the parent function as well all the way to the top named function
        //i dont like the method name but at least its descriptive
        public void ReturnToTopOfBubble(Token value)
        {
            ReturnBubble = value;
            ReturnFlag = true;
            if (Caller != null && Caller.CallingFunction != null && Caller.CallingFunction.IsLoop)
            {
                var tracer = Tracer;
                if (tracer != null)
                    tracer.SetBreak(true);
            }
            if (Caller != null)
            {
                if (IsAnonymous || Invoking)
                    Caller.CallingFunction.ReturnToTopOfBubble(value);
            }
        }
        private int GetUID()
        {
            _uidIndex++;
            return _uidIndex;
        }
        public AnonymousFunction()
        {
            ProvidedArgs = new TokenStack();
            LocalVariables = new TokenStack();
            UID = GetUID();
        }
        //standard constructor
        public AnonymousFunction(string value) : this()
        {
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
                var func = new AnonymousFunction(a.ToString(), true, _base);
                func.Base = Base;
                FunctionStack.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            ParseDirectives(value);
        }
        //this constructor is when function is anonomysly named
        public AnonymousFunction(string value, bool anon, IBaseFunction callerBase) : this()
        {
            Base = callerBase;
            IsAnonymous = true;
            //get top level anonymous functions before everything else
            value = value.Substring(1);
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new AnonymousFunction(a.ToString(), true, _base);
                FunctionStack.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            Value = value;
            Name = "AnonymousFunction"+Compiler.AnonymousFunctionIndex;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            ParseDirectives(value);
            //Name = value.Split('.')[1].Split('(')[0];
        }
        //this is the constructor used when function is an override
        public AnonymousFunction(string value, List<IBaseFunction> predefined) : this()
        {
            Override = true;
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
                var func = new AnonymousFunction(a.ToString(), true, _base);
                FunctionStack.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            ParseDirectives(value);
        }
        //the construction for custom extensions
        public AnonymousFunction(string value, CustomExtension parent) : this()
        {
            Name = value.Split('.')[1].Split('(')[0];
            
            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Compiler.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new AnonymousFunction(a.ToString(), true, _base);
                func.Base = Base;
                FunctionStack.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            var type = ExpectedArgs.ElementAtOrDefault(0);
            if (type != null && type == "this variable")
            {
                parent.SetProperties(Name, new string[] { }, false, false, true, new string[] { });
            }
            else if(type != null && type == "this function")
            {
                parent.SetProperties(Name, new string[] { }, false, false, false, new string[] { });
            }
            else
            {
                Compiler.ExceptionListener.Throw("[287]Custom extension must have input parameter of `this variable` or `this function`");
            }
            //replace this from the parameter, so it can be called by just `function` or `variable` at runtime
            //the `this` is only there for definition, to direct the extension parser to look for this extension
            //in the variable lot or not.
            ExpectedArgs[0] = ExpectedArgs[0].Replace("this ", "");
            ParseDirectives(value);
        }
        private void ParseDirectives(string value)
        {
            if (value.Contains(".") && value.Contains("{"))
            {
                var findPretext = value.Split('.')[1].Split('{')[0];
                if (findPretext.Contains(":"))
                {
                    var dirs = findPretext.Split(':');
                    if (dirs.Length > 0)
                    {
                        var temp = new Dictionary<string, string>();
                        for (var i = 1; i < dirs.Length; i++)
                        {
                            var preParen = dirs[i].Split('(');
                            if (preParen[0].Contains("Where"))
                            {
                                var inParen = preParen[1].Split(')')[0];
                                var postParen = preParen[1].Split('=')[1];
                                temp[inParen] = postParen;
                            }
                        }
                        Directives = temp;
                    }
                }
            }
        }
        public virtual void TryParse(TFunction caller)
        {
            ReturnBubble = null;
            ReturnFlag = false;
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
                ProvidedArgs = new TokenStack();
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
            //clear local var stack after use
            LocalVariables = new TokenStack();
        }
        //this overload is when the function is called with the for extension
        public virtual void TryParse(TFunction caller, bool forFlag)
        {
            ReturnBubble = null;
            ReturnFlag = false;
            if (caller != null)
            {
                BlindExecute = caller.BlindExecute;
                Tracer = caller.Tracer;
                Caller = caller;
            }
            //combine expected args and given args and add them to variabel pool
            if (caller != null && caller.Arguments != null && ExpectedArgs != null && ExpectedArgs.Length > 0)
            {
                ProvidedArgs = new TokenStack();
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
            //clear local var stack after use
            LocalVariables = new TokenStack();
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
            this.IsLoop = true;
            string[] forNumber = findFor.Extend();
            int forNumberAsNumber = int.Parse(forNumber[0]);
            var tracer = new LoopTracer();
            Compiler.LoopTracerStack.Add(tracer);
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
                    }
                    caller.SetTracer(tracer);
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
