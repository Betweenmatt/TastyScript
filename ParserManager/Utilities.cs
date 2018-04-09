using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TastyScript.ParserManager
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
                CancellationToken token = Manager.CancellationTokenSource.Token;
                ManualResetEventSlim mre = new ManualResetEventSlim();
                mre.Wait(ms, token);
                mre.Dispose();
            }
            catch (Exception e)
            {
                if (!(e is OperationCanceledException))
                    Manager.Throw("Unknown error with `Sleep`");
            }
        }
        public static string ScopeRegex(string input)
        {
            return @"(" + input + @"([^{}]*){)([^{}]+|(?<Level>\{)| (?<-Level>\}))+(?(Level)(?!))\}";
        }

        /// <summary>
        /// Checks both absolute and relative, as well as pre-set directories
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileFromPath(string path)
        {
            var file = "";
            //check if its a full path
            if (File.Exists(path))
                file = System.IO.File.ReadAllText(path);
            //check if the path is local to the app directory
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + path))
                file = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + path);
            //check for quick directory
            else if (File.Exists(Settings.QuickDirectory + "/" + path))
                file = System.IO.File.ReadAllText(Settings.QuickDirectory + "/" + path);
            //check for quick directory
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/" + Settings.QuickDirectory + "/" + path))
                file = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/" + Settings.QuickDirectory + "/" + path);
            //or fail
            else
            {
                Manager.Throw($"Could not find path: {path}");
            }
            return file;
        }
        public static string GetPathFromShortPath(string path)
        {
            string file = "";
            //check if its a full path
            if (File.Exists(path))
                file = path;
            //check if the path is local to the app directory
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + path))
                file = AppDomain.CurrentDomain.BaseDirectory + path;
            //check for quick directory
            else if (File.Exists(Settings.QuickDirectory + "/" + path))
                file = Settings.QuickDirectory + "/" + path;
            //check for quick directory
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/" + Settings.QuickDirectory + "/" + path))
                file = AppDomain.CurrentDomain.BaseDirectory + "/" + Settings.QuickDirectory + "/" + path;
            //or fail
            else
            {
                Manager.Throw($"Could not find path: {path}");
            }
            return file;
        }
        public static Bitmap GetImageFromPath(string path)
        {
            Bitmap file = null;
            if (File.Exists(path))
                file = (Bitmap)Bitmap.FromFile(path);
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + path))
                file = (Bitmap)Bitmap.FromFile(AppDomain.CurrentDomain.BaseDirectory + path);
            else if (File.Exists(Settings.QuickDirectory + "/" + path))
                file = (Bitmap)Bitmap.FromFile(Settings.QuickDirectory + "/" + path);
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/" + Settings.QuickDirectory + "/" + path))
                file = (Bitmap)Bitmap.FromFile(AppDomain.CurrentDomain.BaseDirectory + "/" + Settings.QuickDirectory + "/" + path);
            else
            {
                Manager.Throw($"Could not find path: {path}");
            }
            return file;
        }
    }
}
