using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;

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
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "/temp.png";
                ss.Save(filePath, ImageFormat.Png);
                _screen = (Bitmap)Bitmap.FromFile(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
        public void Analyze(string success, Action successAction, Action failureAction, int thresh)
        {
            if (CheckScreen((Bitmap)Bitmap.FromFile(AppDomain.CurrentDomain.BaseDirectory + success), thresh))
                successAction();
            else
                failureAction();
        }
        public void Analyze(string success, string failure, Action successAction, Action failureAction, int thresh)
        {
            if (CheckScreen((Bitmap)Bitmap.FromFile(AppDomain.CurrentDomain.BaseDirectory + success), thresh))
                successAction();
            else if (CheckScreen((Bitmap)Bitmap.FromFile(AppDomain.CurrentDomain.BaseDirectory + failure), thresh))
                failureAction();
            else
            {
                throw new Exception("Image recognition error");
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
                throw new Exception("Image recognition error");
            }
        }
        private bool CheckScreen(Bitmap temp, int thresh)
        {
            float threshold = thresh / 100;
            System.Drawing.Bitmap sourceImage = AForge.Imaging.Image.Clone(_screen, PixelFormat.Format24bppRgb);
            System.Drawing.Bitmap template = AForge.Imaging.Image.Clone(temp, PixelFormat.Format24bppRgb);
            // create template matching algorithm's instance
            //strip sizes down for speed
            double reductionAmount = 0.2;
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
            Console.WriteLine($"[IMG RECOGNITION]More than one match found: {matchings.Length}");
            return false;
        }
    }
}
