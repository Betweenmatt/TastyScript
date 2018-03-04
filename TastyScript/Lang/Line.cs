using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang
{
    internal class Line
    {
        //this will be local to the function scope
        public List<Token> LocalVariables = new List<Token>();
        private IBaseFunction _reference;
        

        private void ParseArrays(string value)
        {
            string val = value;
            //first we have to find all the arrays
            var arrayRegex = new Regex(@"\(([^()]*)\)", RegexOptions.Multiline);
            var arrayMatches = arrayRegex.Matches(val);
            foreach (var a in arrayMatches)
            {
                string tokenname = "{AnonGeneratedToken" + TokenParser.AnonymousTokensIndex + "}";

                var compCheck = ComparisonCheck(a.ToString());
                if (compCheck != "")
                {
                    TokenParser.AnonymousTokens.Add(new Token(tokenname, compCheck));
                }
                else
                {
                    TokenParser.AnonymousTokens.Add(new Token(tokenname, a.ToString()));
                    val = value.Replace(a.ToString(), "->" + tokenname);
                }
            }
        }
        private List<string> GetTokens(string[] names)
        {
            List<string> temp = new List<string>();
            foreach (var n in names)
            {
                var tryLocal = LocalVariables.FirstOrDefault(f => f.Name == n);
                if (tryLocal != null)
                {
                    temp.Add(tryLocal.ToString());
                    continue;
                }
                var tryGlobal = TokenParser.GlobalVariables.FirstOrDefault(f => f.Name == n);
                if (tryGlobal != null)
                {
                    temp.Add(tryGlobal.ToString());
                    continue;
                }
                var tryAnon = TokenParser.AnonymousTokens.FirstOrDefault(f => f.Name == n);
                if (tryAnon != null)
                {
                    temp.Add(tryAnon.ToString());
                    continue;
                }
                //try params?
            }
            return temp;
        }

        enum Operator { EQ, NOTEQ, GT, LT, GTEQ, LTEQ }
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
            var lr = GetTokens(new string[] { splitop[0], splitop[1] });
            if (lr.Count != 2)
                Compiler.ExceptionListener.Throw("There must be one left-hand and one right-hand in comparison objects.",
                    ExceptionType.SyntaxException);
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
            catch (Exception e)
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
    }
}
