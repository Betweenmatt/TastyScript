using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;

namespace TastyScript.CoreExtensions.Function
{
    [Extension("And", new string[] { "condition" }, alias:new string[] { "and" })]
    [Serializable]
    public class ExtensionAnd : BaseExtension { }
}
