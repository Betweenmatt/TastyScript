using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Containers;
using TastyScript.Lang.Extensions;

namespace TastyScript.Lang
{
    [Serializable]
    public class ExtensionStack : StaticList<EDefinition>
    {
        public static List<EDefinition> List { get { return _tlist; } }
        public static EDefinition First(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.FirstOrDefault(f => f.Name == name);
            else
                return _tlist.FirstOrDefault(f => f.Name == name || (f.Alias != null && f.Alias.Contains(name)));
        }
        public static IEnumerable<EDefinition> Where(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.Where(w => w.Name == name);
            else
                return _tlist.Where(w => w.Name == name || (w.Alias != null && w.Alias.Contains(name)));
        }
        public static IEnumerable<EDefinition> WhereContains(string name, bool useAlias = true)
        {
            if (!useAlias)
                return _tlist.Where(w => w.Name.Contains(name));
            else
                return _tlist.Where(w => w.Name.Contains(name) || (w.Alias != null && w.Alias.FirstOrDefault(f => f.Contains(name)) != null));
        }
    }
}
