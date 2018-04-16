using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TastyScript.IFunction.Function;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.IFunction.Extension
{
    public abstract class BaseExtension
    {
        public string[] Alias { get; private set; }
        public string Name { get; private set; }
        public string[] ExpectedArgs { get; protected set; }
        public string Arguments { get; set; }
        public bool IsInvoking { get; protected set; }
        public bool IsObsolete { get; private set; }
        /// <summary>
        /// This flag indicates if the extension is used on a variable or not
        /// since function variables are treated differently.
        /// </summary>
        public bool IsVariableExtension { get; private set; }
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
        public virtual string[] Extend(BaseFunction input)
        {
            ObsoleteWarning();
            Manager.Throw("[51]Unexpected error occured.");
            return null;
        }
        private void ObsoleteWarning()
        {
            if (IsObsolete)
            {
                Manager.ThrowSilent($"The extension [{this.Name}] has been marked obsolete. Please check the documentation for future use.");
            }
        }
        protected string[] Execute()
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
        protected string[] Execute(string str)
        {
            var commaRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            var reg = commaRegex.Split(str);
            for (var i = 0; i < reg.Length; i++)
            {
                //get them quotes outta here!
                reg[i] = reg[i].Replace("\"", "");
            }
            return reg;
        }
        public void SetProperties(string name, string[] args, bool invoking, bool obsolete, bool varext, string[] alias)
        {
            Name = name;
            ExpectedArgs = args;
            Alias = alias;
            IsInvoking = invoking;
            IsObsolete = obsolete;
            IsVariableExtension = varext;
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
