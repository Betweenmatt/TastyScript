using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Extensions
{
    [Extension("Color", new string[] { "color" })]
    [Serializable]
    internal class ExtensionColor : EDefinition {
        public override string[] Extend()
        {
            return base.Extend();
        }
    }
}
