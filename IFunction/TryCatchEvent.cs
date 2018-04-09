using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Function;
using TastyScript.ParserManager;
using TastyScript.ParserManager.ExceptionHandler;

namespace TastyScript.IFunction
{
    public class TryCatchEvent : ITryCatchEvent
    {
        internal BaseFunction TryBlock { get; }
        internal BaseFunction CatchBlock { get; }
        internal TryCatchEvent(BaseFunction tryblock, BaseFunction catchblock)
        {
            TryBlock = tryblock;
            CatchBlock = catchblock;
        }
        public void TriggerCatchEvent(ExceptionObject ex)
        {
            Manager.ExceptionHandler.TryCatchEventStack.RemoveLast();
            TryBlock.ReturnFlag = true;
            string line = System.Text.RegularExpressions.Regex.Escape(ex.Line);
            string[] arg = new string[] { "[" + ex.Type.ToString().CleanString() + "," + ex.Message.CleanString() + "," + line.CleanString() + "," + ex.Snippet.CleanString() + "]" };
            CatchBlock.TryParse(new Tokens.TFunction(CatchBlock, null, arg, TryBlock));
        }
    }
}
