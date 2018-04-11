using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;

namespace TastyScript.CoreExtensions.Function
{
    [Extension("For", new string[] { "enumerator" }, alias:new string[] { "for" })]
    [Serializable]
    public class ExtensionFor : BaseExtension { }
}
