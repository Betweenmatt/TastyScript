using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang
{
    internal class TokenParser
    {
        /// <summary>
        /// The default sleep timer for android commands in milliseconds
        /// </summary>
        public static double SleepDefaultTime;
        public static TokenStack GlobalVariables;
        public static TokenStack AnonymousTokens;
        private static int _anonymousTokensIndex = -1;
        public static int AnonymousTokensIndex
        {
            get
            {
                _anonymousTokensIndex++;
                return _anonymousTokensIndex;
            }
        }
        //saving the halt function for later calling
        public static IBaseFunction HaltFunction { get; private set; }
        public static IBaseFunction GuaranteedHaltFunction { get; private set; }
        
        private static bool _stop;
        public static bool Stop {
            get
            {
                return _stop;
            }
            set
            {
                if (value && CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested)
                    CancellationTokenSource.Cancel();
                _stop = value;
            }
        }
        public static CancellationTokenSource CancellationTokenSource { get; private set; }

        private string[] StrVersion()
        {
            var vers = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var spl = vers.Split('.');
            return spl;
        }
        public TokenParser(List<IBaseFunction> functionList)
        {
            CancellationTokenSource = new CancellationTokenSource();
            FunctionStack.AddRange(functionList);
            AnonymousTokens = new TokenStack();
            GlobalVariables = new TokenStack();
            GlobalVariables.AddRange(new List<Token>()
            {
            new Token("DateTime",()=>{return DateTime.Now.ToString(); }, "{0}", locked:true),
            new Token("Date",()=>{return DateTime.Now.ToShortDateString(); },"{0}", locked:true),
            new Token("Time",()=>{return DateTime.Now.ToShortTimeString(); },"{0}", locked:true),
            new TArray("GetVersion", StrVersion(),"{0}", locked:true),
            new Token("null","null","{0}", locked:true)
            });
            StartParse();
        }

        private void StartParse()
        {
            //start function is an inherit for easier override implementation down the line
            ApplyDirectives();
            var startScope = FunctionStack.First("Start");
            var awakeScope = FunctionStack.First("Awake");
            HaltFunction = FunctionStack.First("Halt");
            GuaranteedHaltFunction = FunctionStack.First("GuaranteedHalt");

            if (awakeScope != null)
            {
                var awakecollection = FunctionStack.Where("Awake");
                foreach (var x in awakecollection)
                {
                    x.TryParse(null);
                }
            }
            if (startScope == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Your script is missing a 'Start' function."));
            var startCollection = FunctionStack.Where("Start");
            if(startCollection.Count() != 2)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                    $"There must only be one `Start` function. Please remove {startCollection.Count() - 2} `Start` functions"));
            //remove the start override from the stack
            var startIndex = FunctionStack.IndexOf(startScope);
            FunctionStack.RemoveAt(startIndex);

            startScope.TryParse(null);
        }
        private void ApplyDirectives()
        {
            var copylist = FunctionStack.List;
            for (var i = 0; i < copylist.Count; i++) 
                if (copylist[i].Directives != null)
                    foreach (var d in copylist[i].Directives)
                        switch (d.Key)
                        {
                            case ("Layer"):
                                DirectiveLayer(d.Value, FunctionStack.List[i]);
                                break;
                            case ("Sealed"):
                                DirectiveSealed(d.Value, FunctionStack.List[i]);
                                break;
                            case ("Omit"):
                                DirectiveOmit(d.Value, FunctionStack.List[i]);
                                break;
                            default:
                                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler($"Unknown directive [{d.Key}]"));
                                break;
                        }
        }
        private void DirectiveOmit(string val, IBaseFunction func)
        {
            val = val.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
            bool value = (val == "true" || val == "True") ? true : false;
            if (value)
            {
                FunctionStack.Remove(func);
            }
        }
        private void DirectiveSealed(string val, IBaseFunction func)
        {
            val = val.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
            bool value = (val == "true" || val == "True") ? true : false;
            if (value)
            {
                func.SetSealed(value);
                var samelist = FunctionStack.Where(func.Name).ToList();
                var thisIndex = samelist.IndexOf(func);
                if(thisIndex != 0)
                {
                    Compiler.ExceptionListener.Throw(
                            $"Cannot override function [{func.Name}] because of the Sealed directive.");
                }
            }
        }
        private void DirectiveLayer(string val, IBaseFunction func)
        {
            int index = 0;
            bool tryint = int.TryParse(val, out index);
            if (!tryint)
                Compiler.ExceptionListener.Throw($"The directive [Layer] must have a whole number as its value.");
            if (index - 1 > FunctionStack.List.Count)
                index = FunctionStack.List.Count - 1;
            FunctionStack.MoveTo(func, index);
            //apply any override to base connections needed since the stack was shuffled
            var samelist = FunctionStack.Where(func.Name).ToList();
            for(var i = 0; i < samelist.Count; i++)
            {
                var obj = samelist[i];
                if (!obj.Override)
                    continue;
                if (i == samelist.Count - 1)
                    continue;
                IBaseFunction next = null;
                for(var nexti = i; nexti <= samelist.Count - i; nexti++)
                {
                    if (samelist.ElementAtOrDefault(nexti) == null)
                        continue;
                    else
                    {
                        next = samelist[nexti];
                        break;
                    }
                }
                if (next == null)
                    Compiler.ExceptionListener.Throw($"Unknown error applying directive [Layer] on function [{func.Name}]");
                obj.SetBase(next);
            }
        }
    }
}
