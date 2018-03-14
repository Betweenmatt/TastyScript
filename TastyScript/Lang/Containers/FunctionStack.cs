using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Containers;

namespace TastyScript.Lang
{
    [Serializable]
    internal class FunctionStack : StaticList<IBaseFunction>
    {
        public static List<IBaseFunction> List { get { return _tlist; } }
        public static IBaseFunction First(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.FirstOrDefault(f => f.Name == name);
            else
                return _tlist.FirstOrDefault(f => f.Name == name || (f.Alias != null && f.Alias.Contains(name)));
        }
        public static IEnumerable<IBaseFunction> Where(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.Where(w => w.Name == name);
            else
                return _tlist.Where(w => w.Name == name || (w.Alias != null && w.Alias.Contains(name)));
        }
        public static IEnumerable<IBaseFunction> WhereContains(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.Where(w => w.Name.Contains(name));
            else
                return _tlist.Where(w => w.Name.Contains(name) || (w.Alias != null && w.Alias.FirstOrDefault(f => f.Contains(name)) != null));
        }
        public static IBaseFunction Last(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.LastOrDefault(f => f.Name == name);
            else
                return _tlist.LastOrDefault(f => f.Name == name || (f.Alias != null && f.Alias.Contains(name)));
        }
        
    }
    
}
