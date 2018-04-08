using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Function;
using TastyScript.ParserManager;

namespace TastyScript.TastyScript
{
    internal class InternalUtilities
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
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, "Unknown error with `Sleep`"));
            }
        }

        //i guess a quick way to essentially deep clone base functions on demand.
        //idk how much this will kill performance but i cant think of another way
        public static BaseFunction CopyFunctionReference(string funcName)
        {
            BaseFunction temp = null;
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
                                var inst = Activator.CreateInstance(func) as BaseFunction;
                                inst.SetProperties(attt.Name, attt.ExpectedArgs, attt.Invoking, attt.Sealed, attt.Obsolete, attt.Alias, attt.IsAnonymous);
                                if (!attt.Depricated)
                                    temp = (inst);
                            }
                        }
            return temp;
        }
        public static BaseExtension CopyExtensionReference(string funcName)
        {
            BaseExtension temp = null;
            string definedIn = typeof(Extension).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (System.Type type in assembly.GetTypes())
                        if (type.GetCustomAttributes(typeof(Extension), true).Length > 0)
                        {
                            var attt = type.GetCustomAttribute(typeof(Extension), true) as Extension;
                            if (attt.Name == funcName)
                            {
                                var func = System.Type.GetType(type.ToString());
                                var inst = Activator.CreateInstance(func) as BaseExtension;
                                inst.SetProperties(attt.Name, attt.ExpectedArgs, attt.Invoking, attt.Sealed, attt.Obsolete, attt.Alias);
                                if (!attt.Depricated)
                                    temp = (inst);
                            }
                        }
            return temp;
        }
    }
}
