using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.Looping
{
    public class LoopTracer
    {
        private static int index = 0;
        public bool Break { get; private set; }
        public bool Continue { get; private set; }
        public int ID { get; }
        public LoopTracer()
        {
            ID = index++;
        }
        public void SetBreak(bool _break)
        {
            Break = _break;
        }
        public void SetContinue(bool _continue)
        {
            Continue = _continue;
        }
    }
}
