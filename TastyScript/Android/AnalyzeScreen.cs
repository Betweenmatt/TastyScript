using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using TastyScript.Lang;
using TastyScript.Lang.Exceptions;

namespace TastyScript.Android
{
    public class AnalyzeScreen
    {
        private static Bitmap _screen;
        public AnalyzeScreen()
        {
            try
            {
                var ss = Program.AndroidDriver.GetScreenshot();
                _screen = (Bitmap)ss;
            }
            catch (Exception e)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                    $"Unexpected error saving screenshot: {e.Message}"));
            }
        }
        public void Analyze(string success, Action successAction, Action failureAction, int thresh)
        {
            if (CheckScreen(Program.GetImageFromPath(success), thresh))
                successAction();
            else
                failureAction();
        }
        public void Analyze(string success, string failure, Action successAction, Action failureAction, int thresh)
        {
            if (CheckScreen(Program.GetImageFromPath(success), thresh))
                successAction();
            else if (CheckScreen(Program.GetImageFromPath(failure), thresh))
                failureAction();
            else
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                    $"Image Recognition error."));
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
