using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Func;

namespace TastyScript.Lang
{
    public class Compiler
    {
        private List<IBaseFunction> _compileStack;
        public static Dictionary<string, string> Files;
        public static IExceptionListener ExceptionListener;
        public static List<IBaseFunction> PredefinedList;
        public static List<LoopTracer> LoopTracerStack;
        private static int _anonymousFunctionIndex = -1;
        public static int AnonymousFunctionIndex
        {
            get
            {
                _anonymousFunctionIndex++;
                return _anonymousFunctionIndex;
            }
        }
        public Compiler(string filename, string file, List<IBaseFunction> predefined)
        {
            LoopTracerStack = new List<LoopTracer>();
            TokenParser.FunctionList = new List<IBaseFunction>();//clear this every run
            Files = new Dictionary<string, string>();
            Files.Add(filename, file);
            _compileStack = GetScopes(file, predefined);
            _compileStack.AddRange(predefined);
            StartScope(_compileStack);
        }

        private List<IBaseFunction> GetImports(string[] imports, List<IBaseFunction> predefined)
        {
            var temp = new List<IBaseFunction>();
            foreach (var x in imports)
            {
                var file = x.Split(';')[0];
                if (file != "")
                {
                    try
                    {
                        var path = file.Replace("\'", "").Replace("\"", "");
                        var fileContents = Program.GetFileFromPath(path);
                        Files.Add(path, fileContents);
                        //add functions first
                        temp.AddRange(GetScopes(fileContents, predefined));
                    }
                    catch 
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Error importing {file}."));
                    }
                }
            }
            return temp;
        }
        private List<IBaseFunction> GetScopes(string file, List<IBaseFunction> predefined)
        {
            var temp = new List<IBaseFunction>();
            //remove all comments
            var commRegex = new Regex(@"#([^\n]+)$",RegexOptions.Multiline);
            file = commRegex.Replace(file, "");

            //add functions first
            var scopeRegex = new Regex(ScopeRegex(@"\bfunction\.\b"),RegexOptions.IgnorePatternWhitespace);
            //var scopeRegex = new Regex(@"\bfunction\.\b([^}]*)\}");
            var scopes = scopeRegex.Matches(file);
            foreach (var s in scopes)
            {
                temp.Add(new AnonymousFunction<object>(s.ToString()));
            }
            //add inherits second
            var _inheritStack = new List<IBaseFunction>();
            var inheritRegex = new Regex(ScopeRegex(@"\boverride\.\b"), RegexOptions.IgnorePatternWhitespace);
            //var inheritRegex = new Regex(@"\boverride\.\b([^}]*)\}");
            var inherits = inheritRegex.Matches(file);
            foreach (var i in inherits)
            {
                var obj = new AnonymousFunction<object>(i.ToString(), predefined);
                _inheritStack.Add(obj);
            }
            //add imports
            var imports = new List<IBaseFunction>();
            var splitEndImport = file.Split(new string[] { "@end" }, StringSplitOptions.None);
            if (splitEndImport.Length > 1)
            {
                var splitImport = splitEndImport[0].Split(new string[] { "@import " }, StringSplitOptions.None);
                imports.AddRange(GetImports(splitImport, predefined));
            }
            temp.AddRange(_inheritStack);
            temp.AddRange(imports);
            return temp;
        }
        //=>
        //\boverride\.\b
        public static string ScopeRegex(string s)
        {
            return @"("+s+@"([^{}]*){)([^{}]+|(?<Level>\{)| (?<-Level>\}))+(?(Level)(?!))\}";
        }
        private void StartScope(List<IBaseFunction> list)
        {
            TokenParser p = new TokenParser(list);
        }
    }
    public class LoopTracer
    {
        public bool Break { get; private set; }
        public bool Continue { get; private set; }
        public void SetBreak(bool _break)
        {
            Break = _break;
        }
        public void SetContinue(bool _continue)
        {
            Continue = _continue;
        }
    }
}
