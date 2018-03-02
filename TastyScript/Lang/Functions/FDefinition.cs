using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    internal static class FunctionHelpers
    {
        public static void Sleep(double ms)
        {
            var func = new TFunction("Sleep", TokenParser.FunctionList.FirstOrDefault(f => f.Name == "Sleep"));
            var newArgs = new TParameter("sleep", new List<IBaseToken>() { new TObject("sleep", ms) });
            func.Value.Value.TryParse(newArgs, null);
        }
        public static void AndroidTouch(int x, int y)
        {
            Program.AndroidDriver.Tap(x, y);
        }
        [Obsolete]
        public static void AndroidBack()
        {
            Program.AndroidDriver.KeyEvent(Android.AndroidKeyCode.Back);
        }
        [Obsolete]
        public static void AndroidCheckScreen(string succPath, IBaseFunction succFunc, IBaseFunction failFunc, int thresh = 90)
        {
            try
            {
                AnalyzeScreen ascreen = new AnalyzeScreen();
                ascreen.Analyze(succPath,
                    () => { succFunc.TryParse(null, null); },
                    () => { failFunc.TryParse(null, null); },
                    thresh
                );
            }
            catch (Exception e)
            {
                if (e is System.IO.FileNotFoundException)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                        $"File could not be found: {succPath}"));
                }
                Console.WriteLine(e);
                Compiler.ExceptionListener.Throw(new ExceptionHandler(""));
            }
        }
        [Obsolete]
        public static void AndroidCheckScreen(string succPath, string failPath, IBaseFunction succFunc, IBaseFunction failFunc, int thresh = 90)
        {
            try
            {
                AnalyzeScreen ascreen = new AnalyzeScreen();
                ascreen.Analyze(succPath, failPath,
                    () => { succFunc.TryParse(null, null); },
                    () => { failFunc.TryParse(null, null); },
                    thresh
                );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Compiler.ExceptionListener.Throw(new ExceptionHandler(""));
            }
        }
    }
    internal class FDefinition<T> : AnonymousFunction<T>, IOverride<T>
    {
        public virtual T CallBase(TParameter args) { return default(T); }
        public override void TryParse(TParameter args, IBaseFunction caller, string lineval = "{0}")
        {
            if (caller != null)
            {
                BlindExecute = caller.BlindExecute;
                Tracer = caller.Tracer;
            }
            LineValue = lineval;
            var findFor = Extensions.FirstOrDefault(f => f.Name == "For") as ExtensionFor;
            if (findFor != null)
            {
                //if for extension exists, reroutes this tryparse method to the loop version without the for check
                ForExtension(args, findFor, lineval);
                return;
            }
            if (args != null)
            {
                var arg = args.Value.Value.FirstOrDefault(f => f.Name.Contains("[]")) as TObject;
                var argarr = args.Value.Value.FirstOrDefault(f => f.Name.Contains("[]")) as TParameter;
                if (argarr != null)//if arg array is multi element
                {
                    ProvidedArgs = new List<IBaseToken>();
                    for (var i = 0; i < argarr.Value.Value.Count; i++)
                    {
                        ProvidedArgs.Add(new TObject(ExpectedArgs[i], argarr.Value.Value[i]));
                    }
                }
                else if (arg != null)//if arg array is a single element 
                {
                    ProvidedArgs = new List<IBaseToken>();
                    ProvidedArgs.Add(new TObject(ExpectedArgs[0], arg.Value.Value));
                }
                else
                {
                    ProvidedArgs = new List<IBaseToken>();
                    for (var i = 0; i < args.Value.Value.Count; i++)
                    {
                        ProvidedArgs.Add(new TObject(ExpectedArgs[i], args.Value.Value[i]));
                    }
                }
            }
            Parse(args);
        }
        public override void TryParse(TParameter args, bool forFlag, IBaseFunction caller, string lineval = "{0}")
        {
            if (caller != null)
            {
                BlindExecute = caller.BlindExecute;
                Tracer = caller.Tracer;
            }
            LineValue = lineval;
            if (args != null)
            {
                var arg = args.Value.Value.FirstOrDefault(f => f.Name.Contains("[]")) as TObject;
                var argarr = args.Value.Value.FirstOrDefault(f => f.Name.Contains("[]")) as TParameter;
                if (argarr != null)//if arg array is multi element
                {
                    ProvidedArgs = new List<IBaseToken>();
                    for (var i = 0; i < argarr.Value.Value.Count; i++)
                    {
                        ProvidedArgs.Add(new TObject(ExpectedArgs[i], argarr.Value.Value[i]));
                    }
                }
                else if (arg != null)//if arg array is a single element 
                {
                    ProvidedArgs = new List<IBaseToken>();
                    ProvidedArgs.Add(new TObject(ExpectedArgs[0], arg.Value.Value));
                }
                else
                {
                    ProvidedArgs = new List<IBaseToken>();
                    for (var i = 0; i < args.Value.Value.Count; i++)
                    {
                        ProvidedArgs.Add(new TObject(ExpectedArgs[i], args.Value.Value[i]));
                    }
                }
            }
            Parse(args);
        }

        public override T Parse(TParameter args)
        {
            if (!TokenParser.Stop)
            {
                if (Tracer == null || (!Tracer.Continue && !Tracer.Break))
                    return CallBase(args);

            }
            else if (TokenParser.Stop && BlindExecute)
            {
                return CallBase(args);
            }

            return default(T);

        }
        [Obsolete("this feels too redundant", true)]
        public virtual void TryCallBase(TParameter args)
        {
        }
    }
}
