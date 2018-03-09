using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TastyScript.Lang.Exceptions;

namespace TastyScript.Lang
{
    internal class Compiler
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
            FunctionStack.Clear();//clear this every run
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
                    var path = file.Replace("\'", "").Replace("\"", "");
                    if (!Files.ContainsKey(path))
                    {
                        try
                        {
                            var fileContents = Program.GetFileFromPath(path);
                            Files.Add(path, fileContents);
                            //add functions first
                            temp.AddRange(GetScopes(fileContents, predefined));
                        }
                        catch
                        {
                            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Error importing {file}."));
                        }
                    }else
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                            $"Cannot import file: {path} because it has already been imported", file));
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
                temp.Add(new AnonymousFunction(s.ToString()));
            }
            //add inherits second
            var _inheritStack = new List<IBaseFunction>();
            var inheritRegex = new Regex(ScopeRegex(@"\boverride\.\b"), RegexOptions.IgnorePatternWhitespace);
            //var inheritRegex = new Regex(@"\boverride\.\b([^}]*)\}");
            var inherits = inheritRegex.Matches(file);
            foreach (var i in inherits)
            {
                var obj = new AnonymousFunction(i.ToString(), predefined);
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
    internal class LoopTracer
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
