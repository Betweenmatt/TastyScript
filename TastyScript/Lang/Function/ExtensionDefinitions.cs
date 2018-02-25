using System;
using System.Collections.Generic;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Func
{
    public interface IExtension
    {
        string Name { get; }
        TParameter Arguments { get; set; }
        void SetProperties(string name, string[] args);
    }
    [Serializable]
    public class BaseExtension : IExtension
    {
        public string Name { get; protected set; }
        public string[] ExpectedArgs { get; protected set; }
        public TParameter ProvidedArgs { get; protected set; }
        public TParameter Arguments { get; set; }
        public virtual TParameter Extend()
        {
            return Arguments;
        }
        public void SetProperties(string name, string[] args)
        {
            Name = name;
            ExpectedArgs = args;
        }
    }

    [Extension("Concat", new string[] { "string" })]
    [Serializable]
    public class ExtensionConcat : BaseExtension
    {
    }
    [Extension("Threshold", new string[] { "int" })]
    [Serializable]
    public class ExtensionThreshold : BaseExtension
    {
    }
    [Extension("Color", new string[] { "color" })]
    [Serializable]
    public class ExtensionColor : BaseExtension
    {
    }
    [Extension("For", new string[] { "enumerator" })]
    [Serializable]
    public class ExtensionFor : BaseExtension
    {
    }
    [Extension("Then", new string[] { "condition" })]
    [Serializable]
    public class ExtensionThen : BaseExtension
    {
    }
    [Extension("Else", new string[] { "condition" })]
    [Serializable]
    public class ExtensionElse : BaseExtension
    {
    }
    [Extension("AddParams", new string[] { "e" })]
    [Serializable]
    public class ExtensionAddParams : BaseExtension
    {
    }
    [Extension("Add", new string[] { "string" })]
    [Serializable]
    public class ExtensionAdd : BaseExtension
    {
    }
}
