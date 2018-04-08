using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Tokens;

namespace TastyScript.IFunction.Containers
{
    public class AnonymousTokenStack : ObjectStack<Token>
    {
        private static int _anonymousTokenIndex = -1;

        public static int AnonymousTokenIndex
        {
            get
            {
                _anonymousTokenIndex++;
                return _anonymousTokenIndex;
            }
        }

        public static Token First(string name) => _tlist.FirstOrDefault(f => f.Name == name);
    }
}
