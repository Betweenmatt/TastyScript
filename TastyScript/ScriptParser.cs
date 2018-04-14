using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.ParserManager;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Tokens;
using TastyScript.IFunction.Function;
using TastyScript.IFunction.Extension;
using System.Text.RegularExpressions;
using TastyScript.TastyScript.Parser;
using System.Reflection;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction;

namespace TastyScript.TastyScript
{
    public class ScriptParser
    {
        private FunctionList _compileStack;
        private FunctionList importFStack = new FunctionList();
        private ExtensionList importEStack = new ExtensionList();
        public static BaseFunction HaltFunction { get; private set; }
        public static BaseFunction GuaranteedHaltFunction { get; private set; }

        public ScriptParser(string filename, string file)
        {
            Manager.LoopTracerStack = new ParserManager.Looping.LoopTracerList();
            FunctionStack.Clear();//clear this every run
            ExtensionStack.Clear();
            Manager.LoadedFileReference = new Dictionary<string, string>();
            Manager.LoadedFileReference.Add(filename, file);
            var onefile = ParseImports(file);
            ImportDlls("CoreFunctions.dll");
            ImportDlls("CoreExtensions.dll");
            
            References.PredefinedList = importFStack.List;

            _compileStack = new FunctionList(ParseScopes(onefile, References.PredefinedList));
            _compileStack.AddRange(importFStack.List);
            ExtensionStack.AddRange(importEStack.List);


            Manager.SetCancellationTokenSource();
            FunctionStack.AddRange(_compileStack.List);
            AnonymousTokenStack.Clear();
            GlobalVariableStack.Clear();
            GlobalVariableStack.AddRange(new List<Token>()
            {
            new Token("DateTime",()=>{return DateTime.Now.ToString(); }, "{0}", locked:true),
            new Token("Date",()=>{return DateTime.Now.ToShortDateString(); },"{0}", locked:true),
            new Token("Time",()=>{return DateTime.Now.ToShortTimeString(); },"{0}", locked:true),
            new TArray("GetVersion", StrVersion(),"{0}", locked:true),
            new Token("null","null","{0}", locked:true),
            new Token("True","True","{0}",locked:true),
            new Token("true","true","{0}",locked:true),
            new Token("False","False","{0}",locked:true),
            new Token("false","false","{0}",locked:true)
            });
            StartParse();
        }
        private string[] StrVersion()
        {
            var vers = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var spl = vers.Split('.');
            return spl;
        }
        private void ImportDlls(string path)
        {
            try
            {
                path = path.Replace("\r", "").Replace("\n", "");
                Assembly dll = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + path);
                importFStack.AddRange(GetPredefinedFunctions(new Assembly[] { dll }));
                importEStack.AddRange(GetExtensions(new Assembly[] { dll }));
            }
            catch
            {
                Manager.Throw($"There was an unexpected error importing {path}.", ExceptionType.SystemException);
            }
        }
        private string ParseImports(string file)
        {
            var output = file;
            //add imports
            var imports = new FunctionList();
            var splitEndImport = file.Split(new string[] { "@end" }, StringSplitOptions.None);
            if (splitEndImport.Length > 1)
            {
                var splitImport = splitEndImport[0].Split(new string[] { "@import " }, StringSplitOptions.None);
                foreach (var x in splitImport)
                {
                    var ifile = x.Split(';')[0];
                    if (ifile != "")
                    {
                        Manager.SetCurrentParsedLine(ifile);
                        var path = ifile.Replace("\'", "").Replace("\"", "");
                        if (!Manager.LoadedFileReference.ContainsKey(path))
                        {
                            try
                            {
                                if (path.Contains(".dll"))
                                {
                                    ImportDlls(path);
                                    Manager.LoadedFileReference.Add(path, path);
                                }
                                else
                                {
                                    var fileContents = Utilities.GetFileFromPath(path);
                                    Manager.LoadedFileReference.Add(path, fileContents);
                                    //add functions first
                                    output += ParseImports(fileContents);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                Manager.Throw($"Error importing {file}.", ExceptionType.SystemException);
                            }
                        }
                        else
                        {
                            Manager.Throw($"Cannot import file: {path} because it has already been imported", ExceptionType.SystemException);
                        }
                    }
                }
            }
            return output;
        }
        private List<BaseFunction> ParseScopes(string file, List<BaseFunction> predefined)
        {
            var temp = new List<BaseFunction>();
            //remove all comments
            var commRegex = new Regex(@"#([^\n]+)$", RegexOptions.Multiline);
            file = commRegex.Replace(file, "");

            //add functions first
            var _functionStack = new List<BaseFunction>();
            var scopeRegex = new Regex(Utilities.ScopeRegex(@"\bfunction\.\b"), RegexOptions.IgnorePatternWhitespace);
            var scopes = scopeRegex.Matches(file);
            foreach (var s in scopes)
            {
                Manager.SetCurrentParsedLine(s.ToString());
                _functionStack.Add(new ParsedFunction(s.ToString()));
            }
            //add inherits second
            var _inheritStack = new List<BaseFunction>();
            var inheritRegex = new Regex(Utilities.ScopeRegex(@"\boverride\.\b"), RegexOptions.IgnorePatternWhitespace);
            var inherits = inheritRegex.Matches(file);
            var tempfunctionstack = new List<BaseFunction>();
            tempfunctionstack.AddRange(_functionStack);
            tempfunctionstack.AddRange(predefined);
            for (var i = inherits.Count - 1; i >= 0; i--)
            {
                var inherit = inherits[i];
                Manager.SetCurrentParsedLine(inherit.ToString());
                var obj = new ParsedFunction(inherit.ToString(), tempfunctionstack);
                _inheritStack.Add(obj);
                tempfunctionstack.Insert(0, obj);
            }
            //add custom extensions
            var _extStack = new List<BaseExtension>();
            var extRegex = new Regex(Utilities.ScopeRegex(@"\bextension\.\b"), RegexOptions.IgnorePatternWhitespace);
            var exts = extRegex.Matches(file);
            for (var i = exts.Count - 1; i >= 0; i--)
            {
                var ext = exts[i];
                var cext = new CustomExtension();
                Manager.SetCurrentParsedLine(ext.ToString());
                cext.FunctionReference = new ParsedFunction(ext.ToString(), cext);
                _extStack.Add(cext);
            }
            ExtensionStack.AddRange(_extStack);
            temp.AddRange(_inheritStack.Reverse<BaseFunction>());
            temp.AddRange(_functionStack);
            return temp;
        }

        private void StartParse()
        {
            new Directives();
            var startScope = FunctionStack.First("Start");
            var awakeScope = FunctionStack.First("Awake");
            HaltFunction = FunctionStack.First("Halt");
            GuaranteedHaltFunction = FunctionStack.First("GuaranteedHalt");

            if (awakeScope != null)
            {
                var awakecollection = FunctionStack.Where("Awake");
                foreach (var x in awakecollection)
                {
                    x.TryParse(null);
                }
            }
            
            if (startScope == null)
                Manager.Throw($"Your script is missing a 'Start' function.", ExceptionType.SystemException);
            var startCollection = FunctionStack.Where("Start");
            if (startCollection.Count() != 2)
                Manager.Throw($"There must be one `Start` function. There are {((startCollection.Count() - 2 < 0) ? 0 : startCollection.Count() -2)} `Start` functions in this script.", ExceptionType.SystemException);
            var startIndex = FunctionStack.IndexOf(startScope);
            FunctionStack.RemoveAt(startIndex);

            new TFunction(startScope, Manager.StartArgs).TryParse();
        }
        //uses reflection to get all the BaseFunction classes with the attribute [Function]
        public static List<BaseFunction> GetPredefinedFunctions(Assembly[] asmbly = null)
        {
            if (asmbly == null)
                asmbly = AppDomain.CurrentDomain.GetAssemblies();
            List<BaseFunction> temp = new List<BaseFunction>();
            string definedIn = typeof(Function).Assembly.GetName().Name;
            foreach (Assembly assembly in asmbly)
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (System.Type type in assembly.GetTypes())
                        if (type.GetCustomAttributes(typeof(Function), true).Length > 0)
                        {
                            var func = assembly.GetType(type.ToString());
                            var inst = Activator.CreateInstance(func) as BaseFunction;
                            var attt = type.GetCustomAttribute(typeof(Function), true) as Function;
                            inst.SetProperties(attt.Name, attt.ExpectedArgs, attt.Invoking, attt.Sealed, attt.Obsolete, attt.Alias, attt.IsAnonymous);
                            if (!attt.Depricated)
                                temp.Add(inst);
                        }
            return temp;
        }
        public static List<BaseExtension> GetExtensions(Assembly[] asmbly = null)
        {
            if (asmbly == null)
                asmbly = AppDomain.CurrentDomain.GetAssemblies();
            List<BaseExtension> temp = new List<BaseExtension>();
            string definedIn = typeof(Extension).Assembly.GetName().Name;
            foreach (Assembly assembly in asmbly)
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (System.Type type in assembly.GetTypes())
                        if (type.GetCustomAttributes(typeof(Extension), true).Length > 0)
                        {
                            var func = assembly.GetType(type.ToString());
                            var inst = Activator.CreateInstance(func) as BaseExtension;
                            var attt = type.GetCustomAttribute(typeof(Extension), true) as Extension;
                            inst.SetProperties(attt.Name, attt.ExpectedArgs, attt.Invoking, attt.Obsolete, attt.VariableExtension, attt.Alias);
                            if (!attt.Depricated)
                                temp.Add(inst);
                        }
            return temp;
        }
    }
}
