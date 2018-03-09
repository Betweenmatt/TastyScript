using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    
    internal interface IExtension
    {
        string Name { get; }
        string Arguments { get; }
        bool Invoking { get; }
        bool Obsolete { get; }
        void SetProperties(string name, string[] args, bool invoking, bool obsolete, bool varext);
        void SetInvokeProperties(string args);
        void SetInvokeProperties(string[] args);
        string GetInvokeProperties();
        void SetArguments(string args);
    }
    [Serializable]
    internal class EDefinition : IExtension
    {
        public string Name { get; protected set; }
        public string[] ExpectedArgs { get; protected set; }
        //public TParameter ProvidedArgs { get; protected set; }
        public string Arguments { get; set; }
        public bool Invoking { get; protected set; }
        public bool Obsolete { get; private set; }
        /// <summary>
        /// This flag indicates if the extension is used on a variable or not
        /// since function variables are treated differently.
        /// </summary>
        public bool VariableExtension { get; private set; }
        private string invokeProperties;
        public virtual string[] Extend()
        {
            ObsoleteWarning();
            return Execute();
        }
        public virtual Token Extend(Token input)
        {
            ObsoleteWarning();
            return null;
        }
        private void ObsoleteWarning()
        {
            if (Obsolete)
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.CompilerException,
                    $"The extension [{this.Name}] has been marked obsolete. Please check the documentation for future use."));
            }
        }
        private string[] Execute()
        {
            var commaRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            var reg = commaRegex.Split(Arguments);
            for (var i = 0; i < reg.Length; i++)
            {
                //get them quotes outta here!
                reg[i] = reg[i].Replace("\"", "");
            }
            return reg;
        }
        public void SetProperties(string name, string[] args, bool invoking, bool obsolete, bool varext)
        {
            Name = name;
            ExpectedArgs = args;
            Invoking = invoking;
            Obsolete = obsolete;
            VariableExtension = varext;
        }
        public void SetArguments(string args)
        {
            Arguments = args;
        }
        public void SetInvokeProperties(string args)
        {
            invokeProperties = args;
        }
        public void SetInvokeProperties(string[] args)
        {
            invokeProperties = string.Join(",", args);
            
        }
        public string GetInvokeProperties()
        {
            return invokeProperties;
        }
    }
}
