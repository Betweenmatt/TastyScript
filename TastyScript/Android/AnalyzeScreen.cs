using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using TastyScript.Lang;
using TastyScript.Lang.Exceptions;
using System.Threading;
using System.Diagnostics;

namespace TastyScript.Android
{
    internal class AnalyzeScreen
    {
        private static Bitmap _screen;
        public AnalyzeScreen()
        {
            try
            {
                var ss = Program.AndroidDriver.GetScreenshot();
                _screen = (Bitmap)ss.Result;
            }
            catch (Exception e)
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.SystemException,
                    $"Unexpected error saving screenshot: {e.Message}"));
                _screen = null;
            }
        }
        public void Analyze(string success, Action successAction, Action failureAction, int thresh)
        {
            try
            {
                Action action = null;
                Thread th = new Thread(() =>
                {
                    if (CheckScreen(Program.GetImageFromPath(success), thresh))
                        action = successAction;
                    else
                        action = failureAction;
                });
                th.Start();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (action == null)
                {
                    Thread.Sleep(1000);//sleep for 1 second before checking again
                                       //if 30 seconds go by, then break and kill the thread
                    if (watch.Elapsed.TotalMilliseconds >= 30000)
                    {
                        action = failureAction;
                        Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.SystemException,
                            $"CheckScreen() timed out.", success));
                        //smash the thread and move on. we dont care about that data anyway
                        try { th.Abort(); } catch (Exception e) { if (!(e is ThreadAbortException)) throw; }
                        break;
                    }
                }
                watch.Stop();
                action();
            }
            catch
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler("[63]Image check failed to execute. Continuing with failure function"));
                failureAction();
            }
        }
        public void Analyze(string success, string failure, Action successAction, Action failureAction, int thresh)
        {
            //since now the constructor can return null without throwing an exceptiong, added this
            //to make sure parsing doesn't continue with an innapropriate check result.
            if (_screen == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler($"[63]Unexpected error saving image on CheckScreen() overload."));
            
            try
            {
                Action action = null;
                bool fail = false;
                Thread th = new Thread(() =>
                {
                    if (CheckScreen(Program.GetImageFromPath(success), thresh))
                        action = successAction;
                    else if (CheckScreen(Program.GetImageFromPath(failure), thresh))
                        action = failureAction;
                    else
                    {
                        fail = true;
                        action = failureAction;
                    }
                });
                th.Start();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (action == null)
                {
                    Thread.Sleep(1000);//sleep for 1 second before checking again
                                       //if 30 seconds go by, then break and kill the thread
                    if (watch.Elapsed.TotalMilliseconds >= 30000)
                    {
                        action = failureAction;
                        Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.SystemException,
                            $"CheckScreen() timed out.", success));
                        //smash the thread and move on. we dont care about that data anyway
                        try { th.Abort(); } catch (Exception e) { if (!(e is ThreadAbortException)) throw; }
                        break;
                    }
                }
                watch.Stop();
                if (!fail)
                    action();
                else
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                            $"Image Recognition error. Neither images matched."));

            }
            catch
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler("[112]Image check failed to execute."));
                failureAction();
            }
        }
        public void Analyze(Bitmap success, Action successAction, Action failureAction, int thresh)
        {
            if (CheckScreen(success, thresh))
                successAction();
            else
                failureAction();
        }
        public void Analyze(Bitmap success, Bitmap failure, Action successAction, Action failureAction, int thresh)
        {
            if (CheckScreen(success, thresh))
                successAction();
            else if (CheckScreen(failure, thresh))
                failureAction();
            else
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                    $"Image Recognition error."));
            }
        }
        private bool CheckScreen(Bitmap temp, int thresh)
        {
            if (_screen == null)
                return false;
            float threshold = thresh / 100f;
            System.Drawing.Bitmap sourceImage = AForge.Imaging.Image.Clone(_screen, PixelFormat.Format24bppRgb);
            System.Drawing.Bitmap template = AForge.Imaging.Image.Clone(temp, PixelFormat.Format24bppRgb);
            // create template matching algorithm's instance
            //strip sizes down for speed
            double reductionAmount = .2;
            sourceImage = new ResizeBicubic((int)(sourceImage.Width * reductionAmount), (int)(sourceImage.Height * reductionAmount)).Apply(sourceImage);
            template = new ResizeBicubic((int)(template.Width * reductionAmount), (int)(template.Height * reductionAmount)).Apply(template);
            // (set similarity threshold to 92.1%)
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(threshold);
            // find all matchings with specified above similarity
            TemplateMatch[] matchings = tm.ProcessImage(sourceImage, template);

            if (matchings.Length == 0)
            {
                return false;
            }
            else if (matchings.Length == 1)
            {
                return true;
            }
            Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler($"[IMG RECOGNITION] More than one match found: {matchings.Length}"));
            return false;
        }
    }
}
