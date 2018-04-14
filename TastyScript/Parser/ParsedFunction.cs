using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TastyScript.IFunction;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Function;
using TastyScript.ParserManager;

namespace TastyScript.TastyScript.Parser
{
    internal class ParsedFunction : BaseFunction
    {
        public ParsedFunction()
        {
            ProvidedArgs = new TokenList();
            LocalVariables = new TokenList();
            GetUID();
        }
        //standard constructor
        public ParsedFunction(string value) : this()
        {
            Name = value.Split('.')[1].Split('(')[0];
            var b = References.PredefinedList.FirstOrDefault(f => f.Name == Name);
            if (b != null)
                if (b.IsSealed == true)
                {
                    Manager.Throw($"Invalid Operation. Cannot create a new instance of a Sealed function: {Name}.", ExceptionType.SystemException);
                    return;
                }
            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Utilities.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new ParsedFunction(a.ToString(), true, _base);
                func.Base = _base;
                FunctionStack.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            ParseDirectives(value);
        }
        //this constructor is when function is anonomysly named
        public ParsedFunction(string value, bool anon, BaseFunction callerBase) : this()
        {
            Base = callerBase;
            IsAnonymous = true;
            //get top level anonymous functions before everything else
            value = value.Substring(1);
            var anonRegex = new Regex(Utilities.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new ParsedFunction(a.ToString(), true, _base);
                FunctionStack.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            Value = value;
            Name = "AnonymousFunction" + Manager.AnonymousFunctionIndex;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            ParseDirectives(value);
            //Name = value.Split('.')[1].Split('(')[0];
        }
        //this is the constructor used when function is an override
        public ParsedFunction(string value, List<BaseFunction> predefined) : this()
        {
            IsOverride = true;
            Name = value.Split('.')[1].Split('(')[0];

            var b = predefined.FirstOrDefault(f => f.Name == Name);
            if (b == null)
            {
                Manager.Throw($"Unexpected error. Function failed to override: {Name}.", ExceptionType.SystemException);
                return;
            }
            if (b.IsSealed == true)
            {
                Manager.Throw($"Invalid Operation. Cannot override Sealed function: {Name}.", ExceptionType.SystemException);
                return;
            }
            Base = b;
            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Utilities.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new ParsedFunction(a.ToString(), true, _base);
                FunctionStack.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            ParseDirectives(value);
        }
        //the construction for custom extensions
        public ParsedFunction(string value, CustomExtension parent) : this()
        {
            Name = value.Split('.')[1].Split('(')[0];

            //get top level anonymous functions before everything else
            var anonRegex = new Regex(Utilities.ScopeRegex(@"=>"), RegexOptions.IgnorePatternWhitespace);
            var anonRegexMatches = anonRegex.Matches(value);
            foreach (var a in anonRegexMatches)
            {
                var func = new ParsedFunction(a.ToString(), true, _base);
                func.Base = _base;
                FunctionStack.Add(func);
                value = value.Replace(a.ToString(), $"\"{func.Name}\"");
            }
            //
            Value = value;
            ExpectedArgs = value.Split('(')[1].Split(')')[0].Split(',');
            var type = ExpectedArgs.ElementAtOrDefault(0);
            if (type != null && type == "this variable")
            {
                parent.SetProperties(Name, new string[] { }, false, false, true, new string[] { });
            }
            else if (type != null && type == "this function")
            {
                parent.SetProperties(Name, new string[] { }, false, false, false, new string[] { });
            }
            else
            {
                Manager.Throw("[287]Custom extension must have input parameter of `this variable` or `this function`", ExceptionType.SystemException);
            }
            //replace this from the parameter, so it can be called by just `function` or `variable` at runtime
            //the `this` is only there for definition, to direct the extension parser to look for this extension
            //in the variable lot or not.
            ExpectedArgs[0] = ExpectedArgs[0].Replace("this ", "");
            ParseDirectives(value);
        }
        private void ParseDirectives(string value)
        {
            if (value.Contains(".") && value.Contains("{"))
            {
                var findPretext = value.Split('.')[1].Split('{')[0];
                if (findPretext.Contains(":"))
                {
                    var dirs = findPretext.Split(':');
                    if (dirs.Length > 0)
                    {
                        var temp = new Dictionary<string, string>();
                        for (var i = 1; i < dirs.Length; i++)
                        {
                            var preParen = dirs[i].Split('(');
                            if (preParen[0].Contains("Where"))
                            {
                                var inParen = preParen[1].Split(')')[0];
                                var postParen = preParen[1].Split('=')[1];
                                temp[inParen] = postParen;
                            }
                        }
                        Directives = temp;
                    }
                }
            }
        }
        protected override void TryParse()
        {
            var findFor = Extensions.First("For");
            if (findFor != null)
            {
                //if for extension exists, reroutes this tryparse method to the loop version without the for check
                ForExtension(findFor);
                return;
            }
            TryParse(true);
        }
        protected override void TryParse(bool forFlag)
        {
            AssignParameters();
            var guts = Value.Split('{')[1].Split('}');
            var lines = guts[0].Split(';');
            foreach (var l in lines)
                if (!Manager.IsScriptStopping && !ReturnFlag)
                    new Line(l, this);
            //clear local var stack after use
            LocalVariables = new TokenList();
        }
    }
}
