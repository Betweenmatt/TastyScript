using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.Lang.Exceptions;

namespace TastyScript.Lang
{
    public static class Utilities
    {
        /// <summary>
        /// Sleeps the main thread until time is reached, or Token.Stop is true
        /// </summary>
        /// <param name="ms"></param>
        public static void Sleep(int ms)
        {
            try
            {
                CancellationToken token = TokenParser.CancellationTokenSource.Token;
                ManualResetEventSlim mre = new ManualResetEventSlim();
                mre.Wait(ms, token);
                mre.Dispose();
            }catch(Exception e)
            {
                if (!(e is OperationCanceledException))
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, "Unknown error with `Sleep`"));
            }
        }
    }
}
