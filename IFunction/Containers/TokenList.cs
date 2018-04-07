using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Tokens;

namespace TastyScript.IFunction.Containers
{
    public class TokenList : ObjectList<Token>
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
}
