using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.ExceptionHandler
{
    public interface IExceptionHandler
    {
        void Throw(string message);
        void Throw(string message, ExceptionType type);
        void ThrowSilent(string message, bool once);
        void ThrowSilent(string message, ExceptionType type, bool once);
        void ThrowDebug(string msg);
        void LogThrow(string msg, Exception e);
        TryCatchStack TryCatchEventStack { get; }
    }
}
namespace TastyScript
{
    public enum ExceptionType
    {
        CompilerException,
        ArgumentException,
        DriverException,
        SyntaxException,
        SystemException,
        UserDefinedException,
        NullReferenceException,
        NotImplementedException
    }
}