using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.ExceptionHandler
{
    public interface IExceptionHandler
    {
        void Throw(string msg);
        void Throw(ExceptionType type, string msg);
        void ThrowSilent(string msg);
        void ThrowSilent(ExceptionType type, string msg);
        void ThrowDebug(string msg);
    }
    public enum ExceptionType
    {
        CompilerException,
        SystemException,
        UserDefinedException,
        NullReferenceException
    }
}
