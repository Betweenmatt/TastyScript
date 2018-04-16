using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Function;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;
using TastyScript.ParserManager.ExceptionHandler;

namespace TastyScript.IFunction
{
    public class TryCatchEvent : ITryCatchEvent
    {
        //internal BaseFunction CatchBlock { get; }
        internal BaseFunction TryBlock { get; }

        internal TFunction CatchBlock { get; }

        public TryCatchEvent(BaseFunction tryblock, TFunction catchblock)
        {
            TryBlock = tryblock;
            CatchBlock = catchblock;
        }

        public void TriggerCatchEvent(ExceptionObject ex)
        {
            Manager.ExceptionHandler.TryCatchEventStack.RemoveLast();
            TryBlock.SetReturnFlag(true);
            string line = System.Text.RegularExpressions.Regex.Escape(ex.Line);
            string[] arg = new string[] { "[" + ex.Type.ToString().CleanString() + "," + ex.Message.CleanString() + "," + line.CleanString() + "," + ex.Snippet.CleanString() + "]" };
            //var tfunc = new TFunction(CatchBlock, TryBlock, arg);
            //tfunc.TryParse();
            CatchBlock.SetArguments(arg);
            CatchBlock.TryParse();
        }
    }
}