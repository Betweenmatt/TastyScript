using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;

namespace TastyScript.CoreExtensions.Function
{
    [Extension("Then", new string[] { "invoke" }, invoking: true, alias:new string[] { "then" })]
    [Serializable]
    public class ExtensionThen : BaseExtension { }
}
