using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("Timer", new string[] { "type" }, isSealed: true)]
    public class FunctionTimer : FunctionDefinition
    {
        private static Stopwatch _watch;
        public static void TimerStop()
        {
            if (_watch != null)
                _watch.Stop();
            _watch = null;
        }
        public override bool CallBase()
        {
            //find extensions
            var tryStart = Extensions.First("Start");
            var tryPrint = Extensions.First("Print");
            var tryColor = Extensions.First("Color");
            var tryStop = Extensions.First("Stop");
            if (tryStart != null)
            {
                _watch = Stopwatch.StartNew();
            }
            if (tryPrint != null)
            {
                if (_watch != null)
                {
                    var elapsedMs = _watch.ElapsedMilliseconds;
                    var color = ConsoleColor.Gray;
                    if (tryColor != null)
                    {
                        var colorParam = tryColor.Extend();
                        ConsoleColor newcol = ConsoleColor.Gray;
                        var nofail = Enum.TryParse<ConsoleColor>(colorParam[0].ToString(), out newcol);
                        if (nofail)
                            color = newcol;
                    }
                    Manager.Print(elapsedMs, color, false);
                }
            }
            if (tryStop != null)
            {
                if (_watch != null)
                    _watch.Stop();
            }
            if(_watch != null)
            {
                long output = _watch.ElapsedMilliseconds;
                var trytype = ProvidedArgs.First("type");
                if(trytype != null)
                {
                    if (trytype.ToString() == "ticks")
                        output = _watch.ElapsedTicks;   
                }

                ReturnBubble = new Token("timems", output.ToString(), Caller.Line);
            }
            return true;
        }
    }
}
