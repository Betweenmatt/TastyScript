using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Function;
using TastyScript.ParserManager.Looping;

namespace TastyScript.IFunction.Tokens
{
    /// <summary>
    /// TFunction overhaul: currently when a function is being invoked you must use func.SetInvokeProperties before the
    /// TryParse(tfunc) call. This is to pass on local variables/provided args to invoked functions anonymous or otherwise.
    /// The idea behind this overhaul is to encapsulate the SetInvokeProperties call inside the TFunction, letting TFunction
    /// decide to send it or not. Too many times have I forgotten to use SetInvokeProperties and this should fix that in an autonomous way.
    /// The current implementation of TFunction is super sloppy anyway so i will be cleaning that up too
    /// </summary>
    public class TFunction : Token
    {
        public BaseFunction Function { get; private set; }
        public string[] Arguments { get; private set; }
        public BaseFunction CallingFunction { get; private set; }
        public bool BlindExecute { get; set; }
        public LoopTracer Tracer { get; private set; }
        public Dictionary<string, object> DynamicDictionary { get; private set; }

        /// <summary>
        /// callFunction is the function being called for reference.
        /// callingFunction is the function that is calling callFunction.
        /// </summary>
        /// <param name="callFunction"></param>
        /// <param name="args"></param>
        /// <param name="callingFunction"></param>
        /// <param name="t"></param>
        public TFunction(BaseFunction callFunction, string args, BaseFunction callingFunction, LoopTracer t = null)
        {
            Name = callFunction.Name;
            Function = callFunction;
            _value = "<Type.TFunction>";
            Extensions = new ExtensionList();
            Arguments = ReturnArgsArray(args);
            CallingFunction = callingFunction;
            
            if (callingFunction?.Caller?.DynamicDictionary != null)
            {
                DynamicDictionary = callingFunction.Caller.DynamicDictionary;
            }
            if (t != null)
                Tracer = t;
            else
                if (callingFunction != null)
                Tracer = callingFunction.Tracer;
            else
                Tracer = null;
        }
        public TFunction(BaseFunction callFunction, ExtensionList extensions, string args, BaseFunction callingFunction, LoopTracer t = null)
        {
            Name = callFunction.Name;
            Function = callFunction;
            _value = "<Type.TFunction>";
            Extensions = extensions;
            Arguments = ReturnArgsArray(args);
            CallingFunction = callingFunction;
            if (callingFunction?.Caller?.DynamicDictionary != null)
            {
                DynamicDictionary = callingFunction.Caller.DynamicDictionary;
            }
            if (t != null)
                Tracer = t;
            else
                if (callingFunction != null)
                Tracer = callingFunction.Tracer;
            else
                Tracer = null;
        }
        /// <summary>
        /// callingFunction is the function that is calling(usually `this`), func is the function being called
        /// </summary>
        /// <param name="func"></param>
        /// <param name="ext"></param>
        /// <param name="args"></param>
        /// <param name="callingFunction"></param>
        public TFunction(BaseFunction func, ExtensionList ext, string[] args, BaseFunction callingFunction, LoopTracer t = null)
        {
            Name = func.Name;
            Function = func;
            _value = "<Type.TFunction>";
            Extensions = ext;
            Arguments = args;
            CallingFunction = callingFunction;
            if(callingFunction?.Caller?.DynamicDictionary != null)
            {
                DynamicDictionary = callingFunction.Caller.DynamicDictionary;
            }
            if (t != null)
                Tracer = t;
            else
                if (callingFunction != null)
                Tracer = callingFunction.Tracer;
            else
                Tracer = null;
        }
        public TFunction(BaseFunction func, ExtensionList ext, Dictionary<string, object> args, BaseFunction callingFunction, LoopTracer t = null)
        {
            Name = func.Name;
            Function = func;
            _value = "<Type.TFunction>";
            Extensions = ext;
            Arguments = new string[] { "<Type.Dynamic>" };
            DynamicDictionary = args;
            CallingFunction = callingFunction;
            if (t != null)
                Tracer = t;
            else
                if (callingFunction != null)
                Tracer = callingFunction.Tracer;
            else
                Tracer = null;
        }
        public void SetTracer(LoopTracer t)
        {
            Tracer = t;
        }
        public string[] ReturnArgsArray()
        {
            return Arguments;
        }
        private string[] ReturnArgsArray(string args)
        {
            if (args == null)
                return new string[] { };
            var output = args;
            var index = 0;
            Dictionary<string, string> stringParts = new Dictionary<string, string>();
            Dictionary<string, string> parenParts = new Dictionary<string, string>();
            if (args.Contains("\""))
            {
                var stringRegex = new Regex("\"([^\"\"]*)\"", RegexOptions.Multiline);
                foreach (var s in stringRegex.Matches(output))
                {
                    var token = "{AutoGeneratedToken" + index + "}";
                    stringParts.Add(token, s.ToString());
                    output = output.Replace(s.ToString(), token);
                    index++;
                }
            }
            if (args.Contains("[") && args.Contains("]"))
            {
                var reg = new Regex(@"(?:(?:\[(?>[^\[\]]+|\[(?<number>)|\](?<-number>))*(?(number)(?!))\])|[^[]])+");
                foreach (var x in reg.Matches(output))
                {
                    var token = "{AutoGeneratedToken" + index + "}";
                    parenParts.Add(token, x.ToString());
                    output = output.Replace(x.ToString(), token);
                    index++;
                }
            }
            var splode = output.Split(',');

            for (var i = 0; i < splode.Length; i++)
            {
                foreach (var p in parenParts)
                {
                    if (splode[i].Contains(p.Key))
                        splode[i] = splode[i].Replace(p.Key, p.Value);
                }
            }
            for (var i = 0; i < splode.Length; i++)
            {
                foreach (var p in stringParts)
                {
                    if (splode[i].Contains(p.Key))
                        splode[i] = splode[i].Replace(p.Key, p.Value).Replace("\"", "");
                }
            }
            return splode;
        }
    }
}
