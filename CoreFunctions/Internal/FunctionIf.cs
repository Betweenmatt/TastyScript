using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Tokens;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("If", new string[] { "bool" }, isSealed: true, alias:new string[] { "if" }, isanon: false, invoking:true)]
    public class FunctionIf : FunctionDefinition
    {
        public override bool CallBase()
        {
            var prov = ProvidedArgs.First("bool");
            if (prov == null)
            {
                Manager.Throw($"Arguments cannot be null.");
                return false;
            }
            bool flag = (prov.ToString() == "True" || prov.ToString() == "true") ? true : false;
            var andFlag = Extensions.First("And");
            var orFlag = Extensions.First("Or");

            if (orFlag != null)
            {
                var orExtensions = Extensions.Where("Or");
                foreach (var or in orExtensions)
                {
                    string[] param = or.Extend();
                    bool paramFlag = (param[0].ToString() == "True" || param[0].ToString() == "true") ? true : false;
                    if (paramFlag)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (andFlag != null)
            {
                var andExtensions = Extensions.Where("And");
                foreach (var and in andExtensions)
                {
                    string[] param = and.Extend();
                    bool paramFlag = (param[0].ToString() == "True" || param[0].ToString() == "true") ? true : false;
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
                var findThen = Extensions.First("Then");
                if (findThen != null)
                {
                    string[] thenFunc = findThen.Extend();
                    var func = FunctionStack.First(thenFunc[0].ToString());
                    if (func == null)
                    {
                        Manager.Throw($"Cannot find the invoked function.");
                        return false;
                    }
                    new TFunction(func, this, findThen.GetInvokeProperties()).TryParse();
                }
                else
                {
                    Manager.Throw($"[460]If function must have a Then Extension");
                    return false;
                }
            }
            else
            {
                //find else extension and call it
                var findElse = Extensions.First("Else");
                if (findElse != null)
                {
                    string[] elseFunc = findElse.Extend();
                    var func = FunctionStack.First(elseFunc[0].ToString());
                    if (func == null)
                    {
                        Manager.Throw($"Cannot find the invoked function.");
                        return false;
                    }
                    new TFunction(func, this, findElse.GetInvokeProperties()).TryParse();
                }
            }
            return true;
        }
    }
}
