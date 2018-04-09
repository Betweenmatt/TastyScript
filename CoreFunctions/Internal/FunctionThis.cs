﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("This", new string[] { "type" }, alias:new string[] { "this" })]
    internal class FunctionThis : FunctionDefinition
    {
        public override bool CallBase()
        {
            /*
             * 
             * Since u always seem to forget, Caller.CallingFunction.Extensions to get the
             * Extensions hooked to the top level function this is being called from
             * 
             * */
            var prop = Extensions.First("Prop");
            var arg = ProvidedArgs.First("type");
            if (prop == null)
            {
                if (arg == null)
                {
                    Manager.Throw("Arguments cannot be null without a Prop extension");
                    return false;
                }
                switch (arg.ToString())
                {
                    case ("Extension"):
                        List<string> output = new List<string>();
                        foreach (var x in Caller.CallingFunction.Extensions.List)
                            output.Add(x.Name);
                        ReturnBubble = new TArray("arr", output.ToArray(), "");
                        return true;
                    case ("UID")://ReadOnly
                        ReturnBubble = new Token("uid", UID.ToString(), "");
                        return true;
                    case ("IsAnonymous")://ReadOnly
                        ReturnBubble = new Token("isAnonymous", (IsAnonymous) ? "True" : "False", "");
                        return true;
                    case ("IsOverride")://ReadOnly
                        ReturnBubble = new Token("isOverride", (IsOverride) ? "True" : "False", "");
                        return true;
                    case ("IsSealed")://ReadOnly
                        ReturnBubble = new Token("isSealed", (IsSealed) ? "True" : "False", "");
                        return true;
                    case ("IsObsolete")://ReadOnly
                        ReturnBubble = new Token("isObsolete", (IsObsolete) ? "True" : "False", "");
                        return true;
                    case ("Dynamic"):
                        var json = JsonConvert.SerializeObject(Caller.CallingFunction.Caller.DynamicDictionary, Formatting.Indented).CleanString();
                        ReturnBubble = new Token("dict", json, "");
                        return true;
                }
            }
            else
            {
                var properties = prop.Extend();
                var fprop = properties.ElementAtOrDefault(0);
                if (fprop == null)
                    Manager.Throw("Prop extension must have arguments");
                if (arg != null) {
                    switch (arg.ToString())
                    {
                        case ("Extension"):
                            if(fprop == "Args")
                            {
                                var sprop = properties.ElementAtOrDefault(1);
                                if (sprop == null)
                                    Manager.Throw("Prop 'Args' must have a second parameter");
                                var get = Caller.CallingFunction.Extensions.First(sprop);
                                if (get == null)
                                    ReturnBubble = new Token("null", "null", "");
                                else
                                    ReturnBubble = new TArray("arr", get.Extend(Caller.CallingFunction), "");
                            }
                            return true;
                        case ("Dynamic"):
                            if(fprop != "")
                            {
                                if (Caller.CallingFunction.Caller.DynamicDictionary.ContainsKey(fprop))
                                {
                                    ReturnBubble = new Token("ret", Caller.CallingFunction.Caller.DynamicDictionary[fprop].ToString(),"");
                                }
                                else
                                {
                                    Console.WriteLine("borked");
                                }
                            }
                            return true;
                    }
                }
            }
            return true;
        }
    }
}