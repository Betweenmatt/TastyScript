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
        public static List<IBaseFunction> FunctionList;
        /// <summary>
        /// The default sleep timer for android commands in milliseconds
        /// </summary>
        public static double SleepDefaultTime;
        public static List<IBaseToken> GlobalVariables;
        //saving the halt function for later calling
        public static IBaseFunction HaltFunction;
        public static List<IExtension> Extensions = new List<IExtension>();
        public static bool Stop;

        public TokenParser(List<IBaseFunction> functionList)
        {
            FunctionList.AddRange(functionList);
            GlobalVariables = new List<IBaseToken>()
            {
            new TObject("DateTime",()=>{return DateTime.Now.ToString(); }, locked:true),
            new TObject("Date",()=>{return DateTime.Now.ToShortDateString(); }, locked:true),
            new TObject("Time",()=>{return DateTime.Now.ToShortTimeString(); }, locked:true),
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

            startScope.TryParse(null, null);
        }

    }
}
