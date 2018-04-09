using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;

namespace TastyScript.CoreExtensions.Function
{
    [Extension("Threshold", new string[] { "int" }, obsolete:true)]
    [Serializable]
    public class ExtensionThreshold : BaseExtension { }
}
