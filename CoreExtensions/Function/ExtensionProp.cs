using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;

namespace TastyScript.CoreExtensions.Function
{
    [Extension("Prop", new string[] { "int" })]
    [Serializable]
    public class ExtensionProp : BaseExtension { }
}
