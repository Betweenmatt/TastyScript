using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Extensions
{
    [Extension("And", new string[] { "condition" })]
    [Serializable]
    internal class ExtensionAnd : EDefinition { }
}
