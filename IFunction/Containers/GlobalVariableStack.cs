using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Tokens;

namespace TastyScript.IFunction.Containers
{
    public class GlobalVariableStack : ObjectStack<Token>
    {
        public static Token First(string name) => _tlist.FirstOrDefault(f => f.Name == name);
        /// <summary>
        /// Converts this container to type TokenList
        /// </summary>
        /// <returns></returns>
        public static TokenList AsTokenList()
        {
            var temp = new TokenList();
            temp.AddRange(_tlist);
            return temp;
        }
    }
}
