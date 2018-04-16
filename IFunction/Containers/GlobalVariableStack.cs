using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Tokens;

namespace TastyScript.IFunction.Containers
{
    public class GlobalVariableStack
    {
        public static List<Token> List { get { return _tlist; } }
        public static Token First(string name) => _tlist.FirstOrDefault(f => f.Name == name);


        protected static List<Token> _tlist = new List<Token>();
        public static void Add(Token item)
        {
            _tlist.Add(item);
        }
        public static void Clear()
        {
            _tlist.Clear();
        }
        public static void AddRange(List<Token> range)
        {
            _tlist.AddRange(range);
        }
        public static int IndexOf(Token item)
        {
            return _tlist.IndexOf(item);
        }
        public static void RemoveAt(int index)
        {
            _tlist.RemoveAt(index);
        }
        public static bool Contains(Token item)
        {
            return _tlist.Contains(item);
        }
        public static void Remove(Token item)
        {
            _tlist.Remove(item);
        }
        public static void MoveTo(Token item, int index)
        {
            var oldindex = _tlist.IndexOf(item);
            _tlist.RemoveAt(oldindex);
            if (index > oldindex) index--;
            _tlist.Insert(index, item);
        }
    }
}
