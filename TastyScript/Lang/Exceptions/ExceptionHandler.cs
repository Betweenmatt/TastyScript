using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang
{
    internal enum ExceptionType
    {
        DriverException,
        SystemException,
        SyntaxException,
        CompilerException,
        NullReferenceException
    }
    internal class ExceptionHandler
    {
        private string _line = "0";
        public string Line { get { return _line; } }
        private string _snippet = "[No snippet available]";
        public string Snippet { get { return _snippet; } }
        public string Message { get; }
        public ExceptionType Type { get; }
        public ExceptionHandler(string msg)
        {
            Type = ExceptionType.CompilerException;
            Message = msg;
            SetLine("");
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
            SetLine("");
        }
        public ExceptionHandler(ExceptionType type, string msg, string line)
        {
            Type = type;
            Message = msg;
            SetLine(line);
        }
        private void SetLine(string line)
        {
            //trying something different
            line = Compiler.ExceptionListener.CurrentLine;
            if (line.Contains("AnonymousFunction"))
            {
                var anonlist = TokenParser.FunctionList.Where(w => w.Name.Contains("AnonymousFunction"));
                foreach(var x in anonlist)
                {
                    line = line.Replace("\""+x.Name+"\"", "=" + x.Value);
                }
            }
            _snippet = line;
            if (line != null)
            {
                var firstLine = line.Split('\n').FirstOrDefault(f=>!String.IsNullOrWhiteSpace(f));
                if (firstLine != null)
                {
                    foreach (var file in Compiler.Files)
                    {
                        var sp = file.Value.Split('\n');
                        var index = 0;
                        foreach (var x in sp)
                        {
                            index++;
                            if (x != null && x.Contains(firstLine))
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
}
