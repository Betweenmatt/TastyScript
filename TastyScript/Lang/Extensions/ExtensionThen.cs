using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Extensions
{
    [Extension("Then", new string[] { "invoke" }, invoking: true, alias:new string[] { "then" })]
    [Serializable]
    internal class ExtensionThen : EDefinition { }
}
