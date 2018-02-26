using System;
using System.Threading;
using TastyScript;

namespace TastyScript
{
    public class IO
    {
        public class Output
        {
            public static void Print(object o, bool line = true)
            {
                if (line)
                    Console.WriteLine(o);
                else
                    Console.Write(o);
            }
            public static void Print(object o, ConsoleColor color, bool line = true)
            {
                Console.ForegroundColor = color;
                if (line)
                    Console.WriteLine(o);
                else
                    Console.Write(o);
                Console.ResetColor();
            }
        }
        public class Input
        {
            public static string ReadLine()
            {
                return Console.ReadLine();
            }
        }
    }

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
        public static string ReadLine(CancellationToken token,int timeOutMillisecs = Timeout.Infinite)
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
        public static void Poke()
        {
            gotInput.Set();
        }
    }
    static class WaitExtensions
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
