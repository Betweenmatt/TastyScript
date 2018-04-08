using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.IFunction.Containers
{
    [Serializable]
    public abstract class ObjectList<T>
    {
        protected List<T> _tlist;
        public ObjectList()
        {
            _tlist = new List<T>();
        }
        public void Add(T item)
        {
            _tlist.Add(item);
        }
        public void Clear()
        {
            _tlist = new List<T>();
        }
        public void AddRange(List<T> range)
        {
            _tlist.AddRange(range);
        }
        public int IndexOf(T item)
        {
            return _tlist.IndexOf(item);
        }
        public void Insert(int index, T item)
        {
            _tlist.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            _tlist.RemoveAt(index);
        }
    }
}
