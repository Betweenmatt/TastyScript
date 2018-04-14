using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;
using TastyScript.ParserManager.Looping;

namespace TastyScript.IFunction.Function
{
    public abstract class BaseFunction
    {
        public string[] Alias { get; private set; }
        protected BaseFunction _base;
        public BaseFunction Base
        {
            get
            {
                //if (IsAnonymous)
                //    return Caller.CallingFunction.Base;
                return _base;
            }
            protected set
            {
                _base = value;
            }
        }
        //public TFunctionOld Caller { get; protected set; }
        public TFunction Caller { get; protected set; }
        public Dictionary<string, string> Directives { get; protected set; }
        public string[] ExpectedArgs { get; protected set; }
        public ExtensionList _extensions;
        public ExtensionList Extensions
        {
            get
            {
                if (_extensions == null)
                    _extensions = new ExtensionList();
                return _extensions;
            }
            set
            {
                if (_extensions == null)
                    _extensions = new ExtensionList();
                _extensions = value;
            }
        }
        private int _generatedTokensIndex = -1;
        public int GeneratedTokensIndex
        {
            get
            {
                _generatedTokensIndex++;
                return _generatedTokensIndex;
            }
        }
        
        [Obsolete("Still trying to figure out why i implemented this")]
        protected string[] InvokeProperties;
        public bool IsAnonymous { get; protected set; }
        public bool IsBlindExecute { get; protected set; }
        /// <summary>
        /// Flag is defined on gui based functions, to separate gui behavior from normal script behavior 
        /// </summary>
        public bool IsGui { get; protected set; }
        public bool IsInvoking { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsLoop { get; protected set; }
        public bool IsObsolete { get; private set; }
        public bool IsOverride { get; protected set; }
        public bool IsSealed { get; private set; }
        public TokenList LocalVariables { get; protected set; }
        public string Name { get; protected set; }
        public TokenList ProvidedArgs { get; protected set; }
        public Token ReturnBubble { get; set; }
        public bool ReturnFlag { get; set; }
        public LoopTracer Tracer { get; protected set; }
        public int UID { get; private set; }
        private static int _uidIndex = -1;
        public string Value { get; protected set; }

        protected abstract void TryParse();
        protected abstract void TryParse(bool forFlag);

        public void TryParse(CallerInheritObject inherit)
        {
            InheritCaller(inherit);
            TryParse();
        }
         /// <summary>
         /// <param name="forFlag"></param> is an arbitrary bool used only to trigger this specific overload which omits the for loop check.
         /// You should be using the overload with no parameters!
         /// </summary>
         /// <param name="forFlag"></param>
        public void TryParse(CallerInheritObject inherit, bool forFlag)
        {
            InheritCaller(inherit);
            TryParse(forFlag);
        }
        /// <summary>
        /// Assign ProvidedArgs values based on the expected args and the given args
        /// </summary>
        protected void AssignParameters()
        {
            //combine expected args and given args and add them to variabel pool
            if (Caller.Arguments != null && ExpectedArgs != null && ExpectedArgs.Length > 0)
            {
                ProvidedArgs = new TokenList();
                var args = Caller.Arguments;
                if (ExpectedArgs.Length > 0)
                {
                    for (var i = 0; i < ExpectedArgs.Length; i++)
                    {
                        var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                        if (args.ElementAtOrDefault(i) == null)
                            ProvidedArgs.Add(new Token(exp, "null", ""));
                        else
                            ProvidedArgs.Add(new Token(exp, args[i],""));
                    }
                }
            }
        }
        public void SetBlindExecute(bool flag)
        {
            IsBlindExecute = flag;
        }
        protected void GetUID()
        {
            _uidIndex++;
            UID = _uidIndex;
        }
        //sets the return flag and the return value to bubble up.
        //if this function is anonymously named, then it calls this method
        //in the parent function as well all the way to the top named function
        //i dont like the method name but at least its descriptive
        public void ReturnToTopOfBubble(Token value)
        {
            ReturnBubble = value;
            ReturnFlag = true;
            if (Caller.IsParentLoop())
            {
                var tracer = Tracer;
                if (tracer != null)
                    tracer.SetBreak(true);
            }
            if (IsAnonymous || IsInvoking)
                Caller.SetParentReturnToTopOfBubble(value);
        }
        public string[] GetInvokeProperties()
        {
            return InvokeProperties;
        }
        public void SetBase(BaseFunction func)
        {
            if (func.IsSealed)
                Manager.Throw($"Cannot override function [{func.Name}] because it is sealed.");
            Base = func;
        }
        /// <summary>
        /// Inherit the required information from caller like the loop tracer and blind execute.
        /// Also inherits variables on invoked functions
        /// </summary>
        /// <param name="caller"></param>
        protected void InheritCaller(CallerInheritObject inherit)
        {
            ReturnBubble = null;
            ReturnFlag = false;
            if (inherit != null)
            {
                InvokeProperties = inherit.InvokeProperties;
                //IsBlindExecute = caller.BlindExecute;
                Tracer = inherit.Tracer;
                Caller = inherit.Caller;
                Extensions = inherit.Extensions;
                IsBlindExecute = Caller.IsParentBlindExecute();
                if (Caller.IsParentInvoking())
                {
                    Console.WriteLine("SetInvokeProperties[new]");
                    List<Token> vars = Caller.GetParentOfParentLocalVariables()?.List ?? new List<Token>();
                    List<Token> prov = Caller.GetParentOfParentLocalArguments()?.List ?? new List<Token>();
                    LocalVariables = new TokenList();
                    vars.RemoveAll(r => r.Name == "");
                    prov.RemoveAll(r => r.Name == "");
                    LocalVariables.AddRange(vars);
                    LocalVariables.AddRange(prov);
                }
            }
        }
        [Obsolete]
        public void SetInvokeProperties(string[] args, List<Token> vars, List<Token> oldargs)
        {/*
            Console.WriteLine("SetInvokeProperties[old]");
            
            invokeProperties = args;
            LocalVariables = new TokenList();
            vars.RemoveAll(r => r.Name == "");
            oldargs.RemoveAll(r => r.Name == "");
            LocalVariables.AddRange(vars);
            LocalVariables.AddRange(oldargs);//add the parameters from the calling function to this functions local var stack
         */   
        }
        public void SetProperties(string name, string[] args, bool invoking, bool isSealed, bool obsolete, string[] alias, bool anon)
        {
            Name = name;
            ExpectedArgs = args;
            IsInvoking = invoking;
            IsSealed = isSealed;
            IsObsolete = obsolete;
            Alias = alias;
            IsAnonymous = anon;
        }
        public void SetSealed(bool flag)
        {
            IsSealed = flag;
        }

        
        protected virtual void ForExtension(BaseExtension findFor)
        {
            this.IsLoop = true;
            string[] forNumber = findFor.Extend();
            int forNumberAsNumber = int.Parse(forNumber[0]);
            var tracer = new LoopTracer();
            Manager.LoopTracerStack.Add(tracer);
            if (forNumberAsNumber <= 0)
                forNumberAsNumber = int.MaxValue;
            for (var x = 0; x < forNumberAsNumber; x++)
            {
                if (!Manager.IsScriptStopping)
                {
                    if (tracer.Break)
                    {
                        break;
                    }
                    if (tracer.Continue)
                    {
                        tracer.SetContinue(false);//reset continue
                    }
                    Caller.SetTracer(tracer);
                    Caller.TryParse(true);
                }
                else
                {
                    break;
                }
            }
            Manager.LoopTracerStack.Remove(tracer);
            tracer = null;
        }
    }
}
