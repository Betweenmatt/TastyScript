using AForge.Imaging;
using AForge.Imaging.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang;

namespace TastyScript.Android
{
    internal class AnalyzeScreen
    {
        private static Bitmap _screen;
        public AnalyzeScreen()
        {
            //pull the screen and store it
            try
            {
                var ss = Main.AndroidDriver.GetScreenshot();
                _screen = (Bitmap)ss.Result;
            }
            catch (Exception e)
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.SystemException,
                    $"Unexpected error saving screenshot: {e.Message}"));
                _screen = null;
            }
        }
        /// <summary>
        /// IF options include split screen style then apply the applicable cropping else return the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private Bitmap SplitSourceImage(Bitmap source, TemplateMatchOptions options)
        {
            if (options.SplitScreenPanel != Panel.All)
            {
                Crop f = new Crop(GetCrop(source, options));
                
                //var ss = f.Apply(source);
                //ss.Save("croptest.png", ImageFormat.Png);
                //throw new Exception();
                try
                {
                    var output = f.Apply(source);
                    if(options.SavePath != "")
                    {
                        output.Save(options.SavePath.CleanString(), ImageFormat.Png);
                    }
                    return output;
                }
                catch
                {
                    Compiler.ExceptionListener.Throw("[53]There was an unexpected issue getting Options for your source image cropping. Please " +
                        "double check your additional options!");
                    return source;
                }
            }
            else
            {
                return source;
            }
        }
        private Rectangle GetCrop(Bitmap source, TemplateMatchOptions options)
        {
            switch (options.SplitScreenPanel)
            {
                case (Panel.Top):
                    return new Rectangle(0, 0, source.Width, (source.Height / 2));
                case (Panel.Bottom):
                    return new Rectangle(0, (source.Height / 2), source.Width, (source.Height / 2));
                case (Panel.Left):
                    return new Rectangle(0, 0, (source.Width / 2), (source.Height));
                case (Panel.Right):
                    return new Rectangle((source.Width / 2), 0, (source.Width / 2), (source.Height));
                case (Panel.Q1):
                    return new Rectangle(0, 0, (source.Width / 2), (source.Height / 2));
                case (Panel.Q2):
                    return new Rectangle((source.Width / 2), 0, (source.Width / 2), (source.Height / 2));
                case (Panel.Q3):
                    return new Rectangle(0, (source.Height / 2), (source.Width / 2), (source.Height / 2));
                case (Panel.Q4):
                    return new Rectangle((source.Width / 2), (source.Height / 2), (source.Width / 2), (source.Height / 2));
                case (Panel.Custom):
                    return new Rectangle(options.CustomX, options.CustomY, options.CustomWidth, options.CustomHeight);
                default:
                    throw new Exception("borked");
            }
        }

        private double[] TemplateMatch(TemplateMatchOptions options)
        {
            Stopwatch watch = Stopwatch.StartNew();
            if (_screen == null)
                return null;
            var originScreenX = _screen.Width;
            var originScreenY = _screen.Height;
            float threshold = options.Threshold / 100f;
            float reductionAmount = options.Reduction / 100f;
            //Console.WriteLine("Cloning " + watch.ElapsedMilliseconds);
            System.Drawing.Bitmap template = AForge.Imaging.Image.Clone(options.Template, PixelFormat.Format24bppRgb);
            //Console.WriteLine("Splitting source " + watch.ElapsedMilliseconds);
            System.Drawing.Bitmap sourceImage = SplitSourceImage(_screen, options);
            //Console.WriteLine("resizing imgs " + watch.ElapsedMilliseconds);
            if (sourceImage == null)
                throw new Exception();
            sourceImage = AForge.Imaging.Image.Clone(sourceImage, PixelFormat.Format24bppRgb);
            sourceImage = new ResizeBicubic((int)(sourceImage.Width * reductionAmount), (int)(sourceImage.Height * reductionAmount)).Apply(sourceImage);
            template = new ResizeBicubic((int)(template.Width * reductionAmount), (int)(template.Height * reductionAmount)).Apply(template);

            //Console.WriteLine("exhausting " + watch.ElapsedMilliseconds);
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(threshold);
            // find all matchings with specified above similarity
            TemplateMatch[] matchings = tm.ProcessImage(sourceImage, template);
            //Console.WriteLine("done " + watch.ElapsedMilliseconds);
            if (matchings.Length == 0)
            {
                return null;
            }
            else if (matchings.Length > 0)
            {
                return (GetCoords(options, matchings[0], template.Width, template.Height, originScreenX, originScreenY));
            }
            Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler($"[IMG RECOGNITION] More than one match found: {matchings.Length}"));
            return null;
        }
        private double[] GetCoords(TemplateMatchOptions options, TemplateMatch match, int templateWidth, int templateHeight, int screenx, int screeny)
        {
            var reduction = options.Reduction / 100f;
            var matchx = match.Rectangle.Location.X / reduction;
            var matchy = match.Rectangle.Location.Y / reduction;
            if (options.SplitScreenPanel != Panel.All) {
                switch (options.SplitScreenPanel)
                {
                    case (Panel.Top):
                        break;
                    case (Panel.Bottom):
                        matchy = matchy + (screeny / 2);
                        break;
                    case (Panel.Left):
                        break;
                    case (Panel.Right):
                        matchx = matchx + (screenx / 2);
                        break;
                    case (Panel.Q1):
                        break;
                    case (Panel.Q2):
                        matchx = matchx + (screenx / 2);
                        matchy = matchy + (screeny / 2);
                        break;
                    case (Panel.Q3):
                        matchy = matchy + (screeny / 2);
                        break;
                    case (Panel.Q4):
                        matchx = matchx + (screenx / 2);
                        matchy = matchy + (screeny / 2);
                        break;
                    case (Panel.Custom):
                        matchx = matchx + (options.CustomX);
                        matchy = matchy + (options.CustomY);
                        break;
                }
            }
            var centerx = matchx + ((templateWidth / reduction) / 2);
            var centery = matchy + ((templateHeight / reduction) / 2);
            return new double[] { centerx, centery };
        }

        public void Analyze(string success, Action successAction, Action failureAction, string[] prop)
        {
            var opt = GetOptionsFromStringArray(prop, success);
            if (TemplateMatch(opt) != null)
                successAction();
            else
                failureAction();
        }
        public void Analyze(string success, string failure, Action successAction, Action failureAction, string[] prop)
        {
            var opt = GetOptionsFromStringArray(prop, success);
            var failopt = GetOptionsFromStringArray(prop, failure);
            if (TemplateMatch(opt) != null)
                successAction();
            else if (TemplateMatch(failopt) != null)
                failureAction();
            else Compiler.ExceptionListener.Throw("Neither the success nor the failure image passed the check.");
        }
        public double[] GetScreenCoords(string temp, string[] prop)
        {
            var opt = GetOptionsFromStringArray(prop, temp);
            //Console.WriteLine(JsonConvert.SerializeObject(opt, Formatting.Indented));
            return TemplateMatch(opt);
        }
        private TemplateMatchOptions GetOptionsFromStringArray(string[] prop, string template)
        {
            var opt = new TemplateMatchOptions(template);
            if (prop == null)
                return opt;
            var thresh = prop.ElementAtOrDefault(0);
            var reduction = prop.ElementAtOrDefault(1);
            var ssp = prop.ElementAtOrDefault(2);
            var x = prop.ElementAtOrDefault(3);
            var y = prop.ElementAtOrDefault(4);
            var width = prop.ElementAtOrDefault(5);
            var height = prop.ElementAtOrDefault(6);
            var savepath = prop.ElementAtOrDefault(7);
            var threshint = 0;
            var reductint = 0;
            var sspenum = Panel.All;
            var xint = 0;
            var yint = 0;
            var widthint = 0;
            var heightint = 0;
            if (thresh != null)
                int.TryParse(thresh, out threshint);
            if (reduction != null)
                int.TryParse(reduction, out reductint);
            if (ssp != null)
                Enum.TryParse<Panel>(ssp, out sspenum);
            if (x != null)
                int.TryParse(x, out xint);
            if (y != null)
                int.TryParse(y, out yint);
            if (width != null)
                int.TryParse(width, out widthint);
            if (height != null)
                int.TryParse(height, out heightint);
            if (savepath != null)
                opt.SavePath = savepath;
            if (threshint != 0)
                opt.Threshold = threshint;
            if (reductint != 0)
                opt.Reduction = reductint;
            if (sspenum != Panel.All)
                opt.SplitScreenPanel = sspenum;
            if (xint != 0)
                opt.CustomX = xint;
            if (yint != 0)
                opt.CustomY = yint;
            if (heightint != 0)
                opt.CustomHeight = heightint;
            if (widthint != 0)
                opt.CustomWidth = widthint;
            return opt;
        }
    }
    internal class TemplateMatchOptions
    {
        //defaults
        public static int DefaultThreshold = 90;
        public static int DefaultReduction = 10;
        public static Panel DefaultSplitScreenPanel = Panel.All;
        public static int DefaultCustomX = 0;
        public static int DefaultCustomY = 0;
        public static int DefaultCustomWidth = 0;
        public static int DefaultCustomHeight = 0;
        //
        public Bitmap Template { get; }
        //percent of threshold ie a value of 90 = (90 / 100) = .9
        public int Threshold { get; set; }
        //percent of reduction ie a value of 30 = (30 / 100) = .3
        public int Reduction { get; set; }
        public Panel SplitScreenPanel { get; set; }
        public int CustomX { get; set; }
        public int CustomY { get; set; }
        public int CustomWidth { get; set; }
        public int CustomHeight { get; set; }
        public string SavePath { get; set; }

        public TemplateMatchOptions(string template)
        {
            Threshold = DefaultThreshold;
            Reduction = DefaultReduction;
            SplitScreenPanel = DefaultSplitScreenPanel;
            CustomX = DefaultCustomX;
            CustomY = DefaultCustomY;
            CustomWidth = DefaultCustomWidth;
            CustomHeight = DefaultCustomHeight;
            SavePath = "";
            Template = Utilities.GetImageFromPath(template);
        }
    }
    enum Panel { All, Top, Left, Right, Bottom, Q1, Q2, Q3, Q4, Custom }
}
