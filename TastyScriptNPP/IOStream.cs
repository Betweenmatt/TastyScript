using Kbg.NppPluginNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TastyScriptNPP
{
    public class IOStream : TastyScript.IIOStream
    {
        private Output _ref;
        public static Dictionary<string, string> ColorOverrides;
        public static IOStream Instance;
        public IOStream(Output output)
        {
            _ref = output;
            Instance = this;
        }
        public static void SetColorOverrides(string cols)
        {
            ColorOverrides = new Dictionary<string, string>();
            var lines = cols.Split(';');
            foreach(var x in lines)
            {
                var lr = x.Split(',');
                if (lr.ElementAtOrDefault(0) == null || lr.ElementAtOrDefault(1) == null)
                    continue;
                ColorOverrides[lr[0]] = lr[1];
            }
        }
        public void Print(object o, bool line = true)
        {
            _ref.Pipe(o.ToString(), line);
        }
        public void Print(object o, ConsoleColor color, bool line = true)
        {
            _ref.Pipe(o.ToString(), GetColorFromConsoleEnum(color), line);
        }
        public void Print(object o, Color color, bool line = true)
        {
            _ref.Pipe(o.ToString(), color, line);
        }
        private Color GetColorFromConsoleEnum(ConsoleColor e)
        {
            if (ColorOverrides == null)
                return Color.Black;
            if (e == ConsoleColor.Gray)
                return Settings.OutputPanel.DefaultTextColor;
            var first = ColorOverrides.FirstOrDefault(f => f.Key == e.ToString());
            if (first.Key == null || first.Value == null)
                return Color.Black;
            return Color.FromName(first.Value);
            
        }
        public void ChangeConsoleTitle(string append)
        {
            Print(append, ConsoleColor.DarkCyan);
        }
        public string Read()
        {
            throw new NotImplementedException();
        }
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            throw new NotImplementedException();
        }
    }
}
