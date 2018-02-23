using System;
using System.Collections.Generic;
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
            if (value.Contains('#'))
                value = value.Split('#')[0];
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
            //number parsing
            var numberTokenRegex = new Regex(@"\b-*[0-9\.]+\b");
            var numbers = numberTokenRegex.Matches(value);
            foreach (var x in numbers)
            {
                string tokenname = " {AnonGeneratedToken" + reference.GeneratedTokensIndex + "} ";
                reference.GeneratedTokens.Add(new TNumber(tokenname.Replace(" ", ""), double.Parse(x.ToString())));
                //do this regex instead of a blind replace to fix the above issue. NOTE this fix may break decimal use!!!!
                var indvRegex = (@"\b-*" + x + @"\b");
                var regex = new Regex(indvRegex);
                value = regex.Replace(value, tokenname);
            }


            //parameters parsing
            var paramTokenRegex = new Regex(@"\(([^()]*)\)");
            var paramss = paramTokenRegex.Matches(value);
            foreach (var x in paramss)
            {
                string tokenname = " {AnonGeneratedToken" + reference.GeneratedTokensIndex + "} ";
                var splode = x.ToString().Replace("(", "").Replace(")", "").Split(',');
                List<IBaseToken> paramlist = new List<IBaseToken>();
                foreach (var p in splode)
                {
                    var stripws = p.Replace(" ", "");
                    if (p.Contains("{AnonGeneratedToken"))
                    {
                        var obj = reference.GeneratedTokens.FirstOrDefault(f => f.Name == stripws);
                        if (obj == null)
                        {
                            Compiler.ExceptionListener.Throw(new ExceptionHandler($"Generated Token {stripws} could not be found.", val));
                            return temp;
                        }
                        paramlist.Add(obj);
                    }
                    else
                    {
                        var tryglob = TokenParser.GlobalVariables.FirstOrDefault(f => f.Name == stripws);
                        if (tryglob != null)
                        {
                            paramlist.Add(tryglob);
                        }
                        var tryvar = reference.VariableTokens.FirstOrDefault(f => f.Name == stripws);
                        if (tryvar != null)
                        {
                            paramlist.Add(tryvar);
                        }
                        if (reference.ProvidedArgs != null)
                        {
                            var trypar = reference.ProvidedArgs.FirstOrDefault(f => f.Name == stripws);
                            if (trypar != null)
                            {
                                paramlist.Add(trypar);
                            }
                        }
                        var tryfunc = TokenParser.FunctionList.FirstOrDefault(f => f.Name == stripws);
                        if (tryfunc != null)
                        {
                            paramlist.Add(tryfunc);
                        }
                    }
                }
                reference.GeneratedTokens.Add(new TParameter(tokenname.Replace(" ", ""), paramlist));
                value = value.Replace(x.ToString(), tokenname);
            }
            //if line is variable assignment
            if (value.Contains("var"))
            {
                var strip = value.Replace("var", "");
                var assign = strip.Split('=');
                if (assign.Length != 2)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", val));
                    return temp;
                }

                var rightHandAssignment = assign[1].Split(' ');
                var leftHandAssignment = assign[0].Replace("var", "").Replace(" ", "");
                if (rightHandAssignment.Length == 0)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", val));
                    return temp;
                }
                else if (rightHandAssignment.Length == 1)
                {
                    var stripws = rightHandAssignment[0].Replace(" ", "");
                    var obj = reference.GeneratedTokens.FirstOrDefault(f => f.Name == stripws);
                    if (obj == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", val));
                        return temp;
                    }
                    reference.VariableTokens.Add(new TVariable(leftHandAssignment, obj));
                }
                else if (rightHandAssignment.Length == 2)
                {
                    var stripws = rightHandAssignment[0].Replace(" ", "");
                    var obj = TokenParser.FunctionList.FirstOrDefault(f => f.Name == stripws);
                    if (obj == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", val));
                        return temp;
                    }
                    var argsname = rightHandAssignment[1].Replace(" ", "");
                    var args = reference.GeneratedTokens.FirstOrDefault(f => f.Name == argsname);
                    if (args == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Unknown error with assignment.", val));
                        return temp;
                    }
                    obj.Arguments = args as TParameter;
                    reference.VariableTokens.Add(new TVariable(leftHandAssignment, obj));
                }//need to add extension check
                return temp;
            }

            var words = value.Split(' ');
            if (words.Length == 1)
                if (words[0] == "")
                    return temp;
                else
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Functions cannot be called without supplying arguments.", val));
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
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Function {stripws} cannot be found.", val));
                        return temp;
                    }
                    var argsname = words[1].Replace(" ", "");
                    var args = reference.GeneratedTokens.FirstOrDefault(f => f.Name == argsname);
                    if (args == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SyntaxException, $"Arguments cannot be found.", val));
                        return temp;
                    }
                    var func = new TFunction(obj.Name, obj);
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
            if (homeToken != null)
            {
                var findhome = value.Split(new string[] { homeToken.Name }, StringSplitOptions.None);
                ext = findhome[1].Split('.');
            }


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
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Extension arguments must be present.", value));
                        return extensions;
                    }
                    var extName = seperate[0].Replace(" ", "");
                    var argsName = seperate[1].Replace(" ", "");
                    var extArgs = reference.GeneratedTokens.FirstOrDefault(f => f.Name == argsName);
                    if (extArgs == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Unexpected error getting arguments for extension.", value));
                        return extensions;
                    }
                    var findExt = TokenParser.Extensions.FirstOrDefault(f => f.Name == extName);
                    if (findExt == null)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException, $"Unexpected error getting arguments for extension.", value));
                        return extensions;
                    }

                    //clone the extension because it was breaking
                    var clone = DeepCopy<BaseExtension<TParameter>>(findExt as BaseExtension<TParameter>);
                    clone.Arguments = extArgs as TParameter;
                    extensions.Add(clone as IExtension);
                }
            }
            return extensions;
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
    }
}
