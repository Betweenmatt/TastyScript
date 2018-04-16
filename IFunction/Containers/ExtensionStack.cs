using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Extension;

namespace TastyScript.IFunction.Containers
{
    public class ExtensionStack
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

        protected static List<BaseExtension> _tlist = new List<BaseExtension>();
        public static void Add(BaseExtension item)
        {
            _tlist.Add(item);
        }
        public static void Clear()
        {
            _tlist.Clear();
        }
        public static void AddRange(List<BaseExtension> range)
        {
            _tlist.AddRange(range);
        }
        public static int IndexOf(BaseExtension item)
        {
            return _tlist.IndexOf(item);
        }
        public static void RemoveAt(int index)
        {
            _tlist.RemoveAt(index);
        }
        public static bool Contains(BaseExtension item)
        {
            return _tlist.Contains(item);
        }
        public static void Remove(BaseExtension item)
        {
            _tlist.Remove(item);
        }
        public static void MoveTo(BaseExtension item, int index)
        {
            var oldindex = _tlist.IndexOf(item);
            _tlist.RemoveAt(oldindex);
            if (index > oldindex) index--;
            _tlist.Insert(index, item);
        }
    }
}
