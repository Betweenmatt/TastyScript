using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Function;

namespace TastyScript.IFunction.Containers
{
    [Serializable]
    public class FunctionStack : ObjectStack<BaseFunction>
    {
        public static List<BaseFunction> List { get { return _tlist; } }
        public static BaseFunction First(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.FirstOrDefault(f => f.Name == name);
            else
                return _tlist.FirstOrDefault(f => f.Name == name || (f.Alias != null && f.Alias.Contains(name)));
        }
        public static BaseFunction First(int uid)
        {
            return _tlist.FirstOrDefault(f => f.UID == uid);
        }
        public static IEnumerable<BaseFunction> Where(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.Where(w => w.Name == name);
            else
                return _tlist.Where(w => w.Name == name || (w.Alias != null && w.Alias.Contains(name)));
        }
        public static IEnumerable<BaseFunction> WhereContains(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.Where(w => w.Name.Contains(name));
            else
                return _tlist.Where(w => w.Name.Contains(name) || (w.Alias != null && w.Alias.FirstOrDefault(f => f.Contains(name)) != null));
        }
        public static BaseFunction Last(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.LastOrDefault(f => f.Name == name);
            else
                return _tlist.LastOrDefault(f => f.Name == name || (f.Alias != null && f.Alias.Contains(name)));
        }
        public static BaseFunction Last(int uid)
        {
            return _tlist.LastOrDefault(l => l.UID == uid);
        }
    }
}
