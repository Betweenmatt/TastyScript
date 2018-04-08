using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    /// <summary>
    /// This is depricated for now. idk if i'll ever try to implement it
    /// </summary>
    [Function("StartAsync",new string[] { "invoke" },isSealed:true, depricated:true)]
    internal class FunctionStartAsync : FunctionDefinition
    {
        public override string CallBase()
        {
            var prov = ProvidedArgs.First("invoke");
            if (prov == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"{this.Name } Arguments cannot be null."));
                return null;
            }
            var func = FunctionStack.First(prov.ToString());
            if (func == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"Invoke function [{prov.ToString()}] could not be found"));
                return null;
            }
            if (!func.Async)
            {
                Compiler.ExceptionListener.Throw($"{this.Name} The invoked function must be marked async",
                    ExceptionType.SystemException);
                return null;
            }
            try
            {
                Thread th = new Thread(() =>
                {
                    func.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List, Caller.CallingFunction.ProvidedArgs.List);
                    func.TryParse(new TFunction(Caller.Function, new List<EDefinition>(), this.GetInvokeProperties(), Caller.CallingFunction));
                });
                th.Start();
            }catch
            {
                Compiler.ExceptionListener.Throw($"Async thread reached an unexpected error.", ExceptionType.SystemException);
                return null;
            }
            return "";
        }
    }
}
