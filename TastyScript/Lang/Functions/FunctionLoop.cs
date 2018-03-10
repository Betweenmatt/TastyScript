using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Loop", new string[] { "invoke" }, isSealed: true, invoking: true)]
    internal class FunctionLoop : FDefinition
    {
        public override string CallBase()
        {
            var prov = ProvidedArgs.First("invoke");
            if (prov == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"[247]Invoke function cannot be null.", LineValue));
            var func = FunctionStack.First(prov.ToString());
            if (func == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"[250]Invoke function cannot be null.", LineValue));
            }
            var findFor = Extensions.FirstOrDefault(f => f.Name == "For") as ExtensionFor;
            if (findFor != null)
            {
                string[] forNumber = findFor.Extend();
                int forNumberAsNumber = int.Parse(forNumber[0].ToString());
                if (forNumberAsNumber == 0)
                    forNumberAsNumber = int.MaxValue;
                LoopTracer tracer = new LoopTracer();
                Compiler.LoopTracerStack.Add(tracer);
                Tracer = tracer;
                for (var x = 0; x <= forNumberAsNumber; x++)
                {
                    //gave a string as the parameter because number was causing srs problems
                    if (!TokenParser.Stop)
                    {
                        if (tracer.Break)
                        {
                            break;
                        }
                        if (tracer.Continue)
                        {
                            tracer.SetContinue(false);//reset continue
                            continue;
                        }
                        var passed = this.GetInvokeProperties();
                        if (passed != null)
                        {
                            var getFirstElement = passed.ElementAtOrDefault(0);
                            if (getFirstElement != null)
                            {
                                passed[0] = x.ToString();
                            }
                            else
                            {
                                passed = new string[] { x.ToString() };
                            }
                        }
                        else
                        {
                            passed = new string[] { x.ToString() };
                        }
                        func.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List);
                        func.TryParse(new TFunction(Caller.Function, new List<EDefinition>(), passed, Caller.CallingFunction));
                    }
                    else
                    {
                        break;
                    }
                }
                Compiler.LoopTracerStack.Remove(tracer);
                tracer = null;
            }
            else
            {
                LoopTracer tracer = new LoopTracer();
                Compiler.LoopTracerStack.Add(tracer);
                Tracer = tracer;
                var x = 0;
                while (true)
                {
                    //gave a string as the parameter because number was causing srs problems
                    if (!TokenParser.Stop)
                    {
                        if (tracer.Break)
                        {
                            break;
                        }
                        if (tracer.Continue)
                        {
                            tracer.SetContinue(false);//reset continue
                            continue;
                        }
                        var passed = this.GetInvokeProperties();
                        if (passed != null)
                        {
                            var getFirstElement = passed.ElementAtOrDefault(0);
                            if (getFirstElement != null)
                            {
                                passed[0] = x.ToString();
                            }
                            else
                            {
                                passed = new string[] { x.ToString() };
                            }
                        }
                        else
                        {
                            passed = new string[] { x.ToString() };
                        }
                        func.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List);
                        func.TryParse(new TFunction(Caller.Function, new List<EDefinition>(), passed, Caller.CallingFunction));
                        x++;
                    }
                    else
                    {
                        break;
                    }
                }
                Compiler.LoopTracerStack.Remove(tracer);
                tracer = null;
            }
            return "";
        }
        //stop the base for looping extension from overriding this custom looping function
        protected override void ForExtension(TFunction caller, ExtensionFor findFor)
        {
            TryParse(caller, true);
        }
    }
}
