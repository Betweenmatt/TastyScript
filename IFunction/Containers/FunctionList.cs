using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Function;

namespace TastyScript.IFunction.Containers
{
    public class FunctionList : ObjectList<BaseFunction>
    {
        public FunctionList() : base() { }
        public FunctionList(List<BaseFunction> list) => _tlist = list;
        public List<BaseFunction> List { get { return _tlist; } }
        public BaseFunction First(string name)
        {
            return _tlist.FirstOrDefault(f => f.Name == name);
        }
        public IEnumerable<BaseFunction> Where(string name)
        {
            return _tlist.Where(w => w.Name == name);
        }
        public IEnumerable<BaseFunction> WhereContains(string name)
        {
            return _tlist.Where(w => w.Name.Contains(name));
        }
    }
}
