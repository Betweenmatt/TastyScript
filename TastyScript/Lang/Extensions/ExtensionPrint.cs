using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Extensions
{
    [Extension("Print", new string[] { "type" }), Serializable]
    internal class ExtensionPrint : EDefinition { }
}
