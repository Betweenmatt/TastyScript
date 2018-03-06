using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang
{
    //The idea behind this static class is to rope all the lower level commands that functions
    //use into one static container to reduce the amount of work needed for future changes
    //and having all the commands in one place will make ios support much easier to implement
    internal static class Commands
    {
        public static void Tap(int x, int y)
        {
            Program.AndroidDriver.SendCommand($"input tap {x} {y}");
        }
        public static void LongTap(int x, int y, int duration)
        {
            Program.AndroidDriver.SendCommand($"input swipe {x} {y} {x} {y} {duration}");
        }
        public static void Swipe(int x1, int y1, int x2, int y2, int duration)
        {
            Program.AndroidDriver.SendCommand($"input swipe {x1} {y1} {x2} {y2} {duration}");
        }
        public static void KeyEvent(AndroidKeyCode code)
        {
            Program.AndroidDriver.SendCommand($"input keyevent {(int)code}");
        }
        public static void SendText(string text)
        {
            Program.AndroidDriver.SendCommand($"input text {text.Replace(" ", "%s")}");
        }
        public static Image GetScreenshot()
        {
            return Program.AndroidDriver.GetScreenshot().Result;
        }
        public static void SetAppPackage(string pkg)
        {
            Program.AndroidDriver.SetAppPackage(pkg);
        }
        public static bool CheckFocus()
        {
            return Program.AndroidDriver.CheckFocusedApp();
        }
        public static void AnalyzeScreen(string success, IBaseFunction successAction, IBaseFunction failureAction, int thresh, TFunction caller = null)
        {
            var tfunc = new TFunction(caller.Function, null, string.Join(",",caller.Function.GetInvokeProperties()));
            AnalyzeScreen ascreen = new AnalyzeScreen();
            ascreen.Analyze(success,
                () => { successAction.TryParse(tfunc); },
                () => { failureAction.TryParse(tfunc); },
                thresh
            );
        }
        public static void AnalyzeScreen(string success, string failure, IBaseFunction successAction, IBaseFunction failureAction, int thresh, TFunction caller = null)
        {
            var tfunc = new TFunction(caller.Function, null, string.Join(",", caller.Function.GetInvokeProperties()));
            AnalyzeScreen ascreen = new AnalyzeScreen();
            ascreen.Analyze(success, failure,
                () => { successAction.TryParse(tfunc); },
                () => { failureAction.TryParse(tfunc); },
                thresh
            );
        }
    }
}
