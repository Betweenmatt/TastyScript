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
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Func
{
    public static class FunctionHelpers
    {
        public static void Sleep(double ms)
        {
            var func = new TFunction("Sleep", TokenParser.FunctionList.FirstOrDefault(f => f.Name == "Sleep"));
            var newArgs = new TParameter("sleep", new List<IBaseToken>() { new TObject("sleep", ms) });
            func.Value.Value.TryParse(newArgs);
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
        public static void AndroidCheckScreen(string succPath, IBaseFunction succFunc, IBaseFunction failFunc, int thresh = 90)
        {
            try
            {
                AnalyzeScreen ascreen = new AnalyzeScreen();
                ascreen.Analyze(succPath,
                    () => { succFunc.TryParse(null); },
                    () => { failFunc.TryParse(null); },
                    thresh
                );
            }catch(Exception e)
            {
                if(e is System.IO.FileNotFoundException)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                        $"File could not be found: {succPath}"));
                }
                Console.WriteLine(e);
                Compiler.ExceptionListener.Throw(new ExceptionHandler(""));
            }
        }
        public static void AndroidCheckScreen(string succPath, string failPath, IBaseFunction succFunc, IBaseFunction failFunc, int thresh = 90)
        {
            try
            {
                AnalyzeScreen ascreen = new AnalyzeScreen();
                ascreen.Analyze(succPath, failPath,
                    () => { succFunc.TryParse(null); },
                    () => { failFunc.TryParse(null); },
                    thresh
                );
            }catch(Exception e)
            {
                Console.WriteLine(e);
                Compiler.ExceptionListener.Throw(new ExceptionHandler(""));
            }
}
    }
    public class FunctionDefinitions<T> : AnonymousFunction<T>, IOverride<T>
    {
        public virtual IFunction<T> CallBase(TParameter args) { return default(IFunction<T>); }
        public override void TryParse(TParameter args, string lineval = "{0}")
        {
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
        public override void TryParse(TParameter args, bool forFlag, string lineval = "{0}")
        {
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
        [Obsolete("this feels too redundant",true)]
        public virtual void TryCallBase(TParameter args)
        {
        }
    }

    [Function("AppPackage", new string[] { "app" }, isSealed: true)]
    public class FunctionAppPackage : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var print = "";
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "app");
            if (argsList != null)
                print = argsList.ToString();
            if (Program.AndroidDriver == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                    $"Cannot set the app package without having a device connected. Please connect to a device first.", LineValue));
            Program.AndroidDriver.SetAppPackage(print);
            return args;
        }
    }
    [Function("TakeScreenshot", new string[] { "path" }, isSealed: true)]
    public class FunctionTakeScreenshot : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var path = ProvidedArgs.FirstOrDefault(f => f.Name == "path");
            if (path == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, $"Path must be specified", LineValue));
                return null;
            }
            var ss = Program.AndroidDriver.GetScreenshot();
            ss.Save(path.ToString(), ImageFormat.Png);
            return args;
        }
    }
    [Function("CheckScreen", new string[] { "succFunc", "failFunc", "succPath", "failPath" }, isSealed: true)]
    public class FunctionCheckScreen : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var succFunc = ProvidedArgs.FirstOrDefault(f => f.Name == "succFunc");
            var failFunc = ProvidedArgs.FirstOrDefault(f => f.Name == "failFunc");
            var succPath = ProvidedArgs.FirstOrDefault(f => f.Name == "succPath");
            var failPath = ProvidedArgs.FirstOrDefault(f => f.Name == "failPath");
            if (succFunc == null || failFunc == null || succPath == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, $"Invoke function cannot be null.", LineValue));
            }
            var sf = TokenParser.FunctionList.FirstOrDefault(f => f.Name == succFunc.ToString());
            var ff = TokenParser.FunctionList.FirstOrDefault(f => f.Name == failFunc.ToString());
            if (sf == null || ff == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"[198]Invoke function cannot be found.", LineValue));
            }
            //check for threshold extension
            var threshExt = Extensions.FirstOrDefault(f => f.Name == "Threshold") as ExtensionThreshold;
            int thresh = 90;
            if(threshExt != null)
            {
                var param = threshExt.Extend();
                var nofail = int.TryParse(param.Value.Value[0].ToString(), out thresh);
            }
            if (failPath != null)
            {
                FunctionHelpers.AndroidCheckScreen(succPath.ToString(), failPath.ToString(), sf, ff,thresh);
            }
            else
            {
                FunctionHelpers.AndroidCheckScreen(succPath.ToString(), sf, ff,thresh);
            }

            return args;
        }
    }
    [Function("Timer",isSealed:true)]
    public class FunctionTimer : FunctionDefinitions<object>
    {
        private static Stopwatch _watch;
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
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
                if(_watch != null)
                    _watch.Stop();
            }
            return args;
        }
    }
    [Function("ConnectDevice", new string[] { "serial" }, isSealed: true)]
    public class FunctionConnectDevice : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var print = "";
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "serial");
            if (argsList != null)
                print = argsList.ToString();
            Program.AndroidDriver = new Android.Driver(print);

            return args;
        }
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
    [Function("Loop", new string[] { "invoke" }, isSealed: true, invoking:true)]
    public class FunctionLoop : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var prov = ProvidedArgs.FirstOrDefault(f => f.Name == "invoke");
            if (prov == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"[247]Invoke function cannot be null.", LineValue));
            var func = TokenParser.FunctionList.FirstOrDefault(f => f.Name == prov.ToString());
            if (func == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"[250]Invoke function cannot be null.", LineValue));
            }
            var findFor = Extensions.FirstOrDefault(f => f.Name == "For") as ExtensionFor;
            if (findFor != null)
            {
                TParameter forNumber = findFor.Extend();
                int forNumberAsNumber = int.Parse(forNumber.Value.Value[0].ToString());
                if (forNumberAsNumber == 0)
                    forNumberAsNumber = int.MaxValue;
                for (var x = 0; x <= forNumberAsNumber; x++)
                {
                    //gave a string as the parameter because number was causing srs problems
                    if (!TokenParser.Stop)
                    {
                        var passed = this.GetInvokeProperties();
                        if (passed != null)
                        {
                            var getFirstElement = passed.Value.Value.ElementAtOrDefault(0);
                            if (getFirstElement != null)
                            {
                                passed.Value.Value[0] = new TObject(passed.Value.Value[0].Name, x.ToString());
                            }
                        }
                        else
                        {
                            passed = new TParameter("Loop", new List<IBaseToken>() { new TObject("enumerator", x.ToString()) });
                        }
                        func.TryParse(passed);
                    }
                    else
                    {
                        break;
                    }
                }
            }else
            {
                var x = 0;
                while (true)
                {
                    //gave a string as the parameter because number was causing srs problems
                    if (!TokenParser.Stop)
                    {
                        var passed = this.GetInvokeProperties();
                        if (passed != null)
                        {
                            var getFirstElement = passed.Value.Value.ElementAtOrDefault(0);
                            if (getFirstElement != null)
                            {
                                passed.Value.Value[0] = new TObject(passed.Value.Value[0].Name, x.ToString());
                            }
                        }
                        else
                        {
                            passed = new TParameter("Loop", new List<IBaseToken>() { new TObject("enumerator", x.ToString()) });
                        }
                        func.TryParse(passed);
                        x++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return args;
        }
        //stop the base for looping extension from overriding this custom looping function
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            TryParse(args, false, lineval);
        }
    }
    [Function("SetDefaultSleep", new string[] { "sleep" })]
    public class FunctionSetDefaultSleep : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var sleep = (ProvidedArgs.FirstOrDefault(f => f.Name == "sleep") as TObject);
            double sleepnum = double.Parse(sleep.ToString());
            TokenParser.SleepDefaultTime = sleepnum;
            return args;
        }
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
    [Function("If", new string[] { "bool" }, isSealed: true)]
    public class FunctionIf : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var prov = ProvidedArgs.FirstOrDefault(f => f.Name == "bool");
            if (prov == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"Arguments cannot be null.", LineValue));
            bool flag = (prov.ToString() == "True") ? true : false;
            var andFlag = Extensions.FirstOrDefault(f => f.Name == "And");
            var orFlag = Extensions.FirstOrDefault(f => f.Name == "Or");
            
            if (orFlag != null)
            {
                var orExtensions = Extensions.Where(w => w.Name == "Or");
                foreach (var o in orExtensions)
                {
                    var or = o as ExtensionOr;
                    TParameter param = or.Extend();
                    bool paramFlag = (param.Value.Value[0].ToString() == "True") ? true : false;
                    if (paramFlag)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (andFlag != null)
            {
                var andExtensions = Extensions.Where(w => w.Name == "And");
                foreach (var a in andExtensions)
                {
                    var and = a as ExtensionAnd;
                    TParameter param = and.Extend();
                    //Console.WriteLine(param.Value.Value[0].ToString());
                    bool paramFlag = (param.Value.Value[0].ToString() == "True") ? true : false;
                    if (!paramFlag)
                    {
                        flag = false;
                        break;
                    }
                }
            }

            if (flag)
            {
                //find then extension and call it
                var findThen = Extensions.FirstOrDefault(f => f.Name == "Then") as ExtensionThen;
                if (findThen != null)
                {
                    TParameter thenFunc = findThen.Extend();
                    var func = TokenParser.FunctionList.FirstOrDefault(f => f.Name == thenFunc.Value.Value[0].ToString());
                    if (func == null)
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                            $"Cannot find the invoked function.", LineValue));
                    //pass in invoke properties. shouldnt break with null
                    func.TryParse(findThen.GetInvokeProperties());
                }else
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, 
                        $"[460]If function must have a Then Extension", LineValue));
                }
            }
            else
            {
                //find else extension and call it
                var findElse = Extensions.FirstOrDefault(f => f.Name == "Else") as ExtensionElse;
                if (findElse != null)
                {
                    TParameter elseFunc = findElse.Extend();
                    var func = TokenParser.FunctionList.FirstOrDefault(f => f.Name == elseFunc.Value.Value[0].ToString());
                    if (func == null)
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                            $"Cannot find the invoked function.", LineValue));
                    func.TryParse(findElse.GetInvokeProperties());
                }
            }
            return args;
        }
    }
    [Function("Start")]
    public class FunctionStart : FunctionDefinitions<object>
    {
        public override void TryParse(TParameter args, string lineval = "{0}")
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"{this.Name} can only be called by internal functions.", LineValue));
      
        }
        public new object CallBase(TParameter args)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"{this.Name} can only be called by internal functions.", LineValue));
            return null;
        }
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
    [Function("Halt")]
    public class FunctionHalt : FunctionDefinitions<object>
    {
        public override void TryParse(TParameter args, string lineval = "{0}")
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"{this.Name} can only be called by internal functions.", LineValue));
        }
        public new object CallBase(TParameter args)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"{this.Name} can only be called by internal functions.", LineValue));
            return null;
        }
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
    [Function("Awake", FunctionObsolete: true)]
    public class FunctionAwake : FunctionDefinitions<object>
    {
        public override void TryParse(TParameter args, string lineval = "{0}")
        {
        }
        public new object CallBase(TParameter args)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"{this.Name} can only be called by internal functions.", LineValue));
            return null;
        }
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
    [Function("Base", isSealed: true)]
    public class FunctionBase : FunctionDefinitions<object>
    {
        public override void TryParse(TParameter args, string lineval = "{0}")
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"{this.Name} can not be overrided", LineValue));
        }
        public new object CallBase(TParameter args)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"{this.Name} can not be overrided.", LineValue));
            return null;
        }
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
    [Function("Sleep", new string[] { "time" })]
    public class FunctionSleep : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var time = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "time") as TObject).Value.Value.ToString());
            Thread.Sleep((int)Math.Ceiling(time));
            return args;
        }
    }
    [Function("Touch", new string[] { "intX", "intY", "sleep" })]
    public class FunctionTouch : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var x = (ProvidedArgs.FirstOrDefault(f => f.Name == "intX") as TObject);
            var y = (ProvidedArgs.FirstOrDefault(f => f.Name == "intY") as TObject);
            if (x == null || y == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException,
                    $"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments", LineValue));
            }
            double intX = double.Parse(x.Value.Value.ToString());
            double intY = double.Parse(y.Value.Value.ToString());
            if (Program.AndroidDriver == null)
                IO.Output.Print($"[DRIVERLESS] Touch x:{intX} y:{intY}");
            else
                FunctionHelpers.AndroidTouch((int)intX, (int)intY);
            double sleep = TokenParser.SleepDefaultTime;
            if (args.Value.Value.Count > 2)
            {
                sleep = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "sleep") as TObject).Value.Value.ToString());
            }
            FunctionHelpers.Sleep(sleep);
            return args;
        }
    }
    [Function("LongTouch", new string[] { "intX", "intY", "duration", "sleep" })]
    public class FunctionLongTouch : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var x = (ProvidedArgs.FirstOrDefault(f => f.Name == "intX") as TObject);
            var y = (ProvidedArgs.FirstOrDefault(f => f.Name == "intY") as TObject);
            var dur = (ProvidedArgs.FirstOrDefault(f => f.Name == "duration") as TObject);
            if(x == null || y == null || dur == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException,
                    $"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments", LineValue));
            }
            double intX = double.Parse(x.Value.Value.ToString());
            double intY = double.Parse(y.Value.Value.ToString());
            double duration = double.Parse(dur.Value.Value.ToString());
            if (Program.AndroidDriver == null)
                IO.Output.Print($"[DRIVERLESS] LongTouch x:{intX} y:{intY} duration:{duration}");
            else
                Program.AndroidDriver.LongPress((int)intX, (int)intY, (int)duration);
            double sleep = TokenParser.SleepDefaultTime;
            if (args.Value.Value.Count > 3)
            {
                sleep = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "sleep") as TObject).Value.Value.ToString());
            }
            FunctionHelpers.Sleep(sleep);
            return args;
        }
    }
    [Function("Swipe", new string[] { "intX1", "intY1", "intX2", "intY2", "duration", "sleep" })]
    public class FunctionSwipe : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var x1 = (ProvidedArgs.FirstOrDefault(f => f.Name == "intX1") as TObject);
            var y1 = (ProvidedArgs.FirstOrDefault(f => f.Name == "intY1") as TObject);
            var x2 = (ProvidedArgs.FirstOrDefault(f => f.Name == "intX2") as TObject);
            var y2 = (ProvidedArgs.FirstOrDefault(f => f.Name == "intY2") as TObject);
            var dur = (ProvidedArgs.FirstOrDefault(f => f.Name == "duration") as TObject);
            if (x1 == null || y1 == null || x2 == null || y2 == null || dur == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException,
                    $"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments", LineValue));
            }
            double intX1 = double.Parse(x1.Value.Value.ToString());
            double intY1 = double.Parse(y1.Value.Value.ToString());
            double intX2 = double.Parse(x2.Value.Value.ToString());
            double intY2 = double.Parse(y2.Value.Value.ToString());
            double duration = double.Parse(dur.Value.Value.ToString());
            if (Program.AndroidDriver == null)
                IO.Output.Print($"[DRIVERLESS] LongTouch x1:{intX1} y1:{intY1} x2:{intX2} y2:{intY2} duration:{duration}");
            else
                Program.AndroidDriver.Swipe((int)intX1, (int)intY1, (int)intX2, (int)intY2, (int)duration);
            double sleep = TokenParser.SleepDefaultTime;
            if (args.Value.Value.Count > 5)
            {
                sleep = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "sleep") as TObject).Value.Value.ToString());
            }
            FunctionHelpers.Sleep(sleep);
            return args;
        }
    }
    [Function("KeyEvent",new string[] { "keyevent" })]
    public class FunctionKeyEvent : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "keyevent");
            if (argsList == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, "Arguments cannot be null.", LineValue));

            if (Program.AndroidDriver != null)
            {
                //FunctionHelpers.AndroidBack();
                AndroidKeyCode newcol = AndroidKeyCode.A;
                var nofail = Enum.TryParse<AndroidKeyCode>(argsList.ToString(), out newcol);
                if (!nofail)
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                        $"The Key Event {argsList.ToString()} could not be found.", LineValue));
                Program.AndroidDriver.KeyEvent(newcol);
            }
            else
            {
                IO.Output.Print($"[DRIVERLESS] Keyevent {argsList.ToString()}");
            }
            return args;
        }
    }
    [Function("SendText", new string[] { "s" })]
    public class FunctionSendText : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "s");
            if (argsList == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, "Arguments cannot be null.", LineValue));

            if (Program.AndroidDriver != null)
            {
                Program.AndroidDriver.SendText(argsList.ToString());
            }
            else
            {
                IO.Output.Print($"[DRIVERLESS] text {argsList.ToString()}");
            }
            return args;
        }
    }
    [Function("Back")]
    public class FunctionBack : FunctionDefinitions<object>
    {
        public override object Parse(TParameter args)
        {
            return CallBase(args);
        }
        public new object CallBase(TParameter args)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                $"The function 'Back()' is depricated. Please use the command 'KeyEvent()' instead. Check out the documentation to learn more.",LineValue));
            /*if (Program.AndroidDriver != null)
            {
                FunctionHelpers.AndroidBack();
            }
            else
            {
                IO.Output.Print($"[DRIVERLESS] Back Button Keyevent");
            }
            */
            return args;
        }
    }
    [Function("PrintLine", new string[] { "s" })]
    public class FunctionPrintLine : FunctionDefinitions<string>
    {
        public override string Parse(TParameter args)
        {
            Concat();
            return CallBase(args);
        }
        private List<string> concatStrings = new List<string>();
        private void Concat()
        {
            var findConcat = Extensions.FirstOrDefault(f => f.Name == "Concat") as ExtensionConcat;
            if (findConcat != null)
            {
                var concatList = Extensions.Where(f => f.Name == "Concat");
                foreach (var x in concatList)
                {
                    var param = x as ExtensionConcat;
                    TParameter ext = param.Extend();
                    concatStrings.Add(ext.Value.Value[0].ToString());
                }
            }
        }
        public new string CallBase(TParameter args)
        {
            var print = "";
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "s");
            if (argsList != null)
                print = argsList.ToString();
            //color extension check
            var findColorExt = Extensions.FirstOrDefault(f => f.Name == "Color") as ExtensionColor;
            var color = ConsoleColor.Gray;
            if(findColorExt != null)
            {
                var param = findColorExt.Extend();
                ConsoleColor newcol = ConsoleColor.Gray;
                var nofail = Enum.TryParse<ConsoleColor>(param.Value.Value[0].ToString(), out newcol);
                if (nofail)
                    color = newcol;
            }
            //try to escape, and if escaping fails fallback on the original string
            string output = print + String.Join("", concatStrings);
            try
            {
                output = System.Text.RegularExpressions.Regex.Unescape(print + String.Join("", concatStrings));
            }
            catch (Exception e)
            {
                if (!(e is ArgumentException) || !(e is ArgumentNullException))
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Unexpected input: {output}", LineValue));
            }
            IO.Output.Print(output,color);

            //clear extensions after done
            concatStrings = new List<string>();
            Extensions = new List<IExtension>();
            return print;
        }
    }
    [Function("Print", new string[] { "s" })]
    public class FunctionPrint : FunctionDefinitions<string>
    {
        public override string Parse(TParameter args)
        {
            Concat();
            return CallBase(args);
        }
        private List<string> concatStrings = new List<string>();
        private void Concat()
        {
            var findConcat = Extensions.FirstOrDefault(f => f.Name == "Concat") as ExtensionConcat;
            if (findConcat != null)
            {
                var concatList = Extensions.Where(f => f.Name == "Concat");
                foreach (var x in concatList)
                {
                    var param = x as ExtensionConcat;
                    TParameter ext = param.Extend();
                    concatStrings.Add(ext.Value.Value[0].ToString());
                }
            }
        }
        public new string CallBase(TParameter args)
        {
            var print = "";
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "s");
            if (argsList != null)
                print = argsList.ToString();
            //color extension check
            var color = ConsoleColor.Gray;
            var findColorExt = Extensions.FirstOrDefault(f => f.Name == "Color") as ExtensionColor;
            if (findColorExt != null)
            {
                var param = findColorExt.Extend();
                ConsoleColor newcol = ConsoleColor.Gray;
                var nofail = Enum.TryParse<ConsoleColor>(param.Value.Value[0].ToString(), out newcol);
                if (nofail)
                    color = newcol;
            }
            //try to escape, and if escaping fails fallback on the original string
            string output = print + String.Join("", concatStrings);
            try
            {
                output = System.Text.RegularExpressions.Regex.Unescape(print + String.Join("", concatStrings));
            }
            catch (Exception e)
            {
                if (!(e is ArgumentException) || !(e is ArgumentNullException))
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Unexpected input: {output}", LineValue));
            }
            IO.Output.Print(output, color,false);


            //clear extensions after done
            concatStrings = new List<string>();
            Extensions = new List<IExtension>();
            return print;
        }
    }
}
