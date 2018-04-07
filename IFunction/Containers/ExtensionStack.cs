using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Extension;

namespace TastyScript.IFunction.Containers
{
    public class ExtensionStack : ObjectStack<BaseExtension>
    {
        public static List<BaseExtension> List { get { return _tlist; } }
        public static BaseExtension First(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.FirstOrDefault(f => f.Name == name);
            else
                return _tlist.FirstOrDefault(f => f.Name == name || (f.Alias != null && f.Alias.Contains(name)));
        }
        public static IEnumerable<BaseExtension> Where(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.Where(w => w.Name == name);
            else
                return _tlist.Where(w => w.Name == name || (w.Alias != null && w.Alias.Contains(name)));
        }
        public static IEnumerable<BaseExtension> WhereContains(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.Where(w => w.Name.Contains(name));
            else
                return _tlist.Where(w => w.Name.Contains(name) || (w.Alias != null && w.Alias.FirstOrDefault(f => f.Contains(name)) != null));
        }
    }
}
