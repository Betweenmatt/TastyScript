using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Containers
{
    [Serializable]
    public abstract class StaticList<T>
    {
        protected static List<T> _tlist = new List<T>();
        public static void Add(T item)
        {
            _tlist.Add(item);
        }
        public static void Clear()
        {
            _tlist.Clear();
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
        public static bool Contains(T item)
        {
            return _tlist.Contains(item);
        }
        public static void Remove(T item)
        {
            _tlist.Remove(item);
        }
        public static void MoveTo(T item, int index)
        {
            var oldindex = _tlist.IndexOf(item);
            _tlist.RemoveAt(oldindex);
            if (index > oldindex) index--;
            _tlist.Insert(index, item);
        }
    }
}
