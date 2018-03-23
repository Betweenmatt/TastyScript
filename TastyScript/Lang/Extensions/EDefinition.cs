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
        string[] Alias { get; }
        void SetProperties(string name, string[] args, bool invoking, bool obsolete, bool varext, string[] alias);
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
        public string[] Alias { get; protected set; }
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
        public virtual string[] Extend(IBaseFunction input)
        {
            ObsoleteWarning();
            Compiler.ExceptionListener.Throw("[51]Unexpected error occured.");
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
    /*
     * 
     * Custom extensions with `this function` aren't triggered until the function calles `This("Extension").Prop("Args",{ExtName});`
     * This is how the extension gets the callers name(which can be retrieved by the `function` var inside the extension. `Return`
     * can be handled however u want
     * 
     * Custom extensions with `this variable` are triggered when parsed, with `variable` var being the variable this extension is attached
     * to, and the `Return` is the value that is returned since all value types are immutable
     * 
     * */
    internal class CustomExtension : EDefinition
    {
        public IBaseFunction FunctionReference;
        public override string[] Extend(IBaseFunction input)
        {
            List<string> temp = new List<string>();
            temp.Add(input.Name);
            temp.AddRange(base.Extend());
            FunctionReference.TryParse(new TFunction(FunctionReference, null, temp.ToArray(), null));
            return Execute(FunctionReference.ReturnBubble.ToString());
        }
        public override Token Extend(Token input)
        {
            List<string> temp = new List<string>();
            temp.Add(input.ToString());
            temp.AddRange(base.Extend());
            FunctionReference.TryParse(new TFunction(FunctionReference, null, temp.ToArray(), null));
            return FunctionReference.ReturnBubble;
        }
    }
}
