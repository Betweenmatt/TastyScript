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
        public static List<IBaseFunction> FunctionList;
        /// <summary>
        /// The default sleep timer for android commands in milliseconds
        /// </summary>
        public static double SleepDefaultTime;
        public static List<IBaseToken> GlobalVariables;
        public static List<Token> AnonymousTokens;
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
        public static List<IExtension> Extensions = new List<IExtension>();
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

        public TokenParser(List<IBaseFunction> functionList)
        {
            CancellationTokenSource = new CancellationTokenSource();
            FunctionList.AddRange(functionList);
            GlobalVariables = new List<IBaseToken>()
            {
            new TObject("DateTime",()=>{return DateTime.Now.ToString(); }, locked:true),
            new TObject("Date",()=>{return DateTime.Now.ToShortDateString(); }, locked:true),
            new TObject("Time",()=>{return DateTime.Now.ToShortTimeString(); }, locked:true),
            new TObject("GetVersion",()=> {return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }, locked:true),
            new TObject("null","null", locked:true)
            };
            StartParse();
        }

        private void StartParse()
        {
            //start function is an inherit for easier override implementation down the line
            var startScope = FunctionList.FirstOrDefault(f => f.Name == "Start");
            var awakeScope = FunctionList.FirstOrDefault(f => f.Name == "Awake");
            HaltFunction = FunctionList.FirstOrDefault(f => f.Name == "Halt");
            GuaranteedHaltFunction = FunctionList.FirstOrDefault(f => f.Name == "GuaranteedHalt");

            if (awakeScope != null)
            {
                foreach (var x in FunctionList)
                {
                    if (x.Name == "Awake")
                    {
                        x.TryParse(null,null);
                    }
                }
            }
            if (startScope == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Your script is missing a 'Start' function."));
            var startCollection = FunctionList.Where(w => w.Name == "Start");
            if(startCollection.Count() != 2)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"There can only be one `Start` function. Please remove {startCollection.Count() - 2} `Start` functions"));
            //remove the start override from the stack
            var startIndex = FunctionList.IndexOf(startScope);
            FunctionList.RemoveAt(startIndex);

            //foreach (var x in FunctionList)
            //    Console.WriteLine(x.Name);
            //Stop = true;

            startScope.TryParse(null, null);
        }

    }
}
