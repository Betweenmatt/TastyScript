using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Extension;

namespace TastyScript.IFunction.Containers
{
    public class ExtensionList : ObjectList<BaseExtension>
    {
        public List<BaseExtension> List { get { return _tlist; } }
        public BaseExtension First(string name)
        {
            return _tlist.FirstOrDefault(f => f.Name == name);
        }
        public IEnumerable<BaseExtension> Where(string name)
        {
            return _tlist.Where(w => w.Name == name);
        }
        public IEnumerable<BaseExtension> WhereContains(string name)
        {
            return _tlist.Where(w => w.Name.Contains(name));
        }
    }
}
