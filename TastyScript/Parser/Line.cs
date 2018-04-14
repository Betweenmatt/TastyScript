using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Function;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.TastyScript.Parser
{
    internal class Line
    {
        private BaseFunction _reference;
        public string Value { get; private set; }

        public Line(string val, BaseFunction reference)
        {
            Manager.SetCurrentParsedLine(val);
            Value = val;
            _reference = reference;
            WalkTree(val);

        }
        private string ReplaceAllNotInStringWhiteSpace(string value)
        {
            bool level = false;
            string output = "";
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '\"')
                {
                    level = (level) ? false : true;
                }
                if (level)
                {
                    //these symbols are for preserving strings.
                    switch (value[i])
                    {
                        case ('+'):
                            output += "&plus;";
                            break;
                        case ('-'):
                            output += "&neg;";
                            break;
                        case ('='):
                            output += "&eq;";
                            break;
                        case ('%'):
                            output += "&per;";
                            break;
                        case ('$'):
                            output += "&dollar;";
                            break;
                        case ('!'):
                            output += "&expl;";
                            break;
                        case ('('):
                            output += "&lparen;";
                            break;
                        case (')'):
                            output += "&rparen;";
                            break;
                        case ('['):
                            output += "&lbrack;";
                            break;
                        case (']'):
                            output += "&rbrack;";
                            break;
                        case ('{'):
                            output += "&lbrace;";
                            break;
                        case ('}'):
                            output += "&rbrace;";
                            break;
                        case ('<'):
                            output += "&lchev;";
                            break;
                        case ('>'):
                            output += "&rchev;";
                            break;
                        case ('.'):
                            output += "&period;";
                            break;
                        case ('&'):
                            output += "&amp;";
                            break;
                        case (','):
                            output += "&coma;";
                            break;
                        case ('\r'):
                            output += "&CR;";
                            break;
                        case ('\n'):
                            output += "&LF;";
                            break;
                        case ('\t'):
                            output += "&tab;";
                            break;
                        default:
                            output += value[i];
                            break;
                    }
                }
                else
                {
                    if (value[i] != ' ' && value[i] != '\n' && value[i] != '\r' && value[i] != '\t')
                    {
                        output += value[i];
                    }
                }
            }
            return output;
        }
        private void WalkTree(string value)
        {
            value = RuntimeDebugger(value);
            value = value.ReplaceFirst("var ", "var%");
            value = ReplaceAllNotInStringWhiteSpace(value);
            value = ParseMathExpressions(value);
            if (value == null)
                return;
            value = ParseArrays(value);
            value = ParseParameters(value);
            value = ParseStrings(value);
            value = ParseNumbers(value);
            value = value.Replace(".", "<-").Replace("\n", "").Replace("\r", "").Replace("\t", "");
            //check for empty lines
            var wscheck = new Regex(@"^\s*$");
            var wscheckk = wscheck.IsMatch(value);
            if (wscheckk)
                return;// temp;
                       //
                       //get var extensions before normal extensions


            //vars here
            if (value.Contains("var%"))
            {
                value = EvaluateVar(value);
                if (value == null || value == "")
                    return;// temp;
            }
            //try extension sweep after vars instead of before
            //value = EvaluateVarExtensions(value);
            var ext = ParseExtensions(value);
            if (ext == null)
                return;
            //
            //temp =
            ParseFunctions(value, ext);
            return;// temp;
        }
        private string ParseStrings(string value)
        {
            var stringTokenRegex = new Regex("\"([^\"\"]*)\"", RegexOptions.Multiline);
            var strings = stringTokenRegex.Matches(value);
            foreach (var x in strings)
            {
                string tokenname = "{AnonGeneratedToken" + AnonymousTokenStack.AnonymousTokenIndex + "}";
                var tstring = new Token(tokenname, Regex.Replace(x.ToString(), "\"", ""), Value);
                value = value.Replace(x.ToString(), tokenname);

                AnonymousTokenStack.Add(tstring);
            }
            return value;
        }
        private string ParseNumbers(string value)
        {
            var numberTokenRegex = new Regex(@"\b-*[0-9\.]+\b", RegexOptions.Multiline);
            var numbers = numberTokenRegex.Matches(value);
            foreach (var x in numbers)
            {
                string tokenname = "{AnonGeneratedToken" + AnonymousTokenStack.AnonymousTokenIndex + "}";
                double output = 0;
                var nofail = double.TryParse(x.ToString(), out output);
                if (nofail)
                {
                    AnonymousTokenStack.Add(new Token(tokenname, output.ToString(), Value));
                    //do this regex instead of a blind replace to fix the above issue. 
                    //NOTE this fix may break decimal use in some situations!!!!
                    var indvRegex = (@"\b-*" + x + @"\b");
                    var regex = new Regex(indvRegex);
                    value = regex.Replace(value, tokenname);
                }
            }
            return value;
        }
        private string ParseMathExpressions(string value)
        {
            var mathexpRegex = new Regex(@"\[([^\[\]]*)\]", RegexOptions.Multiline);
            var mathexp = mathexpRegex.Matches(value);
            foreach (var x in mathexp)
            {
                var input = x.ToString().Replace("[", "").Replace("]", "").Replace(" ", "");
                if (input != null && input != "")
                {
                    string tokenname = "{AnonGeneratedToken" + AnonymousTokenStack.AnonymousTokenIndex + "}";
                    double? exp = MathExpression(input);
                    if (exp == null)
                        return null;
                    AnonymousTokenStack.Add(new Token(tokenname, exp.ToString(), Value));
                    value = value.Replace(x.ToString(), tokenname);
                }
            }
            return value;
        }
        private string ParseParameters(string value)
        {
            string val = value;
            //first we have to find all the arrays
            var regstr = @"(?:(?:\((?>[^()]+|\((?<number>)|\)(?<-number>))*(?(number)(?!))\))|{^()\))+";
            var arrayRegex = new Regex(regstr, RegexOptions.Multiline);
            //var arrayRegex = new Regex(@"\(([^()]*)\)", RegexOptions.Multiline);
            var arrayMatches = arrayRegex.Matches(val);
            foreach (var a in arrayMatches)
            {
                var param = a.ToString().Substring(1, a.ToString().Length - 2);
                if (param.Contains("(") && param.Contains(")"))
                {
                    param = ParseArrays(param);
                    param = ParseParameters(param);
                    param = param.Replace(".", "<-").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                    param = EvaluateVarExtensions(param);

                    var fext = ParseExtensions(param);
                    var fcheckSplit = param.Split(new string[] { "->" }, StringSplitOptions.None);
                    var fcheck = FunctionStack.First(fcheckSplit[0]);
                    if (fcheck != null)
                        param = ParseFunctions(param, fext);

                }
                string tokenname = "{AnonGeneratedToken" + AnonymousTokenStack.AnonymousTokenIndex + "}";

                var compCheck = ComparisonCheck(param);
                if (compCheck != "")
                {
                    AnonymousTokenStack.Add(new Token(tokenname, compCheck, val));
                }
                else
                {
                    var commaRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    var commaSplit = commaRegex.Split(param);
                    var tokens = GetTokens(commaSplit, true);
                    //make sure values are being collected and not tokens
                    if (tokens.Count > 0)
                    {
                        for (int i = 0; i < commaSplit.Length; i++)
                        {
                            var obj = tokens.FirstOrDefault(f => f.Name == commaSplit[i]);
                            if (obj != null)
                                commaSplit[i] = obj.Value;
                        }
                        param = string.Join(",", commaSplit);
                    }
                    AnonymousTokenStack.Add(new Token(tokenname, param, val));
                }
                val = val.Replace(a.ToString(), "->" + tokenname + "|");
            }
            return val;
        }
        private string ParseArrays(string value)
        {
            string val = value;
            if (val.Contains("array("))
            {
                //first we have to find all the arrays
                var rstr = @"(?:(?:\barray\((?>[^()]+|\((?<number>)|\)(?<-number>))*(?(number)(?!))\))|{^()\))+";
                //var arrayRegex = new Regex(@"\barray\(([^()]*)\)", RegexOptions.Multiline);
                var arrayRegex = new Regex(rstr, RegexOptions.Multiline);
                var arrayMatches = arrayRegex.Matches(val);
                foreach (var a in arrayMatches)
                {
                    var param = a.ToString().Substring(6, a.ToString().Length - 7);
                    if (param.Contains("(") && param.Contains(")"))
                    {
                        param = ParseArrays(param);
                        param = ParseParameters(param);
                        param = param.Replace(".", "<-").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        param = EvaluateVarExtensions(param);

                        var fext = ParseExtensions(param);
                        var fcheckSplit = param.Split(new string[] { "->" }, StringSplitOptions.None);
                        var fcheck = FunctionStack.First(fcheckSplit[0]);
                        if (fcheck != null)
                            param = ParseFunctions(param, fext);

                    }
                    string tokenname = "{AnonGeneratedToken" + AnonymousTokenStack.AnonymousTokenIndex + "}";
                    //var param = a.ToString().Replace("array(", "").Replace(")", "");
                    var compCheck = ComparisonCheck(param);
                    if (compCheck != "")
                    {
                        AnonymousTokenStack.Add(new Token(tokenname, compCheck, val));
                    }
                    else
                    {
                        var commaRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        var commaSplit = commaRegex.Split(param);
                        var tokens = GetTokens(commaSplit, true);
                        //make sure values are being collected and not tokens
                        if (tokens.Count > 0)
                        {
                            for (int i = 0; i < commaSplit.Length; i++)
                            {
                                var obj = tokens.FirstOrDefault(f => f.Name == commaSplit[i]);
                                if (obj != null)
                                    commaSplit[i] = obj.Value;
                            }
                            param = string.Join(",", commaSplit);
                        }
                        AnonymousTokenStack.Add(new TArray(tokenname, commaSplit, val));
                    }
                    val = val.Replace(a.ToString(), "" + tokenname + "");
                }
            }
            return val;
        }
        private List<BaseExtension> ParseExtensions(string value)
        {
            List<BaseExtension> temp = new List<BaseExtension>();
            if (value.Contains("<-"))
            {
                string val = value;
                var firstSplit = val.Split(new string[] { "<-" }, StringSplitOptions.None);
                for (int i = 0; i < firstSplit.Length; i++)
                {
                    if (i == 0)
                        continue;
                    var first = firstSplit[i];
                    var secondSplit = first.Split(new string[] { "->" }, StringSplitOptions.None);
                    if (secondSplit.Length != 2)
                    {
                        Manager.Throw("[160]Extensions must provide arguments", ExceptionType.SyntaxException);
                        return null;
                    }
                    var original = ExtensionStack.First(secondSplit[0]);
                    if (original == null)
                    {
                        Manager.Throw($"[310]Cannot find extension [{secondSplit[0]}]", ExceptionType.SyntaxException);
                        return null;
                    }
                    var clone = DeepCopy(original);
                    var param = GetTokens(new string[] { secondSplit[1].Replace("|", "") });
                    if (param.Count != 1)
                    {
                        Manager.Throw("[166]Extensions must provide arguments",ExceptionType.SyntaxException);
                        return null;
                    }
                    if (clone.IsInvoking)
                    {
                        var invokeFuncName = param[0].ToString();
                        if (invokeFuncName.Contains("AnonymousFunction"))
                        {
                            var functionToInvoke = FunctionStack.First(invokeFuncName.Replace("\"", ""));

                            if (functionToInvoke != null)
                            {
                                var args = GetTokens(functionToInvoke.ExpectedArgs, true, true);
                                List<string> argsarr = new List<string>();
                                foreach (var x in args)
                                {
                                    argsarr.Add(x.ToString());
                                }
                                clone.SetInvokeProperties(argsarr.ToArray());
                            }
                        }
                    }
                    clone.SetArguments(param[0].ToString());
                    temp.Add(clone);
                }
            }
            return temp;
        }
        private string ParseFunctions(string value, List<BaseExtension> ext, bool safelook = false)
        {
            string val = value;
            var firstSplit = value.Split('|')[0];
            var secondSplit = firstSplit.Split(new string[] { "->" }, StringSplitOptions.None);
            var func = FunctionStack.First(secondSplit[0]);
            if (func == null)
            {
                if (safelook)
                    return "";
                else
                {
                    Manager.Throw($"[181]Cannot find function [{secondSplit[0]}]",ExceptionType.SyntaxException);
                    return null;
                }
            }
            //get args
            var param = GetTokens(new string[] { secondSplit[1] });
            if (param.Count != 1)
            {
                Manager.Throw("[185]Extensions must provide arguments", ExceptionType.SyntaxException);
                return null;
            }
            if (func.IsInvoking)
            {
                var invokeFuncName = param[0].ToString();
                if (invokeFuncName.Contains("AnonymousFunction"))
                {
                    var functionToInvoke = FunctionStack.First(invokeFuncName.Replace("\"", ""));
                    if (functionToInvoke != null)
                    {
                        var args = GetTokens(functionToInvoke.ExpectedArgs, true, true);
                        List<string> argsarr = new List<string>();
                        foreach (var x in args)
                        {
                            argsarr.Add(x.ToString());
                        }
                        func.SetInvokeProperties(argsarr.ToArray(), _reference.LocalVariables.List, _reference.ProvidedArgs.List);
                    }
                }
            }
            var caller = new TFunction(func, _reference, new ExtensionList(ext), param[0].ToString());
            //do the whole returning thing
            var getret = Parse(caller);
            if (getret != null)
            {
                string tokenname = "{AnonGeneratedToken" + AnonymousTokenStack.AnonymousTokenIndex + "}";
                getret.SetName(tokenname);
                AnonymousTokenStack.Add(getret);
                val = tokenname;
                return val;
            }
            return "null";
        }

        private List<Token> GetTokens(string[] names, bool safe = false, bool returnInput = false)
        {
            List<Token> temp = new List<Token>();
            foreach (var n in names)
            {
                var stripws = n.Replace(" ", "");
                var tryParams = _reference.ProvidedArgs.First(stripws);
                if (tryParams != null)
                {
                    temp.Add(new Token(stripws, tryParams.ToString(), Value));
                    continue;
                }
                var tryLocal = _reference.LocalVariables.First(stripws);
                if (tryLocal != null)
                {
                    temp.Add(new Token(stripws, tryLocal.ToString(), Value));
                    continue;
                }
                var tryGlobal = GlobalVariableStack.First(stripws);
                if (tryGlobal != null)
                {
                    temp.Add(new Token(stripws, tryGlobal.ToString(), Value));
                    continue;
                }
                if (stripws.Contains("{AnonGeneratedToken"))
                {
                    var tryAnon = AnonymousTokenStack.First(stripws);
                    if (tryAnon != null)
                    {
                        temp.Add(new Token(stripws, tryAnon.ToString(), Value));
                        continue;
                    }
                    else
                    {
                        Manager.Throw("[441]Unexpected error finding token.", ExceptionType.SyntaxException);
                        return null;
                    }
                }
                if (returnInput)
                {
                    //temp.Add(new Token(stripws, stripws, Value));
                    double number = 0;
                    bool isNumeric = double.TryParse(stripws, out number);
                    if (isNumeric)
                        temp.Add(new Token(stripws, stripws, Value));
                    else if (stripws.Contains("\""))
                        temp.Add(new Token(stripws, stripws, Value));
                    else
                        temp.Add(new Token(stripws, "null", Value));
                }
            }

            if (temp.Count == 0 && !safe)
            {
                //throw new Exception();
                Manager.Throw($"Cannot find tokens [{string.Join(",", names)}]", ExceptionType.SyntaxException);
                return null;
            }
            return temp;
        }
        private double? MathExpression(string expression)
        {
            string exp = expression;
            //get vars and params out of the expression

            var varRegex = new Regex(@"\w[A-Za-z]*\d*");
            var varRegexMatches = varRegex.Matches(exp);
            foreach (var x in varRegexMatches)
            {
                var tok = GetTokens(new string[] { x.ToString() }, true);

                var tokfirst = tok.FirstOrDefault(f => f != null);
                if (tokfirst != null)
                {
                    exp = exp.Replace(x.ToString(), tokfirst.ToString());
                }
            }
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("expression", typeof(string), exp);
                DataRow row = table.NewRow();
                table.Rows.Add(row);
                return double.Parse((string)row["expression"]);
            }
            catch (Exception e)
            {
                Manager.Throw($"[331]Unexpected error with mathematical expression:\n{e.Message}", ExceptionType.SyntaxException);
                return null;
            }
        }
        #region Comparison
        enum Operator { EQ, NOTEQ, GT, LT, GTEQ, LTEQ, NOT, NULL, NOTNULL }
        private string SingleSideComparisonCheck(string line)
        {
            string output = "";
            if (line.Contains("!?"))
                output = FindSingleSideComparisonOperation(Operator.NOTNULL, line);
            else if (line.Contains("!"))
                output = FindSingleSideComparisonOperation(Operator.NOT, line);
            else if (line.Contains("?"))
                output = FindSingleSideComparisonOperation(Operator.NULL, line);
            return output;
        }
        private string FindSingleSideComparisonOperation(Operator op, string line)
        {
            string output = "";
            string opString = "";
            switch (op)
            {
                case (Operator.NOTNULL):
                    opString = "!?";
                    break;
                case (Operator.NOT):
                    opString = "!";
                    break;
                case (Operator.NULL):
                    opString = "?";
                    break;
            }
            var splitop = line.Split(new string[] { opString }, StringSplitOptions.None);
            var lr = GetTokens(new string[] { splitop[1] }, true, true);
            if (lr.Count != 1)
            {
                Manager.Throw("One side operators can only have 1 token.", ExceptionType.SyntaxException);
                return null;
            }
            var token = lr[0].ToString().Replace("\"", "");
            try
            {
                switch (op)
                {
                    case (Operator.NOTNULL):
                        output = (token != null && token.ToString() != "" && token.ToString() != "null")
                            ? "True" : "False";
                        break;
                    case (Operator.NULL):
                        output = (token == null || token.ToString() == "" || token.ToString() == "null")
                            ? "True" : "False";
                        break;
                    case (Operator.NOT):
                        output = (token != null && (token.ToString() == "false" || token.ToString() == "False"))
                            ? "True" : "False";
                        break;
                }
            }
            catch { Manager.Throw($"Unexpected input: {line}", ExceptionType.SyntaxException); return null; }
            return output;
        }
        private string ComparisonCheck(string line)
        {
            string output = "";
            if (line.Contains("=="))
                output = FindOperation(Operator.EQ, line);
            else if (line.Contains("!="))
                output = FindOperation(Operator.NOTEQ, line);
            else if (line.Contains(">="))
                output = FindOperation(Operator.GTEQ, line);
            else if (line.Contains("<="))
                output = FindOperation(Operator.LTEQ, line);
            else if (line.Contains(">"))
                output = FindOperation(Operator.GT, line);
            else if (line.Contains("<"))
                output = FindOperation(Operator.LT, line);
            else
                output = SingleSideComparisonCheck(line);
            return output;
        }
        //the heavy lifting for comparison check
        private string FindOperation(Operator op, string line)
        {
            string output = "";
            string opString = "";
            switch (op)
            {
                case (Operator.EQ):
                    opString = "==";
                    break;
                case (Operator.NOTEQ):
                    opString = "!=";
                    break;
                case (Operator.GT):
                    opString = ">";
                    break;
                case (Operator.LT):
                    opString = "<";
                    break;
                case (Operator.GTEQ):
                    opString = ">=";
                    break;
                case (Operator.LTEQ):
                    opString = "<=";
                    break;
            }
            var splitop = line.Split(new string[] { opString }, StringSplitOptions.None);
            var lr = GetTokens(new string[] { splitop[0], splitop[1] }, true, true);
            if (lr.Count != 2)
            {
                Manager.Throw("There must be one left-hand and one right-hand in comparison objects.", ExceptionType.SyntaxException);
                return null;
            }
            var left = lr[0].ToString().Replace("\"", "");
            var right = lr[1].ToString().Replace("\"", "");
            try
            {
                switch (op)
                {
                    case (Operator.EQ):
                        output = (left == right)
                            ? "True" : "False";
                        break;
                    case (Operator.NOTEQ):
                        output = (left != right)
                            ? "True" : "False";
                        break;
                    case (Operator.GT):
                        output = (double.Parse(left) > double.Parse(right))
                            ? "True" : "False";
                        break;
                    case (Operator.LT):
                        output = (double.Parse(left) < double.Parse(right))
                            ? "True" : "False";
                        break;
                    case (Operator.GTEQ):
                        output = (double.Parse(left) >= double.Parse(right))
                            ? "True" : "False";
                        break;
                    case (Operator.LTEQ):
                        output = (double.Parse(left) <= double.Parse(right))
                            ? "True" : "False";
                        break;
                }
            }
            catch
            {
                Manager.Throw($"Unexpected input: {line}", ExceptionType.SyntaxException);
                return null;
            }

            return output;
        }
        //this rips off the comparison check, since the concept is the same.
        private void CompareFail(string line)
        {
            Manager.Throw($"Can not compare more or less than 2 values", ExceptionType.SyntaxException);
            return;
        }
        #endregion

        public static T DeepCopy<T>(T obj) => obj.Copy<T>();

        private string EvaluateVarExtensions(string val)
        {
            var value = val;
            if (val.Contains("<-"))
            {
                //get extensions
                var ext = ParseExtensions(value);
                //get object to be extended
                var strip = value.Split(new string[] { "<-" }, StringSplitOptions.None);
                var objLeft = strip[0];
                var objRemoveKeywords = objLeft.Split(new string[] { "+=", "-=", "++", "--", "=" }, StringSplitOptions.RemoveEmptyEntries);
                var obj = objRemoveKeywords[objRemoveKeywords.Length - 1];
                var objVar = GetTokens(new string[] { obj.Replace("|", "") }, true).FirstOrDefault();
                if (objVar != null)
                {
                    foreach (var e in ext)
                    {
                        if (e.IsVariableExtension)
                        {
                            string tokenname = "{AnonGeneratedToken" + AnonymousTokenStack.AnonymousTokenIndex + "}";
                            var extobj = e.Extend(objVar);
                            if (extobj == null)
                            {
                                Manager.Throw($"[610]Unexpected error compiling extension [{e.Name}]", ExceptionType.SyntaxException);
                                return null;
                            }
                            extobj.SetName(tokenname);
                            AnonymousTokenStack.Add(extobj);
                            value = value.Replace(obj + "<-" + strip[1], tokenname);
                        }
                    }
                }
            }
            return value;
        }
        private string EvaluateVar(string value)
        {
            //get the var scope
            List<Token> varList = null;
            if (value.Contains("$var%"))
                varList = GlobalVariableStack.List;
            else if (value.Contains("var%"))
                varList = _reference.LocalVariables.List;
            if (varList == null)
            {
                Manager.Throw($"[244]Unexpected error occured.", ExceptionType.SyntaxException);
                return null;
            }
            //assign based on operator

            var strip = value.Replace("$", "").Replace("var%", "");
            string[] assign = default(string[]);
            if (strip.Contains("++"))
                assign = strip.Split(new string[] { "++" }, StringSplitOptions.None);
            else if (strip.Contains("--"))
                assign = strip.Split(new string[] { "--" }, StringSplitOptions.None);
            else if (strip.Contains("+="))
                assign = strip.Split(new string[] { "+=" }, StringSplitOptions.None);
            else if (strip.Contains("-="))
                assign = strip.Split(new string[] { "-=" }, StringSplitOptions.None);
            else if (strip.Contains("="))
                assign = strip.Split(new string[] { "=" }, StringSplitOptions.None);

            //get the left hand
            var leftHand = assign[0].Replace(" ", "");
            var varRef = varList.FirstOrDefault(f=>f.Name == leftHand);
            if (varRef != null && varRef.IsLocked)
            {
                Manager.Throw($"[282]Cannot re-assign a sealed variable!", ExceptionType.SyntaxException);
                return null;
            }
            //one sided assignment
            if (strip.Contains("++") || strip.Contains("--"))
            {
                if (varRef == null)
                {
                    Manager.Throw($"[269]Cannot find the left hand variable.", ExceptionType.SyntaxException);
                    return null;
                }
                double numOut = 0;
                double.TryParse(varRef.ToString(), out numOut);
                if (strip.Contains("++"))
                    numOut++;
                else
                    numOut--;
                varRef.SetValue(numOut.ToString());
                return "";
            }
            Token token = null;
            var rightHand = assign[1].Replace(" ", "");
            var parts = rightHand.Split('+');
            string output = "";
            foreach (var p in parts)
            {
                var x = p;
                if (x.Contains("->"))
                {
                    var fext = ParseExtensions(x);
                    var fcheckSplit = x.Split(new string[] { "->" }, StringSplitOptions.None);
                    var fcheck = FunctionStack.First(fcheckSplit[0]);
                    if (fcheck != null)
                        x = ParseFunctions(x, fext);
                }
                if (x.Contains("<-"))
                    x = EvaluateVarExtensions(x);
                if (x == null || x == "" || x == " ")
                {
                    Manager.Throw($"[688]Right hand must be a value.", ExceptionType.SyntaxException);
                    return null;
                }
                var prentoken = GetTokens(new string[] { x });
                if (prentoken == null)
                    return null;
                var ntoken = prentoken.ElementAtOrDefault(0);
                if (ntoken == null)
                {
                    Manager.Throw($"[692]Right hand must be a value.", ExceptionType.SyntaxException);
                    return null;
                }
                output += ntoken.ToString();
            }
            token = new Token("concatination", output, Value);
            rightHand = output;
            //}
            if (token == null)
            {
                Manager.Throw($"[699]Right hand must be a value.", ExceptionType.SyntaxException);
                return null;
            }
            if (strip.Contains("+=") || strip.Contains("-="))
            {
                if (varRef == null)
                {
                    Manager.Throw($"[291]Cannot find the left hand variable. {Value}", ExceptionType.SyntaxException);
                    return null;
                }
                //check if number and apply the change
                double leftNumOut = 0;
                double rightNumOut = 0;

                //check if token is a number too
                var nofailRight = double.TryParse(token.ToString(), out rightNumOut);
                var nofailLeft = double.TryParse(varRef.ToString(), out leftNumOut);
                if (nofailLeft && nofailRight)
                {
                    if (strip.Contains("+="))
                        leftNumOut += rightNumOut;
                    else
                        leftNumOut -= rightNumOut;
                    varRef.SetValue(leftNumOut.ToString());
                }
                else//one or both arent numbers, which means concatenation intead of incrementation.
                {
                    var str = varRef.ToString();
                    if (strip.Contains("+="))
                        str += token.ToString();
                    else
                    {
                        Manager.Throw("[314]Cannot apply the operand -= with type string.", ExceptionType.SyntaxException);
                        return null;
                    }
                    varRef.SetValue(str);
                }
                return "";
            }
            if (strip.Contains("="))
            {
                if (varRef != null)
                {
                    varRef.SetValue(token.ToString());
                }
                else
                    varList.Add(new Token(leftHand, token.ToString(), Value));
                return "";
            }
            Manager.Throw("[330]Unknown error with assignment.", ExceptionType.SyntaxException);
            return null;
        }

        private Token Parse(TFunction t)
        {
            if (!_reference.ReturnFlag)
            {
                if (!Manager.IsScriptStopping)
                {
                    if (_reference.Tracer == null || (!_reference.Tracer.Continue && !_reference.Tracer.Break))
                        return TryParseMember(t);
                }
                else if (Manager.IsScriptStopping && _reference.IsBlindExecute)
                {
                    return TryParseMember(t);
                }
            }
            return null;
        }
        private Token TryParseMember(TFunction tfunc)
        {
            if (tfunc == null)
                return null;
            if (tfunc.Name == "Base")
            {
                tfunc.RedirectFunctionToParentBase();
                return tfunc.TryParse();
            }
            //self calling check to prevent stack overflow exception
            if (tfunc.Name == _reference.Name)
            {
                Manager.Throw("Cannot call function from itself. Please use `Base()` if this is an override.", ExceptionType.CompilerException);
                return null;
            }
            return tfunc.TryParse();
        }

        private string RuntimeDebugger(string value)
        {
            var val = value;
            if (val.Contains("!DEBUG"))
            {
                val = val.Replace("\t", "").Replace("\n", "").Replace("\r", "");
                try
                {
                    if (val.Contains("!DEBUG_DUMP_LOCVARS"))
                    {
                        var localVars = JsonConvert.SerializeObject(_reference.LocalVariables, Formatting.Indented);
                        Manager.ThrowDebug($"{val}:\t{localVars}");
                        return "";
                    }
                    else if (val.Contains("!DEBUG_DUMP_ANONVARS"))
                    {
                        var anonVars = JsonConvert.SerializeObject(AnonymousTokenStack.List, Formatting.Indented);
                        Manager.ThrowDebug($"{val}:\t{anonVars}");
                        return "";
                    }
                    else if (val.Contains("!DEBUG_DUMP_GLOBVARS"))
                    {
                        var globalVars = JsonConvert.SerializeObject(GlobalVariableStack.List, Formatting.Indented);
                        Manager.ThrowDebug($"!{val}:\t{globalVars}");
                        return "";
                    }
                    else if (val.Contains("!DEBUG_DUMP_PARAMVARS"))
                    {
                        var paramVars = JsonConvert.SerializeObject(_reference.ProvidedArgs, Formatting.Indented);
                        Manager.ThrowDebug($"!{val}:\t{paramVars}");
                        return "";
                    }
                    else if (val.Contains("!DEBUG_DUMP_BASE"))
                    {
                        if (val.Contains("."))
                        {
                            try
                            {
                                var ext = val.Split('.')[1];
                                var paramVars = JsonConvert.SerializeObject(GetPropValue(_reference.Base, ext), Formatting.Indented);
                                Manager.ThrowDebug($"!{val}:\t{paramVars}");
                            }
                            catch
                            {
                                Manager.Throw($"Cannot find property {val}", ExceptionType.SyntaxException);
                                return null;
                            }
                            return "";
                        }
                        else
                        {
                            var paramVars = JsonConvert.SerializeObject(_reference.Base, Formatting.Indented);
                            Manager.ThrowDebug($"{val}:\t{paramVars}");
                            return "";
                        }
                    }
                    else if (val.Contains("!DEBUG_DUMP_FUNC"))
                    {
                        if (val.Contains("."))
                        {
                            try
                            {
                                var ext = val.Split('.')[1];
                                var paramVars = JsonConvert.SerializeObject(GetPropValue(_reference, ext), Formatting.Indented);
                                Manager.ThrowDebug($"!{val}:\t{paramVars}");
                            }
                            catch
                            {
                                Manager.Throw($"Cannot find property {val}", ExceptionType.SyntaxException);
                                return null;
                            }
                            return "";
                        }
                        else
                        {
                            var paramVars = JsonConvert.SerializeObject(_reference, Formatting.Indented);
                            Manager.ThrowDebug($"{val}:\t{paramVars}");
                            return "";
                        }
                    }
                }
                catch
                {
                    Manager.Throw($"You broke the debugger! Function [{val}]", ExceptionType.SyntaxException);
                    return null;
                }
                return "";
            }
            return val;
        }
        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
