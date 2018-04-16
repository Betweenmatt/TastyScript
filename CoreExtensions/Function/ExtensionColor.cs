using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;

namespace TastyScript.CoreExtensions.Function
{
    [Extension("Color", new string[] { "color" })]
    [Serializable]
    public class ExtensionColor : BaseExtension {
        public override string[] Extend()
        {
            return base.Extend();
        }
    }
}
