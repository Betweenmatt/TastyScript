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
            new Token("null","null","{0}", locked:true),
            new Token("True","True","{0}",locked:true),
            new Token("true","true","{0}",locked:true),
            new Token("False","False","{0}",locked:true), 
            new Token("false","false","{0}",locked:true)
            });
            StartParse();
        }

        private void StartParse()
        {
            //start function is an inherit for easier override implementation down the line
            new Directives();
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
        
    }
}
