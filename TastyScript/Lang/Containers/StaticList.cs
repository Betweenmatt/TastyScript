using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Containers
{
    internal abstract class StaticList<T>
    {
        protected static List<T> _tlist = new List<T>();
        public static void Add(T item)
        {
            _tlist.Add(item);
        }
        public static void Clear()
        {
            _tlist = new List<T>();
        }
        public static void AddRange(List<T> range)
        {
            _tlist.AddRange(range);
        }
        public static int IndexOf(T item)
        {
            return _tlist.IndexOf(item);
        }
        public static void RemoveAt(int index)
        {
            _tlist.RemoveAt(index);
        }
    }
}
