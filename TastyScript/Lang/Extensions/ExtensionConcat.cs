using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Extensions
{
    [Extension("Concat", new string[] { "string" })]
    [Serializable]
    internal class ExtensionConcat : EDefinition { }
}
