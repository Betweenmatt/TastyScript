using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScriptNPP.FunctionTips
{
    internal class XmlFunctionTipsKeyword
    {
        public string Name;
        public List<XmlFunctionTipsOverload> Overloads;
    }
    internal class XmlFunctionTipsOverload
    {
        public string RetVal;
        public string Descr;
        public List<XmlFunctionTipsParam> Parameters;
    }
    internal class XmlFunctionTipsParam
    {
        public string Name;
    }
}