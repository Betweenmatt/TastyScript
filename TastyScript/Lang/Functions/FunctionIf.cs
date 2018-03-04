using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("If", new string[] { "bool" }, isSealed: true)]
    internal class FunctionIf : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var prov = ProvidedArgs.FirstOrDefault(f => f.Name == "bool");
            if (prov == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"Arguments cannot be null.", LineValue));
            bool flag = (prov.ToString() == "True") ? true : false;
            var andFlag = Extensions.FirstOrDefault(f => f.Name == "And");
            var orFlag = Extensions.FirstOrDefault(f => f.Name == "Or");

            if (orFlag != null)
            {
                var orExtensions = Extensions.Where(w => w.Name == "Or");
                foreach (var o in orExtensions)
                {
                    var or = o as ExtensionOr;
                    TParameter param = or.Extend();
                    bool paramFlag = (param.Value.Value[0].ToString() == "True") ? true : false;
                    if (paramFlag)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (andFlag != null)
            {
                var andExtensions = Extensions.Where(w => w.Name == "And");
                foreach (var a in andExtensions)
                {
                    var and = a as ExtensionAnd;
                    TParameter param = and.Extend();
                    //Console.WriteLine(param.Value.Value[0].ToString());
                    bool paramFlag = (param.Value.Value[0].ToString() == "True") ? true : false;
                    if (!paramFlag)
                    {
                        flag = false;
                        break;
                    }
                }
            }

            if (flag)
            {
                //find then extension and call it
                var findThen = Extensions.FirstOrDefault(f => f.Name == "Then") as ExtensionThen;
                if (findThen != null)
                {
                    TParameter thenFunc = findThen.Extend();
                    var func = TokenParser.FunctionList.FirstOrDefault(f => f.Name == thenFunc.Value.Value[0].ToString());
                    if (func == null)
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                            $"Cannot find the invoked function.", LineValue));
                    //pass in invoke properties. shouldnt break with null
                    func.TryParse(findThen.GetInvokeProperties(), this);
                }
                else
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                        $"[460]If function must have a Then Extension", LineValue));
                }
            }
            else
            {
                //find else extension and call it
                var findElse = Extensions.FirstOrDefault(f => f.Name == "Else") as ExtensionElse;
                if (findElse != null)
                {
                    TParameter elseFunc = findElse.Extend();
                    var func = TokenParser.FunctionList.FirstOrDefault(f => f.Name == elseFunc.Value.Value[0].ToString());
                    if (func == null)
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                            $"Cannot find the invoked function.", LineValue));
                    func.TryParse(findElse.GetInvokeProperties(), this);
                }
            }
            return args;
        }
    }
}
