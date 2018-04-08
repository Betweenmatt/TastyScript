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

        public void SetProperties(string name, string[] args, bool invoking, bool obsolete, bool varext, string[] alias)
        {
            Name = name;
            ExpectedArgs = args;
            Alias = alias;
            Invoking = invoking;
            Obsolete = obsolete;
            VariableExtension = varext;
        }
    }
}
