using System;

namespace TastyScript.Lang
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Function : Attribute
    {
        public string Name { get; }
        public string[] ExpectedArgs { get; }
        /// <summary>
        /// functions marked obsolete get ignored by the compiler when adding to function stack
        /// </summary>
        public bool Obsolete { get; }
        public Function(string name, bool FunctionObsolete = false)
        {
            Obsolete = FunctionObsolete;
            Name = name;
            ExpectedArgs = new string[] { };
        }
        public Function(string name, string[] args, bool FunctionObsolete = false)
        {
            Obsolete = FunctionObsolete;
            Name = name;
            ExpectedArgs = args;
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Extension : Attribute
    {
        public string Name { get; }
        public string[] ExpectedArgs { get; }
        /// <summary>
        /// functions marked obsolete get ignored by the compiler when adding to function stack
        /// </summary>
        public bool Obsolete { get; }
        public Extension(string name, bool FunctionObsolete = false)
        {
            Obsolete = FunctionObsolete;
            Name = name;
            ExpectedArgs = new string[] { };
        }
        public Extension(string name, string[] args, bool FunctionObsolete = false)
        {
            Obsolete = FunctionObsolete;
            Name = name;
            ExpectedArgs = args;
        }
    }
}
