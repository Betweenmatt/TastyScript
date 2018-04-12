using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.IOStream
{
    /// <summary>
    /// Workaround for making Console.ReadLine cancelable
    /// </summary>
    public class Reader
    {
        private static Thread inputThread;
        private static AutoResetEvent getInput;
        private static AutoResetEvent gotInput;
        private static string input;

        static Reader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);

            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }

        // omit the parameter to read a line without a timeout
        public static string ReadLine(CancellationToken token, int timeOutMillisecs = Timeout.Infinite)
        {
            try
            {
                getInput.Set();
                //bool success = gotInput.WaitOne(timeOutMillisecs);
                bool success = gotInput.WaitOne(token);
                //bool suc = _cancelToken.WaitHandle.WaitOne(timeOutMillisecs);
                if (success)
                    return input;
                else
                    throw new TimeoutException("User did not provide input within the timelimit.");
            }
            catch (OperationCanceledException e)
            {
                Debug.WriteLine("[56]Operation cancelled");
            }
            catch(TimeoutException e)
            {
                Debug.WriteLine("[60]Timeout reached");
            }
            return string.Empty;
        }
        public static void Poke()
        {
            gotInput.Set();
        }
    }
    internal static class WaitExtensions
    {
        public static bool WaitOne(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            int n = WaitHandle.WaitAny(new[] { handle, cancellationToken.WaitHandle }, millisecondsTimeout);
            switch (n)
            {
                case WaitHandle.WaitTimeout:
                    return false;
                case 0:
                    return true;
                default:
                    cancellationToken.ThrowIfCancellationRequested();
                    return false; // never reached
            }
        }
        public static bool WaitOne(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return handle.WaitOne((int)timeout.TotalMilliseconds, cancellationToken);
        }

        public static bool WaitOne(this WaitHandle handle, CancellationToken cancellationToken)
        {
            return handle.WaitOne(Timeout.Infinite, cancellationToken);
        }
    }
}
