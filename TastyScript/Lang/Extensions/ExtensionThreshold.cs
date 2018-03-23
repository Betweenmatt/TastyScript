using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Extensions
{
    [Extension("Threshold", new string[] { "int" }, obsolete:true)]
    [Serializable]
    internal class ExtensionThreshold : EDefinition { }
}
