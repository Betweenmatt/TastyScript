using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.ParserManager
{
    public static class Utilities
    {
        public static string ScopeRegex(string input)
        {
            return @"(" + input + @"([^{}]*){)([^{}]+|(?<Level>\{)| (?<-Level>\}))+(?(Level)(?!))\}";
        }
    }
}
