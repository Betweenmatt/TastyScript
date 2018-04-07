using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.IFunction.Extension
{
    public abstract class BaseExtension
    {
        public string[] Alias { get; private set; }
        public string Name { get; private set; }
    }
}
