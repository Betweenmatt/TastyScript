﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Extensions
{
    [Extension("Else", new string[] { "invoke" }, invoking: true, alias: new string[] { "else" })]
    [Serializable]
    internal class ExtensionElse : EDefinition { }
}
