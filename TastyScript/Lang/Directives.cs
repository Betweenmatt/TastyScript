using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang
{
    /*
     * Directives are a sort of function meta-data i've been working on. It accepts constant values like strings, numbers and bools
     * and is presented after the function parameters, and before the left brace of the function. Each directive must begin with a 
     * colon and a Where() function like so 
     * `function.Example() : Where(Sealed) = true {`
     * To add a new directive, just add it to the switch clause in the constructor.
     */
    internal class Directives
    {
        public Directives()
        {
            var copylist = FunctionStack.List;
            for (var i = 0; i < copylist.Count; i++)
                if (copylist[i].Directives != null)
                    foreach (var d in copylist[i].Directives)
                        switch (d.Key)
                        {
                            case ("Layer"):
                                DirectiveLayer(d.Value, FunctionStack.List[i]);
                                break;
                            case ("Sealed"):
                                DirectiveSealed(d.Value, FunctionStack.List[i]);
                                break;
                            case ("Omit"):
                                DirectiveOmit(d.Value, FunctionStack.List[i]);
                                break;
                            default:
                                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler($"Unknown directive [{d.Key}]"));
                                break;
                        }
        }
        private void DirectiveOmit(string val, IBaseFunction func)
        {
            val = val.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
            bool value = (val == "true" || val == "True") ? true : false;
            if (value)
            {
                FunctionStack.Remove(func);
            }
        }
        private void DirectiveSealed(string val, IBaseFunction func)
        {
            val = val.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
            bool value = (val == "true" || val == "True") ? true : false;
            if (value)
            {
                func.SetSealed(value);
                FunctionStack.MoveTo(func, 0);
                var samelist = FunctionStack.Where(func.Name).ToList();
                foreach(var x in samelist)
                {
                    if(x != func && x.Directives != null && x.Directives.ContainsKey("Sealed"))
                    {
                        var stripws = x.Directives["Sealed"].Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                        if (stripws == "true" || stripws == "True")
                            Compiler.ExceptionListener.Throw($"The function [{func.Name}] has already been sealed by a directive. Please remove a Sealed directive.");
                    }
                }
                func.SetBase(samelist[samelist.Count - 1]);
            }
        }
        private void DirectiveLayer(string val, IBaseFunction func)
        {
            int index = 0;
            bool tryint = int.TryParse(val, out index);
            if (!tryint)
                Compiler.ExceptionListener.Throw($"The directive [Layer] must have a whole number as its value.");
            if (index - 1 > FunctionStack.List.Count)
                index = FunctionStack.List.Count - 1;
            FunctionStack.MoveTo(func, index);
            //apply any override to base connections needed since the stack was shuffled
            var samelist = FunctionStack.Where(func.Name).ToList();
            for (var i = 0; i < samelist.Count; i++)
            {
                var obj = samelist[i];
                if (!obj.Override)
                    continue;
                if (i == samelist.Count - 1)
                    continue;
                IBaseFunction next = null;
                for (var nexti = i; nexti <= samelist.Count - i; nexti++)
                {
                    if (samelist.ElementAtOrDefault(nexti) == null)
                        continue;
                    else
                    {
                        next = samelist[nexti];
                        break;
                    }
                }
                if (next == null)
                    Compiler.ExceptionListener.Throw($"Unknown error applying directive [Layer] on function [{func.Name}]");
                obj.SetBase(next);
            }
        }
    }
}
