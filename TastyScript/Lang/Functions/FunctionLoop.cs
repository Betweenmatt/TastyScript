using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("Loop", new string[] { "invoke" }, isSealed: true, invoking: true)]
    internal class FunctionLoop : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var prov = ProvidedArgs.FirstOrDefault(f => f.Name == "invoke");
            if (prov == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"[247]Invoke function cannot be null.", LineValue));
            var func = TokenParser.FunctionList.FirstOrDefault(f => f.Name == prov.ToString());
            if (func == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"[250]Invoke function cannot be null.", LineValue));
            }
            var findFor = Extensions.FirstOrDefault(f => f.Name == "For") as ExtensionFor;
            if (findFor != null)
            {
                TParameter forNumber = findFor.Extend();
                int forNumberAsNumber = int.Parse(forNumber.Value.Value[0].ToString());
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
                            var getFirstElement = passed.Value.Value.ElementAtOrDefault(0);
                            if (getFirstElement != null)
                            {
                                passed.Value.Value[0] = new TObject(passed.Value.Value[0].Name, x.ToString());
                            }
                        }
                        else
                        {
                            passed = new TParameter("Loop", new List<IBaseToken>() { new TObject("enumerator", x.ToString()) });
                        }
                        func.TryParse(passed, this, LineValue);
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
                            var getFirstElement = passed.Value.Value.ElementAtOrDefault(0);
                            if (getFirstElement != null)
                            {
                                passed.Value.Value[0] = new TObject(passed.Value.Value[0].Name, x.ToString());
                            }
                        }
                        else
                        {
                            passed = new TParameter("Loop", new List<IBaseToken>() { new TObject("enumerator", x.ToString()) });
                        }
                        func.TryParse(passed, this, LineValue);
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
            return args;
        }
        //stop the base for looping extension from overriding this custom looping function
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            TryParse(args, false, this, lineval);
        }
    }
}
