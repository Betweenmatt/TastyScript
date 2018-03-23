using Kbg.NppPluginNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TastyScriptNPP.FunctionTips;

namespace TastyScriptNPP.Forms
{
    public partial class FunctionTipsPanel : Form
    {
        public FunctionTipsPanel()
        {
            InitializeComponent();
        }
        public void PopulatePanel()
        {
            var xmlpath = Path.Combine(Main.IniPath, "TastyScriptFunctions.xml");
            if (File.Exists(xmlpath))
            {
                using (XmlReader reader = XmlReader.Create(xmlpath))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: // The node is an element.
                                if (reader.Name == "FunctionList")
                                    break;
                                if(reader.Name == "KeyWord")
                                {
                                    var x = new XmlFunctionTipsKeyword();
                                    x.Name = reader.GetAttribute("name");
                                }
                                break;
                            case XmlNodeType.Text: //Display the text in each element.
                                Console.WriteLine(reader.Value);
                                break;
                            case XmlNodeType.EndElement: //Display the end of the element.
                                Console.Write("</" + reader.Name);
                                Console.WriteLine(">");
                                break;
                        }
                    }
                }
            }
            else
            {

            }
        }
    }
}
