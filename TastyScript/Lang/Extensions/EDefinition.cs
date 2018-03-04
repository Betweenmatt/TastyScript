using System;
using System.Collections.Generic;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Extensions
{
    internal interface IExtension
    {
        string Name { get; }
        TParameter Arguments { get; set; }
        bool Invoking { get; }
        void SetProperties(string name, string[] args, bool invoking = false);
        void SetInvokeProperties(TParameter args);
        TParameter GetInvokeProperties();
    }
    [Serializable]
    internal class EDefinition : IExtension
    {
        public string Name { get; protected set; }
        public string[] ExpectedArgs { get; protected set; }
        //public TParameter ProvidedArgs { get; protected set; }
        public TParameter Arguments { get; set; }
        public bool Invoking { get; protected set; }
        private TParameter invokeProperties;
        public virtual TParameter Extend()
        {
            return Arguments;
        }
        public virtual TParameter Extend(IBaseToken input)
        {
            return Arguments;
        }
        public void SetProperties(string name, string[] args, bool invoking = false)
        {
            Name = name;
            ExpectedArgs = args;
            Invoking = invoking;
        }
        public void SetInvokeProperties(TParameter args)
        {
            invokeProperties = args;
        }
        public TParameter GetInvokeProperties()
        {
            return invokeProperties;
        }
    }
}
