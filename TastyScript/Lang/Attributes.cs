﻿using System;

namespace TastyScript.Lang
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class Function : Attribute
    {
        public string Name { get; }
        public string[] ExpectedArgs { get; }
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
        public Function(string name, bool isSealed = false, bool invoking = false, bool depricated = false, bool obsolete = false)
        {
            Sealed = isSealed;
            Obsolete = obsolete;
            Depricated = depricated;
            Name = name;
            Invoking = invoking;
            ExpectedArgs = new string[] { };
        }
        public Function(string name, string[] args, bool isSealed = false, bool invoking = false, bool depricated = false, bool obsolete = false)
        {
            Sealed = isSealed;
            Obsolete = obsolete;
            Depricated = depricated;
            Name = name;
            Invoking = invoking;
            ExpectedArgs = args;
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class Extension : Attribute
    {
        public string Name { get; }
        /// <summary>
        /// functions marked sealed cannot be overriden
        /// </summary>
        public bool Sealed { get; }
        public string[] ExpectedArgs { get; }
        public bool Invoking { get; }
        /// <summary>
        /// functions marked obsolete get ignored by the compiler when adding to function stack
        /// </summary>
        public bool Depricated { get; }
        /// <summary>
        /// functions marked obsolete are still called, but a warning is sent on use
        /// </summary>
        public bool Obsolete { get; }
        public Extension(string name, bool isSealed = false, bool invoking = false, bool depricated = false, bool obsolete = false)
        {
            Sealed = isSealed;
            Depricated = depricated;
            Obsolete = obsolete;
            Name = name;
            Invoking = invoking;
            ExpectedArgs = new string[] { };
        }
        public Extension(string name, string[] args, bool isSealed = false, bool invoking = false, bool depricated = false, bool obsolete = false)
        {
            Sealed = isSealed;
            Obsolete = obsolete;
            Depricated = depricated;
            Name = name;
            Invoking = invoking;
            ExpectedArgs = args;
        }
    }
}
