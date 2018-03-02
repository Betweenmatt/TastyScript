using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Extensions
{
    [Extension("Then", new string[] { "invoke" }, invoking: true)]
    [Serializable]
    internal class ExtensionThen : EDefinition { }
}
