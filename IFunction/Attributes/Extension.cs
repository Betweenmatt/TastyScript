using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.IFunction.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Extension : Attribute
    {
        public string Name { get; }
        /// <summary>
        /// functions marked sealed cannot be overriden
        /// </summary>
        public bool Sealed { get; }
        public string[] ExpectedArgs { get; }
        public string[] Alias { get; }
        public bool Invoking { get; }
        /// <summary>
        /// functions marked obsolete get ignored by the compiler when adding to function stack
        /// </summary>
        public bool Depricated { get; }
        /// <summary>
        /// functions marked obsolete are still called, but a warning is sent on use
        /// </summary>
        public bool Obsolete { get; }
        /// <summary>
        /// This flag indicates if the extension is meant for a variable,
        /// since function extensions are triggered using a different method.
        /// </summary>
        public bool VariableExtension { get; }
        public Extension(string name, bool isSealed = false, bool invoking = false, bool depricated = false, bool obsolete = false, bool varExtension = false, string[] alias = null)
        {
            Sealed = isSealed;
            Depricated = depricated;
            Obsolete = obsolete;
            VariableExtension = varExtension;
            Name = name;
            Invoking = invoking;
            Alias = alias;
            ExpectedArgs = new string[] { };
        }
        public Extension(string name, string[] args, bool isSealed = false, bool invoking = false, bool depricated = false, bool obsolete = false, bool varExtension = false, string[] alias = null)
        {
            Sealed = isSealed;
            Obsolete = obsolete;
            VariableExtension = varExtension;
            Depricated = depricated;
            Name = name;
            Invoking = invoking;
            Alias = alias;
            ExpectedArgs = args;
        }
    }
}
