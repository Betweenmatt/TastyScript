using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScriptNPP
{
    internal class Settings
    {
        internal class OutputPanel
        {
            public static Color DefaultTextColor;
            public static Color DefaultBGColor;
            public static bool Bold;
            public static bool Italic;
            public static int FontSize;
            public static string LogLevel;
            public static string FontName;
            private static string _colorOverrides;
            public static string ColorOverrides
            {
                get
                {
                    return _colorOverrides;
                }
                set
                {
                    _colorOverrides = value;
                    IOStream.SetColorOverrides(value);
                }
            }

            public static void SetDefaultTextColor(int argb)
            {
                DefaultTextColor = Color.FromArgb(argb);
            }
            public static void SetDefaultBGColor(int argb)
            {
                DefaultBGColor = Color.FromArgb(argb);
            }
        }
        
    }
}
