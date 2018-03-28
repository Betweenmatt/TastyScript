using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Extensions;

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
        private List<IBaseFunction> importFStack = new List<IBaseFunction>();
        private List<EDefinition> importEStack = new List<EDefinition>();
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
            var onefile = ParseImports(file);
            _compileStack = ParseScopes(onefile, predefined);
            _compileStack.AddRange(predefined);
            _compileStack.AddRange(importFStack);
            ExtensionStack.AddRange(importEStack);
            StartScope(_compileStack);
        }
        private void ImportDlls(string path)
        {
            //wip
            throw new NotImplementedException();
            //Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            //Console.WriteLine(path);
            //var newpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            //Console.WriteLine(newpath);
            path = path.Replace("\r", "").Replace("\n", "");
            Assembly dll = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "lib/" + path);
            importFStack.AddRange(Utilities.GetPredefinedFunctions(new Assembly[] { dll }));
            importEStack.AddRange(Utilities.GetExtensions(new Assembly[] { dll }));

        }
        private string ParseImports(string file)
        {
            var output = file;
            //add imports
            var imports = new List<IBaseFunction>();
            var splitEndImport = file.Split(new string[] { "@end" }, StringSplitOptions.None);
            if (splitEndImport.Length > 1)
            {
                var splitImport = splitEndImport[0].Split(new string[] { "@import " }, StringSplitOptions.None);
                foreach(var x in splitImport)
                {
                    var ifile = x.Split(';')[0];
                    if (ifile != "")
                    {
                        var path = ifile.Replace("\'", "").Replace("\"", "");
                        if (!Files.ContainsKey(path))
                        {
                            try
                            {
                                if (path.Contains(".dll"))
                                {
                                    ImportDlls(path);
                                    Files.Add(path, path);
                                }
                                else
                                {
                                    var fileContents = Utilities.GetFileFromPath(path);
                                    Files.Add(path, fileContents);
                                    //add functions first
                                    output += ParseImports(fileContents);
                                }
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e);
                                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Error importing {file}."));
                            }
                        }
                        else
                        {
                            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                                $"Cannot import file: {path} because it has already been imported", file));
                        }
                    }
                }
            }
            return output;
        }
        private List<IBaseFunction> ParseScopes(string file, List<IBaseFunction> predefined)
        {
            var temp = new List<IBaseFunction>();
            //remove all comments
            var commRegex = new Regex(@"#([^\n]+)$", RegexOptions.Multiline);
            file = commRegex.Replace(file, "");

            //add functions first
            var _functionStack = new List<IBaseFunction>();
            var scopeRegex = new Regex(ScopeRegex(@"\bfunction\.\b"), RegexOptions.IgnorePatternWhitespace);
            var scopes = scopeRegex.Matches(file);
            foreach (var s in scopes)
            {
                _functionStack.Add(new AnonymousFunction(s.ToString()));
            }
            //add inherits second
            var _inheritStack = new List<IBaseFunction>();
            var inheritRegex = new Regex(ScopeRegex(@"\boverride\.\b"), RegexOptions.IgnorePatternWhitespace);
            var inherits = inheritRegex.Matches(file);
            var tempfunctionstack = new List<IBaseFunction>();
            tempfunctionstack.AddRange(_functionStack);
            tempfunctionstack.AddRange(predefined);
            for (var i = inherits.Count - 1; i >= 0; i--) 
            {
                var inherit = inherits[i];
                var obj = new AnonymousFunction(inherit.ToString(), tempfunctionstack);
                _inheritStack.Add(obj);
                tempfunctionstack.Insert(0,obj);
            }
            //add custom extensions
            var _extStack = new List<EDefinition>();
            var extRegex = new Regex(ScopeRegex(@"\bextension\.\b"), RegexOptions.IgnorePatternWhitespace);
            var exts = extRegex.Matches(file);
            for (var i = exts.Count - 1; i >= 0; i--)
            {
                var ext = exts[i];
                var cext = new CustomExtension();
                cext.FunctionReference = new AnonymousFunction(ext.ToString(), cext);
                _extStack.Add(cext);
            }
            ExtensionStack.AddRange(_extStack);
            //add async
            var _asyncStack = new List<IBaseFunction>();
            var asyncRegex = new Regex(ScopeRegex(@"\basync\.\b"), RegexOptions.IgnorePatternWhitespace);
            var asyncs = asyncRegex.Matches(file);
            foreach (var i in asyncs)
            {
                var obj = new AnonymousFunction(i.ToString());
                obj.Async = true;
                _asyncStack.Add(obj);
            }
            temp.AddRange(_inheritStack.Reverse<IBaseFunction>());
            temp.AddRange(_functionStack);
            temp.AddRange(_asyncStack);
            return temp;
        }

        [Obsolete]
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
                            var fileContents = Utilities.GetFileFromPath(path);
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
        [Obsolete]
        private List<IBaseFunction> GetScopes(string file, List<IBaseFunction> predefined)
        {
            var temp = new List<IBaseFunction>();
            //remove all comments
            var commRegex = new Regex(@"#([^\n]+)$",RegexOptions.Multiline);
            file = commRegex.Replace(file, "");

            //add functions first
            var scopeRegex = new Regex(ScopeRegex(@"\bfunction\.\b"),RegexOptions.IgnorePatternWhitespace);
            var scopes = scopeRegex.Matches(file);
            foreach (var s in scopes)
            {
                temp.Add(new AnonymousFunction(s.ToString()));
            }
            //add inherits second
            var _inheritStack = new List<IBaseFunction>();
            var inheritRegex = new Regex(ScopeRegex(@"\boverride\.\b"), RegexOptions.IgnorePatternWhitespace);
            var inherits = inheritRegex.Matches(file);
            foreach (var i in inherits)
            {
                var obj = new AnonymousFunction(i.ToString(), predefined);
                _inheritStack.Add(obj);
            }
            //add async
            var _asyncStack = new List<IBaseFunction>();
            var asyncRegex = new Regex(ScopeRegex(@"\basync\.\b"), RegexOptions.IgnorePatternWhitespace);
            var asyncs = asyncRegex.Matches(file);
            foreach (var i in asyncs)
            {
                var obj = new AnonymousFunction(i.ToString());
                obj.Async = true;
                _asyncStack.Add(obj);
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
            temp.AddRange(_asyncStack);
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
        private static int index = 0;
        public bool Break { get; private set; }
        public bool Continue { get; private set; }
        public int ID { get; }
        public LoopTracer()
        {
            ID = index++;
        }
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
