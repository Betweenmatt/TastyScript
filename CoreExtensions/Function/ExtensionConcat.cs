using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;

namespace TastyScript.CoreExtensions.Function
{
    [Extension("Concat", new string[] { "string" })]
    [Serializable]
    public class ExtensionConcat : BaseExtension { }
}
