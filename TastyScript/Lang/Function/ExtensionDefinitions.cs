using System;
using System.Collections.Generic;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Func
{
    public interface IExtension
    {
        string Name { get; }
        TParameter Arguments { get; set; }
        bool Invoking { get; }
        void SetProperties(string name, string[] args, bool invoking = false);
        void SetInvokeProperties(TParameter args);
        TParameter GetInvokeProperties();
    }
    [Serializable]
    public class BaseExtension : IExtension
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

    [Extension("Print", new string[] { "type" }),Serializable]
    public class ExtensionPrint : BaseExtension { }
    [Extension("Stop"),Serializable]
    public class ExtensionStop : BaseExtension { }
    [Extension("Start"),Serializable]
    public class ExtensionStart : BaseExtension { }
    [Extension("Concat", new string[] { "string" })]
    [Serializable]
    public class ExtensionConcat : BaseExtension { }
    [Extension("Threshold", new string[] { "int" })]
    [Serializable]
    public class ExtensionThreshold : BaseExtension { }
    [Extension("Color", new string[] { "color" })]
    [Serializable]
    public class ExtensionColor : BaseExtension { }
    [Extension("For", new string[] { "enumerator" })]
    [Serializable]
    public class ExtensionFor : BaseExtension { }
    [Extension("Then", new string[] { "invoke" }, invoking:true)]
    [Serializable]
    public class ExtensionThen : BaseExtension { }
    [Extension("Or", new string[] { "condition" })]
    [Serializable]
    public class ExtensionOr : BaseExtension { }
    [Extension("And", new string[] { "condition" })]
    [Serializable]
    public class ExtensionAnd : BaseExtension { }
    [Extension("Else", new string[] { "invoke" }, invoking: true)]
    [Serializable]
    public class ExtensionElse : BaseExtension { }
    [Extension("AddParams", new string[] { "e" }, FunctionObsolete: true)]
    [Serializable]
    public class ExtensionAddParams : BaseExtension { }
    [Extension("Add", new string[] { "string" }, FunctionObsolete: true)]
    [Serializable]
    public class ExtensionAdd : BaseExtension { }
}
