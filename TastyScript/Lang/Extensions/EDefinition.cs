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
        void SetProperties(string name, string[] args, bool invoking = false);
        void SetInvokeProperties(string args);
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
        private string invokeProperties;
        public virtual string[] Extend()
        {
            var commaRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            var reg =  commaRegex.Split(Arguments);
            for (var i = 0; i < reg.Length; i++)
            {
                //get them quotes outta here!
                reg[i] = reg[i].Replace("\"", "");
            }
            return reg;
        }
        public virtual string[] Extend(Token input)
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
        public void SetProperties(string name, string[] args, bool invoking = false)
        {
            Name = name;
            ExpectedArgs = args;
            Invoking = invoking;
        }
        public void SetArguments(string args)
        {
            Arguments = args;
        }
        public void SetInvokeProperties(string args)
        {
            invokeProperties = args;
        }
        public string GetInvokeProperties()
        {
            return invokeProperties;
        }
    }
}
