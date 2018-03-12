using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang
{
    internal class TokenStack : ObjectStack<Token>
    {
        public List<Token> List { get { return _tlist; } }
        public Token First(string name)
        {
            return _tlist.FirstOrDefault(f => f.Name == name);
        }
        public IEnumerable<Token> Where(string name)
        {
            return _tlist.Where(w => w.Name == name);
        }
        public IEnumerable<Token> WhereContains(string name)
        {
            return _tlist.Where(w => w.Name.Contains(name));
        }
    }
    internal abstract class ObjectStack<T>
    {
        protected List<T> _tlist;
        public ObjectStack()
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
        public void RemoveAt(int index)
        {
            _tlist.RemoveAt(index);
        }
    }
}
