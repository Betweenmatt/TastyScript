using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Exceptions
{
    public enum ExceptionType
    {
        DriverException,
        SystemException,
        SyntaxException,
        CompilerException,
        NullReferenceException
    }
    public class ExceptionHandler
    {
        private string _line = "0";
        public string Line { get { return _line; } }
        public string Message { get; }
        public ExceptionType Type { get; }
        public ExceptionHandler(string msg)
        {
            Type = ExceptionType.CompilerException;
            Message = msg;
        }
        public ExceptionHandler(string msg, string line)
        {
            Type = ExceptionType.CompilerException;
            Message = msg;
            SetLine(line);
        }
        public ExceptionHandler(ExceptionType type, string msg)
        {
            Type = type;
            Message = msg;
        }
        public ExceptionHandler(ExceptionType type, string msg, string line)
        {
            Type = type;
            Message = msg;
            SetLine(line);
        }
        private void SetLine(string line)
        {
            if (line != null)
            {
                var firstLine = line.Split('\n').FirstOrDefault(f=>!String.IsNullOrWhiteSpace(f));
                foreach (var file in Compiler.Files)
                {
                    var sp = file.Value.Split('\n');
                    var index = 0;
                    foreach (var x in sp)
                    {
                        index++;
                        if (x.Contains(firstLine))
                            break;
                    }
                    if (index != sp.Length)
                    {
                        _line = $"{file.Key}:{index}";
                        break;
                    }
                }
            }
        }
    }
}
