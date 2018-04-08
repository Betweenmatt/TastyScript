using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.IFunction.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Function : Attribute
    {
        public string Name { get; }
        public string[] ExpectedArgs { get; }
        public string[] Alias { get; }
        public bool Invoking { get; }
        /// <summary>
        /// functions marked sealed cannot be overriden
        /// </summary>
        public bool Sealed { get; }
        /// <summary>
        /// functions marked obsolete are still called, but a warning is sent on use
        /// </summary>
        public bool Obsolete { get; }
        /// <summary>
        /// functions marked depricated get ignored by the compiler when adding to function stack
        /// </summary>
        public bool Depricated { get; }
        public bool IsAnonymous { get; }
        public Function(string name, bool isSealed = false, bool invoking = false, bool depricated = false, bool obsolete = false, string[] alias = null, bool isanon = false)
        {
            Sealed = isSealed;
            Obsolete = obsolete;
            Depricated = depricated;
            Name = name;
            Invoking = invoking;
            Alias = alias;
            ExpectedArgs = new string[] { };
            IsAnonymous = isanon;
        }
        public Function(string name, string[] args, bool isSealed = false, bool invoking = false, bool depricated = false, bool obsolete = false, string[] alias = null, bool isanon = false)
        {
            Sealed = isSealed;
            Obsolete = obsolete;
            Depricated = depricated;
            Name = name;
            Invoking = invoking;
            Alias = alias;
            ExpectedArgs = args;
            IsAnonymous = isanon;
        }
    }
}
