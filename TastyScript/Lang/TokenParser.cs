using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Func;
using TastyScript.Lang.Token;

namespace TastyScript.Lang
{
    public class TokenParser
    {
        public static List<IBaseFunction> FunctionList = new List<IBaseFunction>();
        /// <summary>
        /// The default sleep timer for android commands in milliseconds
        /// </summary>
        public static double SleepDefaultTime;
        public static List<IBaseToken> GlobalVariables = new List<IBaseToken>()
        {
            new TString("DateTime",()=>{return DateTime.Now.ToString(); }),
            new TString("Date",()=>{return DateTime.Now.ToShortDateString(); }),
            new TString("Time",()=>{return DateTime.Now.ToShortTimeString(); })
        };
        //saving the halt function for later calling
        public static IBaseFunction HaltFunction;
        public static List<IExtension> Extensions = new List<IExtension>();
        public static bool Stop;

        public TokenParser(List<IBaseFunction> functionList)
        {
            FunctionList = functionList;
            StartParse();
        }

        private void StartParse()
        {
            //start function is an inherit for easier override implementation down the line
            var startScope = FunctionList.FirstOrDefault(f => f.Name == "Start");
            var awakeScope = FunctionList.FirstOrDefault(f => f.Name == "Awake");
            HaltFunction = FunctionList.FirstOrDefault(f => f.Name == "Halt");

            if (awakeScope != null)
            {
                foreach (var x in FunctionList)
                {
                    if (x.Name == "Awake")
                    {
                        x.TryParse(null);
                    }
                }
            }
            if (startScope == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Your script is missing a 'Start' function."));
            startScope.TryParse(null);
        }

    }
}
