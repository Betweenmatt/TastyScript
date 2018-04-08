using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Base", isSealed: true)]
    internal class FunctionBase : FunctionDefinition
    {
        public override void TryParse(TFunction caller)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"{this.Name} can not be overrided", LineValue));
        }
        public override string CallBase()
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"{this.Name} can not be overrided.", LineValue));
            return null;
        }
        protected override void ForExtension(TFunction caller, ExtensionFor findFor)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
}
