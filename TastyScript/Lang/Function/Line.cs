using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Func
{
    public class Line
    {
        public List<IBaseToken> Tokens { get; private set; }
        public string Value { get; private set; }
        public Line(string value, IBaseFunction reference)
        {
            //performance check on a line by line basis
            //Console.WriteLine($"\t{DateTime.Now.ToString("HH:mm:ss.fff")}:\t{value}");
            Value = value;
            Tokens = Parse(value, reference);
        }
        private List<IBaseToken> Parse(string val, IBaseFunction reference)
        {
            List<IBaseToken> temp = new List<IBaseToken>();
            string value = val.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            var wscheck = new Regex(@"^\s*$");
            var wscheckk = wscheck.IsMatch(value);
            if (wscheckk)
                return temp;
            //if (value.Contains('#'))
            //    value = value.Split('#')[0];
            //
            //string parsing(look for quotes)
            var stringTokenRegex = new Regex("\"([^\"\"]*)\"");
            var strings = stringTokenRegex.Matches(value);
            foreach (var x in strings)
            {
                string tokenname = " {AnonGeneratedToken" + reference.GeneratedTokensIndex + "} ";
                var tstring = new TString(tokenname.Replace(" ", ""), Regex.Replace(x.ToString(), "\"", ""));
                value = value.Replace(x.ToString(), tokenname);

                reference.GeneratedTokens.Add(tstring);
            }
            
            //
            //do math expressions(look for brackets)
            var mathexpRegex = new Regex(@"\[([^\[\]]*)\]");
            var mathexp = mathexpRegex.Matches(value);
            foreach (var x in mathexp)
            {
                var input = x.ToString().Replace("[", "").Replace("]", "").Replace(" ", "");
                if (input != null && input != "")
                {
                    string tokenname = " {AnonGeneratedToken" + reference.GeneratedTokensIndex + "} ";
                    double exp = MathExpression(input, reference, val);
                    reference.GeneratedTokens.Add(new TNumber(tokenname.Replace(" ", ""), exp));
                    value = value.Replace(x.ToString(), tokenname);
                }
            }

            //
            //number parsing
            var numberTokenRegex = new Regex(@"\b-*[0-9\.]+\b");
            var numbers = numberTokenRegex.Matches(value);
            foreach (var x in numbers)
            {
                string tokenname = " {AnonGeneratedToken" + reference.GeneratedTokensIndex + "} ";
                reference.GeneratedTokens.Add(new TNumber(tokenname.Replace(" ", ""), double.Parse(x.ToString())));
                //do this regex instead of a blind replace to fix the above issue. NOTE this fix may break decimal use in some situations!!!!
                var indvRegex = (@"\b-*" + x + @"\b");
                var regex = new Regex(indvRegex);
                value = regex.Replace(value, tokenname);
            }

            
            //
            //parameters parsing(look for parentheses)
            var paramTokenRegex = new Regex(@"\(([^()]*)\)");
            var paramss = paramTokenRegex.Matches(value);
            foreach (var x in paramss)
            {
                string tokenname = " {AnonGeneratedToken" + reference.GeneratedTokensIndex + "} ";
                List<IBaseToken> paramlist = new List<IBaseToken>();
                var compCheck = ComparisonCheck(x.ToString(),reference,val);
                if (compCheck != "")
                {
                    paramlist.Add(new TString("bool", compCheck));
                }
                else
                {
                    var splode = x.ToString().Replace("(", "").Replace(")", "").Split(',');
                    paramlist = GetTokens(splode, reference, val);
                }
                reference.GeneratedTokens.Add(new TParameter(tokenname.Replace(" ", ""), paramlist));
                value = value.Replace(x.ToString(), tokenname);
            }

            //
            //if line is variable assignment
            if (value.Contains("var "))
            {
                value = CheckForVar(value, reference, val);
                if (value == "")
                    return temp;
            }


            //
            //lastly do functions and extensions
            var words = value.Split(' ');
            if (words.Length == 1)
                if (words[0] == "")
                    return temp;
                else
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"[113]Functions cannot be called without supplying arguments.", val));
                    return temp;
                }
            else if (words.Length > 1)
            {
                var stripws = words[0].Replace(" ", "");
                if (stripws != "" && stripws != " ")//??
                {
                    var obj = TokenParser.FunctionList.FirstOrDefault(f => f.Name == stripws);
                    if (obj == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"[124]Function [{stripws}] cannot be found.", val));
                        return temp;
                    }
                    var argsname = words[1].Replace(" ", "");
                    var args = reference.GeneratedTokens.FirstOrDefault(f => f.Name == argsname);
                    if (args == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"[131]Arguments cannot be found.", val));
                        return temp;
                    }
                    var func = new TFunction(obj.Name, obj);
                    IBaseFunction clone = null;
                    if (obj.Invoking)
                    {
                        clone = Program.CopyFunctionReference(obj.Name);
                        var asT = args as TParameter;
                        if (asT != null)
                        {
                            var invokeFuncName = asT.Value.Value[0].ToString();
                            if (invokeFuncName.Contains("AnonymousFunction"))
                            {
                                var functionToInvoke = TokenParser.FunctionList.FirstOrDefault(f => f.Name == invokeFuncName);
                                if (functionToInvoke != null)
                                {
                                    var argss = new TParameter("args", GetTokens(functionToInvoke.ExpectedArgs, reference, value));
                                    clone.SetInvokeProperties(argss);
                                    func = new TFunction(obj.Name, clone);//replace the old func with the clone
                                }
                            }
                        }
                        
                    }
                    
                    func.Arguments = args as TParameter;
                    func.Extensions = EvaluateExtensions(value, reference);
                    temp.Add(func);
                }
            }
            return temp;
        }

        private List<IExtension> EvaluateExtensions(string value, IBaseFunction reference, IBaseToken homeToken = null)
        {
            string[] ext = value.Split('.');

            var extensions = new List<IExtension>();
            if (ext.Length > 1)
            {
                for (var x = 0; x < ext.Length; x++)
                {
                    if (x == 0)
                        continue;
                    var seperate = ext[x].Split(' ');
                    if (seperate.Length < 2)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"[157]Extension arguments must be present.", value));
                        return extensions;
                    }
                    var extName = seperate[0].Replace(" ", "");
                    var argsName = seperate[1].Replace(" ", "");
                    var extArgs = reference.GeneratedTokens.FirstOrDefault(f => f.Name == argsName);
                    if (extArgs == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"[170]Unexpected error getting arguments for extension.", value));
                    }
                    var findExt = TokenParser.Extensions.FirstOrDefault(f => f.Name == extName);
                    if (findExt == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"[176]Unexpected error getting arguments for extension.", value));
                    }
                    
                    //clone the extension because it was breaking
                    var clone = DeepCopy<BaseExtension>(findExt as BaseExtension);
                    if (clone.Invoking)
                    {
                        var asT = extArgs as TParameter;
                        if (asT != null)
                        {
                            var invokeFuncName = asT.Value.Value[0].ToString();
                            if (invokeFuncName.Contains("AnonymousFunction"))
                            {
                                var functionToInvoke = TokenParser.FunctionList.FirstOrDefault(f => f.Name == invokeFuncName);
                                if (functionToInvoke != null)
                                {
                                    var args = new TParameter("args", GetTokens(functionToInvoke.ExpectedArgs, reference, value));
                                    clone.SetInvokeProperties(args);
                                }
                            }

                        }
                    }
                    clone.Arguments = extArgs as TParameter;
                    extensions.Add(clone as IExtension);
                }
            }
            return extensions;
        }

        private string CheckForVar(string value, IBaseFunction reference, string lineRef)
        {
            if (value.Contains("$var "))
            {
                var strip = value.Replace("$var ", "");
                var assign = strip.Split('=');
                if (assign.Length != 2)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", lineRef));
                }
                var rightHandAssignment = assign[1].Replace(" ", "");
                var leftHandAssignment = assign[0].Replace(" ", "");
                if (rightHandAssignment == null || rightHandAssignment == "" || rightHandAssignment == " ")
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignemnt.", lineRef));
                }
                else
                {
                    var stripws = rightHandAssignment.Replace(" ", "");
                    var varRef = TokenParser.GlobalVariables.FirstOrDefault(f => f.Name == leftHandAssignment);
                    if (stripws == "null")
                    {
                        if (varRef != null)
                        {
                            TokenParser.GlobalVariables.Remove(varRef);
                        }
                        return "";
                    }
                    var obj = reference.GeneratedTokens.FirstOrDefault(f => f.Name == stripws);
                    if (obj == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", lineRef));
                    }
                    if (varRef != null)
                    {
                        var varAsT = varRef as TVariable;
                        varAsT.SetValue(obj);
                    }
                    else
                        TokenParser.GlobalVariables.Add(new TVariable(leftHandAssignment, obj));
                }
                return "";
            }
            else if (value.Contains("var "))
            {
                if (value.Contains('='))
                {
                    var strip = value.Replace("var ", "");
                    var assign = strip.Split('=');
                    if (assign.Length != 2)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", lineRef));
                    }
                    var rightHandAssignment = assign[1].Replace(" ", "");
                    var leftHandAssignment = assign[0].Replace(" ", "");
                    if (rightHandAssignment == null || rightHandAssignment == "" || rightHandAssignment == " ")
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignemnt.", lineRef));
                    }
                    else
                    {
                        var stripws = rightHandAssignment.Replace(" ", "");
                        var varRef = reference.VariableTokens.FirstOrDefault(f => f.Name == leftHandAssignment);
                        if (stripws == "null")
                        {
                            if (varRef != null)
                                reference.VariableTokens.Remove(varRef);
                            return "";
                        }
                        var obj = reference.GeneratedTokens.FirstOrDefault(f => f.Name == stripws);
                        if (obj == null)
                        {
                            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", lineRef));
                        }
                        if (varRef != null)
                        {
                            var varAsT = varRef as TVariable;
                            varAsT.SetValue(obj);
                        }
                        else
                            reference.VariableTokens.Add(new TVariable(leftHandAssignment, obj));
                    }
                    return "";
                }
            }
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", lineRef));
            return "";
        }
        public static T DeepCopy<T>(T obj)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new Exception("The source object must be serializable");
            }

            if (Object.ReferenceEquals(obj, null))
            {
                throw new Exception("The source object must not be null");
            }
            T result = default(T);
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, obj);
                memoryStream.Seek(0, SeekOrigin.Begin);
                result = (T)formatter.Deserialize(memoryStream);
                memoryStream.Close();
            }
            return result;

        }

        //evaluates a mathematical expression written in string format
        private double MathExpression(string expression, IBaseFunction reference, string lineReference)
        {
            string exp = expression;
            //get vars and params out of the expression
            //idk if i should be checking for anon tokens, so im gunna leave this here for a while just in case
            var tokenRegex = new Regex(@"\{AnonymousGeneratedToken*\d*\}");
            var tokenRegexMatches = tokenRegex.Matches(exp);
            foreach(var x in tokenRegexMatches)
            {
                var tok = GetTokens(new string[] { x.ToString() }, reference, lineReference);
                var tokfirst = tok.FirstOrDefault(f => f != null);
                if (tokfirst != null)
                {
                    exp = exp.Replace(x.ToString(), tokfirst.ToString());
                }
            }
            var varRegex = new Regex(@"\w[A-Za-z]*\d*");
            var varRegexMatches = varRegex.Matches(exp);
            foreach(var x in varRegexMatches)
            {
                var tok = GetTokens(new string[] { x.ToString() }, reference, lineReference, true);
                
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
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException,
                    $"[331]Unexpected error with mathematical expression:\n{e.Message}",
                    lineReference));
            }
            return 0;
        }
        #region Comparison
        /// <summary>
        /// Check for comparison operators
        /// </summary>
        /// <param name="line"></param>
        /// <param name="reference"></param>
        /// <param name="lineReference"></param>
        /// <returns></returns>
        enum Operator { EQ, NOTEQ, GT, LT, GTEQ, LTEQ }
        private string ComparisonCheck(string line, IBaseFunction reference, string lineReference)
        {
            string output = "";
            if (line.Contains("=="))
                output = FindOperation(Operator.EQ, line, lineReference, reference);
            else if (line.Contains("!="))
                output = FindOperation(Operator.NOTEQ, line, lineReference, reference);
            else if (line.Contains(">="))
                output = FindOperation(Operator.GTEQ, line, lineReference, reference);
            else if (line.Contains("<="))
                output = FindOperation(Operator.LTEQ, line, lineReference, reference);
            else if (line.Contains(">"))
                output = FindOperation(Operator.GT, line, lineReference, reference);
            else if (line.Contains("<"))
                output = FindOperation(Operator.LT, line, lineReference, reference);
            return output;
        }
        //the heavy lifting for comparison check
        private string FindOperation(Operator op, string line, string lineRef, IBaseFunction reference)
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
            if (splitop.Length != 2)
                CompareFail(lineRef);
            var lr = GetTokens(new string[] { splitop[0], splitop[1] }, reference, lineRef);
            try
            {
                switch (op)
                {
                    case (Operator.EQ):
                        output = (lr[0].ToString() == lr[1].ToString()) 
                            ? "True" : "False";
                        break;
                    case (Operator.NOTEQ):
                        output = (lr[0].ToString() != lr[1].ToString()) 
                            ? "True" : "False";
                        break;
                    case (Operator.GT):
                        output = (double.Parse(lr[0].ToString()) > double.Parse(lr[1].ToString()))
                            ? "True" : "False";
                        break;
                    case (Operator.LT):
                        output = (double.Parse(lr[0].ToString()) < double.Parse(lr[1].ToString()))
                            ? "True" : "False";
                        break;
                    case (Operator.GTEQ):
                        output = (double.Parse(lr[0].ToString()) >= double.Parse(lr[1].ToString()))
                            ? "True" : "False";
                        break;
                    case (Operator.LTEQ):
                        output = (double.Parse(lr[0].ToString()) <= double.Parse(lr[1].ToString()))
                            ? "True" : "False";
                        break;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unexpected input: {line}", lineRef));
            }

            return output;
        }
        //this rips off the comparison check, since the concept is the same.
        
        private void CompareFail(string line)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Can not compare more or less than 2 values", line));
        }
        #endregion

        /// <summary>
        /// takes parameters() and gets all the tokens inside it and returns a list
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="reference"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private List<IBaseToken> GetTokens(string[] arr, IBaseFunction reference, string val, bool failsafe = false)
        {
            var temp = new List<IBaseToken>();
            foreach (var p in arr)
            {
                IBaseToken tempvar = null;
                var stripws = p.Replace("(", "").Replace(")", "").Replace("\n","").Replace("\r","").Replace("\t","");
                if (stripws.Contains("var "))
                {
                    tempvar = new TString(stripws.Replace("var ", "").Replace(" ", ""), "0");
                }
                else
                {
                    stripws = stripws.Replace(" ", "");//wait till after var declaration to strip spaces
                    if (stripws == "" || stripws == " ")
                        continue;
                    if (p.Contains("{AnonGeneratedToken"))
                    {
                        var obj = reference.GeneratedTokens.FirstOrDefault(f => f.Name == stripws);
                        if (obj == null)
                        {
                            Compiler.ExceptionListener.Throw(new ExceptionHandler($"Generated Token {stripws} could not be found.", val));
                            return temp;
                        }
                        tempvar = obj;
                    }
                    else
                    {
                        var tryglob = TokenParser.GlobalVariables.FirstOrDefault(f => f.Name == stripws);
                        if (tryglob != null)
                        {
                            var globval = tryglob.ToString();
                            tempvar = (new TString(tryglob.Name, globval));
                        }
                        var tryvar = reference.VariableTokens.FirstOrDefault(f => f.Name == stripws);
                        if (tryvar != null)
                        {
                            var varval = tryvar.ToString();
                            tempvar = (new TString(tryvar.Name, varval));
                        }
                        if (reference.ProvidedArgs != null)
                        {
                            var trypar = reference.ProvidedArgs.FirstOrDefault(f => f.Name == stripws);
                            if (trypar != null)
                            {
                                var parval = trypar.ToString();
                                tempvar = (new TString(trypar.Name, parval));
                            }
                        }
                        var tryfunc = TokenParser.FunctionList.FirstOrDefault(f => f.Name == stripws);
                        if (tryfunc != null)
                        {
                            tempvar = (tryfunc);
                        }
                    }
                }
                if (tempvar == null && !failsafe)
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                        $"[503]Unknown variable [{stripws}] found.", val));
                temp.Add(tempvar);
            }
            return temp;
        }
    }
}
