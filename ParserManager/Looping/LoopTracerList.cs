using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.Looping
{
    public class LoopTracerList
    {
        private List<LoopTracer> _tlist;
        public LoopTracerList() => _tlist = new List<LoopTracer>();
        public void Add(LoopTracer item) => _tlist.Add(item);
        public void Remove(LoopTracer item) => _tlist.Remove(item);
        public LoopTracer Last() => _tlist.LastOrDefault();
        public LoopTracer First() => _tlist.FirstOrDefault();
        public List<LoopTracer> List() => _tlist;
    }
}
