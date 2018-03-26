using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Extensions
{
    [Extension("For", new string[] { "enumerator" })]
    [Serializable]
    public class ExtensionFor : EDefinition { }
}
