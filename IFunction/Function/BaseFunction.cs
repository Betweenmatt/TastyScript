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
        private BaseFunction _base;
        public BaseFunction Base
        {
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

        [Obsolete]//idk where this is being used
        protected string[] invokeProperties;
        public bool IsAnonymous { get; protected set; }
        public bool IsBlindExecute { get; protected set; }
        /// <summary>
        /// Flag is defined on gui based functions, to separate gui behavior from normal script behavior 
        /// </summary>
        public bool IsGui { get; protected set; }
        public bool IsInvoking { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsLoop { get; private set; }
        public bool IsObsolete { get; private set; }
        public bool IsOverride { get; protected set; }
        public bool IsSealed { get; private set; }
        public TokenList LocalVariables { get; protected set; }
        public string Name { get; protected set; }
        public TokenList ProvidedArgs { get; protected set; }
        public Token ReturnBubble { get; private set; }
        public bool ReturnFlag { get; private set; }
        public LoopTracer Tracer { get; protected set; }
        public int UID { get; private set; }
        private static int _uidIndex = -1;
        public string Value { get; protected set; }

        public abstract void TryParse(TFunction caller);
        public abstract void TryParse(TFunction caller, bool forFlag);
        public void SetBlindExecute(bool flag)
        {
            IsBlindExecute = flag;
        }
        [Obsolete]//idk where this is being used
        public string[] GetInvokeProperties()
        {
            if (invokeProperties == null)
                return new string[] { };
            else
                return invokeProperties;
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
            if (Caller != null && Caller.CallingFunction != null && Caller.CallingFunction.IsLoop)
            {
                var tracer = Tracer;
                if (tracer != null)
                    tracer.SetBreak(true);
            }
            if (Caller != null)
            {
                if (IsAnonymous || IsInvoking)
                    Caller.CallingFunction.ReturnToTopOfBubble(value);
            }
        }
        public void SetBase(BaseFunction func)
        {
            if (func.IsSealed)
                Manager.Throw($"Cannot override function [{func.Name}] because it is sealed.");
            Base = func;
        }
        public void SetInvokeProperties(string[] args, List<Token> vars, List<Token> oldargs)
        {
            invokeProperties = args;
            LocalVariables = new TokenList();
            vars.RemoveAll(r => r.Name == "");
            oldargs.RemoveAll(r => r.Name == "");
            LocalVariables.AddRange(vars);
            LocalVariables.AddRange(oldargs);//add the parameters from the calling function to this functions local var stack
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
        protected void ResetReturn()
        {
            ReturnBubble = null;
            ReturnFlag = false;
        }
        public void SetSealed(bool flag)
        {
            IsSealed = flag;
        }

        
        protected virtual void ForExtension(TFunction caller, BaseExtension findFor)
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
                if ((IsGui && !Manager.IsGUIScriptStopping) || (!IsGui && !Manager.IsScriptStopping))
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
            Manager.LoopTracerStack.Remove(tracer);
            tracer = null;
        }
    }
}
