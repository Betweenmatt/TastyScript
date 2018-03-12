using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.Lang;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Extensions;

namespace TastyScript
{
    internal static class Utilities
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
        //uses reflection to get all the IBaseFunction classes with the attribute [Function]
        public static List<IBaseFunction> GetPredefinedFunctions()
        {
            List<IBaseFunction> temp = new List<IBaseFunction>();
            string definedIn = typeof(Function).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                // Note that we have to call GetName().Name.  Just GetName() will not work.  The following
                // if statement never ran when I tried to compare the results of GetName().
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (System.Type type in assembly.GetTypes())
                        if (type.GetCustomAttributes(typeof(Function), true).Length > 0)
                        {
                            var func = System.Type.GetType(type.ToString());
                            var inst = Activator.CreateInstance(func) as IBaseFunction;
                            var attt = type.GetCustomAttribute(typeof(Function), true) as Function;
                            inst.SetProperties(attt.Name, attt.ExpectedArgs, attt.Invoking, attt.Sealed, attt.Obsolete, attt.Alias);
                            if (!attt.Depricated)
                                temp.Add(inst);
                        }
            return temp;
        }
        //i guess a quick way to essentially deep clone base functions on demand.
        //idk how much this will kill performance but i cant think of another way
        public static IBaseFunction CopyFunctionReference(string funcName)
        {
            IBaseFunction temp = null;
            string definedIn = typeof(Function).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (System.Type type in assembly.GetTypes())
                        if (type.GetCustomAttributes(typeof(Function), true).Length > 0)
                        {
                            var attt = type.GetCustomAttribute(typeof(Function), true) as Function;
                            if (attt.Name == funcName)
                            {
                                var func = System.Type.GetType(type.ToString());
                                var inst = Activator.CreateInstance(func) as IBaseFunction;
                                inst.SetProperties(attt.Name, attt.ExpectedArgs, attt.Invoking, attt.Sealed, attt.Obsolete, attt.Alias);
                                if (!attt.Depricated)
                                    temp = (inst);
                            }
                        }
            return temp;
        }
        public static void GetExtensions()
        {
            List<EDefinition> temp = new List<EDefinition>();
            string definedIn = typeof(Extension).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                // Note that we have to call GetName().Name.  Just GetName() will not work.  The following
                // if statement never ran when I tried to compare the results of GetName().
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (System.Type type in assembly.GetTypes())
                        if (type.GetCustomAttributes(typeof(Extension), true).Length > 0)
                        {
                            var func = System.Type.GetType(type.ToString());
                            var inst = Activator.CreateInstance(func) as EDefinition;
                            var attt = type.GetCustomAttribute(typeof(Extension), true) as Extension;
                            inst.SetProperties(attt.Name, attt.ExpectedArgs, attt.Invoking, attt.Obsolete, attt.VariableExtension, attt.Alias);
                            if (!attt.Depricated)
                                temp.Add(inst);
                        }
            ExtensionStack.Clear();
            ExtensionStack.AddRange(temp);
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
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                    $"Could not find path: {path}"));
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
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                    $"Could not find path: {path}"));
            }
            return file;
        }
    }
}
