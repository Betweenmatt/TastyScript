using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TastyScript.ParserManager.IOStream
{
    public class XmlStreamObj
    {
        public string Text;
        public ConsoleColor Color
        {
            get
            {
                Enum.TryParse<ConsoleColor>(ColorSetter, out ConsoleColor result);
                return result;
            }
        }
        public string ColorSetter;
        public bool Line;
        public string Id;

        public static XmlStreamObj ReadStreamXml(string msg)
        {
            if (msg.Contains("<obj"))
            {
                try
                {
                    XDocument xdoc = XDocument.Parse(msg);

                    var objs = from lv1 in xdoc.Descendants("obj")
                               select new XmlStreamObj
                               {
                                   ColorSetter = lv1.Attribute("color").Value,
                                   Text = lv1.Attribute("text").Value,
                                   Line = lv1.Attribute("line").Value == "True" || lv1.Attribute("line").Value == "true" ? true : false,
                                   Id = lv1.Attribute("id").Value
                               };
                    var x = objs.ElementAtOrDefault(0);
                    if (x != null)
                        return x;
                }
                catch
                {
                    Manager.ThrowSilent($"Unknown error parsing xml, skipping this line.");
                }
            }
            return null;
        }
    }
}
