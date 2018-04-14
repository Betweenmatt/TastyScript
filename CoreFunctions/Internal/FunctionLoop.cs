using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;
using TastyScript.ParserManager.Looping;

namespace TastyScript.CoreFunctions
{
    [Function("Loop", new string[] { "invoke" }, isSealed: true, invoking: true, isanon: false, alias:new string[] { "loop" })]
    public class FunctionLoop : FunctionDefinition
    {
        public override bool CallBase()
        {
            this.IsLoop = true;
            var prov = ProvidedArgs.First("invoke");
            if (prov == null)
            {
                Manager.Throw($"[247]Invoke function cannot be null.");
                return false;
            }
            var func = FunctionStack.First(prov.ToString());
            if (func == null)
            {
                Manager.Throw($"[250]Invoke function cannot be null.");
                return false;
            }
            var findFor = Extensions.First("For");
            if (findFor != null && findFor.Extend() != null && findFor.Extend().ElementAtOrDefault(0) != null && findFor.Extend()[0] != "")
            {
                string[] forNumber = findFor.Extend();
                int forNumberAsNumber = int.Parse(forNumber[0].ToString());
                //if (forNumberAsNumber == 0)
                //    forNumberAsNumber = int.MaxValue;
                var tracer = new LoopTracer();
                Manager.LoopTracerStack.Add(tracer);
                for (var x = 0; x <= forNumberAsNumber; x++)
                {
                    //gave a string as the parameter because number was causing srs problems
                    if (!Manager.IsScriptStopping)
                    {
                        if (tracer.Break)
                        {
                            break;
                        }
                        if (tracer.Continue)
                        {
                            tracer.SetContinue(false);//reset continue
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
                        func.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List, Caller.CallingFunction.ProvidedArgs.List);
                        func.TryParse(new TFunctionOld(Caller.Function, new ExtensionList(), passed, this, tracer));
                    }
                    else
                    {
                        break;
                    }
                }
                Manager.LoopTracerStack.Remove(tracer);
                tracer = null;
            }
            else
            {
                //LoopTracer tracer = new LoopTracer();
                //Compiler.LoopTracerStack.Add(tracer);
                //Tracer = tracer;
                var tracer = new LoopTracer();
                Manager.LoopTracerStack.Add(tracer);
                var x = 0;
                while (true)
                {
                    //gave a string as the parameter because number was causing srs problems
                    if (!Manager.IsScriptStopping)
                    {
                        if (tracer.Break)
                        {
                            break;
                        }
                        if (tracer.Continue)
                        {
                            tracer.SetContinue(false);//reset continue
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
                        func.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List, Caller.CallingFunction.ProvidedArgs.List);
                        func.TryParse(new TFunctionOld(Caller.Function, new ExtensionList(), passed, this, tracer));
                        x++;
                    }
                    else
                    {
                        break;
                    }
                }
                Manager.LoopTracerStack.Remove(tracer);;
                tracer = null;
            }
            return true;
        }
        //stop the base for looping extension from overriding this custom looping function
        protected override void ForExtension(TFunctionOld caller, BaseExtension findFor)
        {
            TryParse(caller, true);
        }
    }
}
