using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Function;
using TastyScript.ParserManager;

namespace TastyScript.TastyScript
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
                            case ("MaxVer"):
                                DirectiveMaxVer(d.Value, FunctionStack.List[i]);
                                break;
                            case ("MinVer"):
                                DirectiveMinVer(d.Value, FunctionStack.List[i]);
                                break;
                            case ("Sealed"):
                                DirectiveSealed(d.Value, FunctionStack.List[i]);
                                break;
                            case ("IsBase"):
                                DirectiveIsBase(d.Value, FunctionStack.List[i]);
                                break;
                            case ("Omit"):
                                DirectiveOmit(d.Value, FunctionStack.List[i]);
                                break;
                            default:
                                Manager.ThrowSilent($"Unknown directive [{d.Key}]");
            break;
        }
    }
    private void DirectiveMinVer(string val, BaseFunction func)
    {
        val = val.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
        string[] splode = val.Split(',');
        if (splode[0] == "soft")
        {
            var maj = splode.ElementAtOrDefault(1);
            var min = splode.ElementAtOrDefault(2);
            var build = splode.ElementAtOrDefault(3);
            var rev = splode.ElementAtOrDefault(4);
            if (maj == null)
                Manager.Throw("MinVer Directive must express a major version");
            string[] ver = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            int majint = -1;
            int minint = -1;
            int buildint = -1;
            int revint = -1;
            int.TryParse(maj, out majint);
            if (min != null)
                int.TryParse(min, out minint);
            if (build != null)
                int.TryParse(build, out buildint);
            if (rev != null)
                int.TryParse(rev, out revint);
            if (majint == -1)
                Manager.Throw("MinVer Directive must express a major version as a number");
            if (int.Parse(ver[0]) < majint)
            {
                Manager.ThrowSilent($"Function [{func.Name}] did not pass minimum version directive. Expected Major version [{majint}].");
                if (FunctionStack.Contains(func))
                    FunctionStack.Remove(func);
                return;
            }
            if (minint != -1 && int.Parse(ver[1]) < minint)
            {
                Manager.ThrowSilent($"Function [{func.Name}] did not pass minimum version directive. Expected Minor version [{minint}].");
                if (FunctionStack.Contains(func))
                    FunctionStack.Remove(func);
                return;
            }
            if (buildint != -1 && int.Parse(ver[2]) < buildint)
            {
                Manager.ThrowSilent($"Function [{func.Name}] did not pass minimum version directive. Expected Build version [{buildint}].");
                if (FunctionStack.Contains(func))
                    FunctionStack.Remove(func);
                return;
            }
            if (revint != -1 && int.Parse(ver[3]) < revint)
            {
                Manager.ThrowSilent($"Function [{func.Name}] did not pass minimum version directive. Expected Revision version [{revint}].");
                if (FunctionStack.Contains(func))
                    FunctionStack.Remove(func);
                return;
            }
        }
        else
        {
            var maj = splode.ElementAtOrDefault(0);
            var min = splode.ElementAtOrDefault(1);
            var build = splode.ElementAtOrDefault(2);
            var rev = splode.ElementAtOrDefault(3);
            if (maj == null)
                Manager.Throw("MinVer Directive must express a major version");
            string[] ver = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            int majint = -1;
            int minint = -1;
            int buildint = -1;
            int revint = -1;
            int.TryParse(maj, out majint);
            if (min != null)
                int.TryParse(min, out minint);
            if (build != null)
                int.TryParse(build, out buildint);
            if (rev != null)
                int.TryParse(rev, out revint);
            if (majint == -1)
                Manager.Throw("MinVer Directive must express a major version as a number");
            if (int.Parse(ver[0]) < majint)
                Manager.Throw($"Function [{func.Name}] did not pass minimum version directive. Expected Major version [{majint}].");
            if (minint != -1 && int.Parse(ver[1]) < minint)
                Manager.Throw($"Function [{func.Name}] did not pass minimum version directive. Expected Minor version [{minint}].");
            if (buildint != -1 && int.Parse(ver[2]) < buildint)
                Manager.Throw($"Function [{func.Name}] did not pass minimum version directive. Expected Build version [{buildint}].");
            if (revint != -1 && int.Parse(ver[3]) < revint)
                Manager.Throw($"Function [{func.Name}] did not pass minimum version directive. Expected Revision version [{revint}].");
        }
    }
    private void DirectiveMaxVer(string val, BaseFunction func)
    {
        val = val.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
        string[] splode = val.Split(',');
        if (splode[0] == "soft")
        {
            var maj = splode.ElementAtOrDefault(1);
            var min = splode.ElementAtOrDefault(2);
            var build = splode.ElementAtOrDefault(3);
            var rev = splode.ElementAtOrDefault(4);
            if (maj == null)
                Manager.Throw("MinVer Directive must express a major version");
            string[] ver = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            int majint = -1;
            int minint = -1;
            int buildint = -1;
            int revint = -1;
            int.TryParse(maj, out majint);
            if (min != null)
                int.TryParse(min, out minint);
            if (build != null)
                int.TryParse(build, out buildint);
            if (rev != null)
                int.TryParse(rev, out revint);
            if (majint == -1)
                Manager.Throw("MinVer Directive must express a major version as a number");
            if (int.Parse(ver[0]) > majint)
            {
                Manager.ThrowSilent($"Function [{func.Name}] did not pass minimum version directive. Expected Major version [{majint}].");
                if (FunctionStack.Contains(func))
                    FunctionStack.Remove(func);
                return;
            }
            if (minint != -1 && int.Parse(ver[1]) > minint)
            {
                Manager.ThrowSilent($"Function [{func.Name}] did not pass minimum version directive. Expected Minor version [{minint}].");
                if (FunctionStack.Contains(func))
                    FunctionStack.Remove(func);
                return;
            }
            if (buildint != -1 && int.Parse(ver[2]) > buildint)
            {
                Manager.ThrowSilent($"Function [{func.Name}] did not pass minimum version directive. Expected Build version [{buildint}].");
                if (FunctionStack.Contains(func))
                    FunctionStack.Remove(func);
                return;
            }
            if (revint != -1 && int.Parse(ver[3]) > revint)
            {
                Manager.ThrowSilent($"Function [{func.Name}] did not pass minimum version directive. Expected Revision version [{revint}].");
                if (FunctionStack.Contains(func))
                    FunctionStack.Remove(func);
                return;
            }
        }
        else
        {
            var maj = splode.ElementAtOrDefault(0);
            var min = splode.ElementAtOrDefault(1);
            var build = splode.ElementAtOrDefault(2);
            var rev = splode.ElementAtOrDefault(3);
            if (maj == null)
                Manager.Throw("MinVer Directive must express a major version");
            string[] ver = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            int majint = -1;
            int minint = -1;
            int buildint = -1;
            int revint = -1;
            int.TryParse(maj, out majint);
            if (min != null)
                int.TryParse(min, out minint);
            if (build != null)
                int.TryParse(build, out buildint);
            if (rev != null)
                int.TryParse(rev, out revint);
            if (majint == -1)
                Manager.Throw("MinVer Directive must express a major version as a number");
            if (int.Parse(ver[0]) > majint)
                Manager.Throw($"Function [{func.Name}] did not pass minimum version directive. Expected Major version [{majint}].");
            if (minint != -1 && int.Parse(ver[1]) > minint)
                Manager.Throw($"Function [{func.Name}] did not pass minimum version directive. Expected Minor version [{minint}].");
            if (buildint != -1 && int.Parse(ver[2]) > buildint)
                Manager.Throw($"Function [{func.Name}] did not pass minimum version directive. Expected Build version [{buildint}].");
            if (revint != -1 && int.Parse(ver[3]) > revint)
                Manager.Throw($"Function [{func.Name}] did not pass minimum version directive. Expected Revision version [{revint}].");
        }
    }
    private void DirectiveOmit(string val, BaseFunction func)
    {
        val = val.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
        bool value = (val == "true" || val == "True") ? true : false;
        if (value)
        {
            if (FunctionStack.Contains(func))
                FunctionStack.Remove(func);
        }
    }
    //sets the override to directly hop to the base when called, while still remaining overridable
    //only one function per type can be IsBase
    private void DirectiveIsBase(string val, BaseFunction func)
    {
        val = val.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
        bool value = (val == "true" || val == "True") ? true : false;
        if (value)
        {
            var samelist = FunctionStack.Where(func.Name).ToList();
            foreach (var x in samelist)
            {
                //check to make sure this is the only IsBase directive of this type
                if (x.UID != func.UID && x.Directives != null && x.Directives.ContainsKey("IsBase"))
                {
                    var stripws = x.Directives["IsBase"].Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                    if (stripws == "true" || stripws == "True")
                        Manager.Throw($"The function [{func.Name}] has already been set IsBase by a directive. Please remove an IsBase directive.");
                }
            }
            if (FunctionStack.Contains(func))
                FunctionStack.First(func.UID).SetBase(samelist[samelist.Count - 1]);
        }
    }
    //seals the function so no more overrides can occur
    private void DirectiveSealed(string val, BaseFunction func)
    {
        val = val.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
        bool value = (val == "true" || val == "True") ? true : false;
        if (value)
        {
            //FunctionStack.MoveTo(func, 0);
            var samelist = FunctionStack.Where(func.Name).ToList();
            foreach (var x in samelist)
            {
                if (x.UID != func.UID && x.Directives != null && x.Directives.ContainsKey("Sealed"))
                {
                    var stripws = x.Directives["Sealed"].Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                    if (stripws == "true" || stripws == "True")
                        Manager.Throw($"The function [{func.Name}] has already been sealed by a directive. Please remove a Sealed directive.");
                }
            }
            func.SetSealed(value);
        }
    }
    //this is a little to obscure. using other directives to performe similar functionality
    [Obsolete]
    private void DirectiveLayer(string val, BaseFunction func)
    {
        int index = 0;
        bool tryint = int.TryParse(val, out index);
        if (!tryint)
            Manager.Throw($"The directive [Layer] must have a whole number as its value.");
        if (index - 1 > FunctionStack.List.Count)
            index = FunctionStack.List.Count - 1;
        FunctionStack.MoveTo(func, index);
        //apply any override to base connections needed since the stack was shuffled
        var samelist = FunctionStack.Where(func.Name).ToList();
        for (var i = 0; i < samelist.Count; i++)
        {
            var obj = samelist[i];
            if (!obj.IsOverride)
                continue;
            if (i == samelist.Count - 1)
                continue;
            BaseFunction next = null;
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
                Manager.Throw($"Unknown error applying directive [Layer] on function [{func.Name}]");
            obj.SetBase(next);
        }
    }
}
}
