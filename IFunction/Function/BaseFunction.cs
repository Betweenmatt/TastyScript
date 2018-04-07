using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;

namespace TastyScript.IFunction.Function
{
    public abstract class BaseFunction
    {
        public bool IsAnonymous { get; private set; }
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
            private set
            {
                _base = value;
            }
        }
        public TFunction Caller { get; protected set; }
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
        public bool IsBlindExecute { get; private set; }
        public bool IsInvoking { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsLoop { get; private set; }
        public bool IsOverride { get; private set; }
        public bool IsSealed { get; private set; }
        public TokenList LocalVariables { get; set; }
        public string Name { get; private set; }
        public TokenList ProvidedArgs { get; private set; }
        public Token ReturnBubble { get; private set; }
        public bool ReturnFlag { get; private set; }
        public LoopTracer Tracer { get; private set; }
        public int UID { get; }
        public string Value { get; private set; }
    }
}
