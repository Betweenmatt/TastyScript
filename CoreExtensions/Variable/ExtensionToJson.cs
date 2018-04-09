using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreExtensions.Variable
{
    [Extension("ToJson", varExtension:true)]
    public class ExtensionToJson : BaseExtension
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            bool pretty = false;
            if (args != null || args.ElementAtOrDefault(0) != null)
                if (args[0] == "True" || args[0] == "true")
                    pretty = true;

            var inputAsTobj = new TArray("arr", input.Value, input.Line); ;
            if (inputAsTobj == null)
                Manager.Throw($"Cannot find Token [{input.Name}]");

            var trim = inputAsTobj.ToString();
            trim = trim.Substring(1, trim.Length - 2);
            int level = 0;
            string output = "{";

            for (int i = 0; i < trim.Length; i++)
            {
                if(trim[i] == '[')
                {
                    if (level != 0)
                        output += "[";
                    output += "\"";
                    level++;
                    continue;
                }
                if(trim[i] == ']')
                {
                    output += "\"";
                    if (level != 1)
                        output += "]";
                    level--;
                    continue;
                }
                if (trim[i] == ',' && level == 1)
                {
                    output += "\":\"";
                    continue;
                }
                if(trim[i] == ',' && level > 1)
                {
                    output += "\",\"";
                    continue;
                }
                output += trim[i];
            }
            output += "}";
            output = output.Replace("\"\"", "\"").Replace("\"[", "[").Replace("]\"", "]");
            dynamic checkresult = null;
            try
            {
                //do a quick deserialize just to double check json worked right
                //probly not the best method :\
                checkresult = JsonConvert.DeserializeObject(output);
            }
            catch
            {
                Manager.Throw("There was an error converting tojson, please double check your arrays!");
            }
            if (pretty)
            {
                output = JsonConvert.SerializeObject(checkresult, Formatting.Indented);
            }

            output = output.UnCleanString().CleanString();

            return new Token("json", output, "");
        }
    }
}
