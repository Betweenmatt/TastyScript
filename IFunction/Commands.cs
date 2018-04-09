using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Function;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;
using TastyScript.ParserManager.Driver.Android;

namespace TastyScript.IFunction
{
    public static class Commands
    {
        public static string GetScreenHeight() => Manager.Driver.ScreenHeight;
        public static string GetScreenWidth() => Manager.Driver.ScreenWidth;
        public static string Connect(string i)
        {
            return Manager.Driver.Connect(i.UnCleanString());
        }
        public static void Tap(int x, int y)
        {
            Manager.Driver.SendCommand($"input tap {x} {y}");
        }
        public static void LongTap(int x, int y, int duration)
        {
            Manager.Driver.SendCommand($"input swipe {x} {y} {x} {y} {duration}");
        }
        public static void Swipe(int x1, int y1, int x2, int y2, int duration)
        {
            Manager.Driver.SendCommand($"input swipe {x1} {y1} {x2} {y2} {duration}");
        }
        public static void KeyEvent(AndroidKeyCode code)
        {
            Manager.Driver.SendCommand($"input keyevent {(int)code}");
        }
        public static void SendText(string text)
        {
            Manager.Driver.SendCommand($"input text {text.UnCleanString().Replace(" ", "%s")}");
        }
        public static Image GetScreenshot()
        {
            return Manager.Driver.GetScreenshot().Result;
        }
        public static string SetAppPackage(string pkg)
        {
            return Manager.Driver.SetAppPackage(pkg.UnCleanString());
        }
        public static double[] GetImageCoordinates(string path, string[] prop)
        {
            AnalyzeScreen a = new AnalyzeScreen();
            var ret = a.GetScreenCoords(path.UnCleanString(), prop);
            return ret;
        }
        public static string GetDeviceSerial()
        {
            if (!Manager.Driver.IsConnected())
                return Manager.Driver.GetName();
            else
                return "";
        }
        public static void AnalyzeScreen(string success, BaseFunction successAction, BaseFunction failureAction, string[] prop, TFunction caller = null)
        {
            var tfunc = new TFunction(caller.Function, new ExtensionList(), string.Join(",", caller.Function.GetInvokeProperties()), caller.CallingFunction);
            try
            {
                bool finished = false;
                bool func = false;
                Thread th = new Thread(() =>
                {
                    AnalyzeScreen ascreen = new AnalyzeScreen();
                    ascreen.Analyze(success.UnCleanString(),
                        () => { finished = true; func = true; },
                        () => { finished = true; },
                        prop
                    );
                });
                th.Start();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (finished == false)
                {
                    if (Manager.IsScriptStopping)
                        break;
                    Thread.Sleep(1000);//sleep for 1 second before checking again
                                       //if 30 seconds go by, then break and kill the thread
                    if (watch.Elapsed.TotalMilliseconds >= 45000)
                    {
                        Manager.ThrowSilent($"CheckScreen() timed out.");
                        //smash the thread and move on. we dont care about that data anyway
                        try { th.Abort(); } catch (Exception e) { if (!(e is ThreadAbortException)) throw; }
                        break;
                    }
                }
                watch.Stop();
                if (func)
                    successAction.TryParse(tfunc);
                else
                    failureAction.TryParse(tfunc);
            }
            catch
            {
                Manager.ThrowSilent(("[63]Image check failed to execute. Continuing with failure function"));
                failureAction.TryParse(tfunc);
            }
        }
        public static void AnalyzeScreen(string success, string failure, BaseFunction successAction, BaseFunction failureAction, string[] prop, TFunction caller = null)
        {
            var tfunc = new TFunction(caller.Function, new ExtensionList(), string.Join(",", caller.Function.GetInvokeProperties()), caller.CallingFunction);
            try
            {
                bool finished = false;
                bool func = false;
                Thread th = new Thread(() =>
                {
                    AnalyzeScreen ascreen = new AnalyzeScreen();
                    ascreen.Analyze(success.UnCleanString(), failure.UnCleanString(),
                            () => { finished = true; func = true; },
                            () => { finished = true; },
                            prop
                        );
                });
                th.Start();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (finished == false)
                {
                    if (Manager.IsScriptStopping)
                        break;
                    Thread.Sleep(1000);//sleep for 1 second before checking again
                                       //if 30 seconds go by, then break and kill the thread
                    if (watch.Elapsed.TotalMilliseconds >= 45000)
                    {
                        Manager.ThrowSilent($"CheckScreen() timed out.");
                        //smash the thread and move on. we dont care about that data anyway
                        try { th.Abort(); } catch (Exception e) { if (!(e is ThreadAbortException)) throw; }
                        break;
                    }
                }
                watch.Stop();
                if (func)
                    successAction.TryParse(tfunc);
                else
                    failureAction.TryParse(tfunc);
            }
            catch
            {
                Manager.Throw(("[63]Image check failed to execute. Continuing with failure function"));
            }

        }
    }
}
