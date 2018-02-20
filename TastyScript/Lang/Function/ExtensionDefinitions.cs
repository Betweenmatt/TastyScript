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
    public class BaseExtension<T> : IExtension
    {
        public string Name { get; protected set; }
        public string[] ExpectedArgs { get; protected set; }
        public TParameter ProvidedArgs { get; protected set; }
        public TParameter Arguments { get; set; }
        public virtual T Extend()
        {
            return default(T);
        }
        public void SetProperties(string name, string[] args)
        {
            Name = name;
            ExpectedArgs = args;
        }
    }

    [Extension("Threshold", new string[] { "int" })]
    [Serializable]
    public class ExtensionThreshold : BaseExtension<TParameter>
    {
        public override TParameter Extend()
        {
            base.Extend();
            return Arguments;
        }
        public ExtensionFor Clone()
        {
            return new ExtensionFor();
        }
    }
    [Extension("Color", new string[] { "color" })]
    [Serializable]
    public class ExtensionColor : BaseExtension<TParameter>
    {
        public override TParameter Extend()
        {
            base.Extend();
            return Arguments;
        }
        public ExtensionFor Clone()
        {
            return new ExtensionFor();
        }
    }
    [Extension("For", new string[] { "enumerator" })]
    [Serializable]
    public class ExtensionFor : BaseExtension<TParameter>
    {
        public override TParameter Extend()
        {
            base.Extend();
            return Arguments;
        }
        public ExtensionFor Clone()
        {
            return new ExtensionFor();
        }
    }
    [Extension("Then", new string[] { "condition" })]
    [Serializable]
    public class ExtensionThen : BaseExtension<TParameter>
    {
        public override TParameter Extend()
        {
            base.Extend();
            return Arguments;
        }
    }
    [Extension("Else", new string[] { "condition" })]
    [Serializable]
    public class ExtensionElse : BaseExtension<TParameter>
    {
        public override TParameter Extend()
        {
            base.Extend();
            return Arguments;
        }
    }
    [Extension("AddParams", new string[] { "e" })]
    [Serializable]
    public class ExtensionAddParams : BaseExtension<TParameter>
    {
        public override TParameter Extend()
        {
            base.Extend();
            return Arguments;
        }
    }
    [Extension("Add", new string[] { "string" })]
    [Serializable]
    public class ExtensionAdd : BaseExtension<TParameter>
    {
        public override TParameter Extend()
        {
            base.Extend();
            return Arguments;
        }
    }
}
