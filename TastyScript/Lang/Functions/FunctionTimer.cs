using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("Timer", isSealed: true)]
    internal class FunctionTimer : FDefinition<object>
    {
        private static Stopwatch _watch;
        public static void TimerStop()
        {
            if (_watch != null)
                _watch.Stop();
        }
        public override object CallBase(TParameter args)
        {
            //find extensions
            var tryStart = Extensions.FirstOrDefault(f => f.Name == "Start") as ExtensionStart;
            var tryPrint = Extensions.FirstOrDefault(f => f.Name == "Print") as ExtensionPrint;
            var tryColor = Extensions.FirstOrDefault(f => f.Name == "Color") as ExtensionColor;
            var tryStop = Extensions.FirstOrDefault(f => f.Name == "Stop") as ExtensionStop;
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
                        var nofail = Enum.TryParse<ConsoleColor>(colorParam.Value.Value[0].ToString(), out newcol);
                        if (nofail)
                            color = newcol;
                    }
                    IO.Output.Print(elapsedMs, color, false);
                }
            }
            if (tryStop != null)
            {
                if (_watch != null)
                    _watch.Stop();
            }
            return args;
        }
    }
}
