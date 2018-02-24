using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.Lang;
using TastyScript.Lang.Exceptions;

namespace TastyScript.Android
{
    public class Driver
    {
        public DeviceData Device { get; private set; }
        private string _appPackage = "";
        public string AppPackage { get { return _appPackage; } }
        private CancellationTokenSource _cancelationToken;
        public Driver(string input)
        {
            Device = AdbClient.Instance.GetDevices().FirstOrDefault(f => f.Serial == input);
            if (Device == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException, $"The device {input} could not be found. Make sure your adb client is loaded!\n Type the command 'devices' to see all the connected devices"));
                return;
            }
            IO.Output.Print($"Device {input} has been connected",ConsoleColor.DarkGreen);
            Console.Title = Program.Title + $" | Device {Device.Serial}";
            _cancelationToken = new CancellationTokenSource();
        }
        public void SetAppPackage(string appPackage)
        {
            _appPackage = appPackage;
            IO.Output.Print($"App Package set to {appPackage}",ConsoleColor.DarkGreen);
        }
        public void Tap(int x, int y)
        {
            if (AppPackage != "")
            {
                //halt software while app isnt in focus
                bool checkFocus = CheckFocusedApp();
                bool test = true;
                if (!checkFocus)
                {
                    test = false;
                    _cancelationToken.Cancel();
                }
                while (!checkFocus) { Thread.Sleep(5000); checkFocus = CheckFocusedApp(); }
                if (!test)
                {
                    //reup the cancelation token if it was canceled
                    _cancelationToken = new CancellationTokenSource();
                }
            }
            else
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException,
                    $"Warning! There is no app package set, events will not be paused when app loses focus. Check documentation for more info."), once: true);
            }
            ShellCommand($"input tap {x} {y}");
            //trying to prevent event bursts, especailly on load
            Thread.Sleep(300);
        }
        /// <summary>
        /// Send a keyevent via "input keyevent 'code'"
        /// </summary>
        /// <param name="code"></param>
        public void KeyEvent(AndroidKeyCode code)
        {
            if (AppPackage != "")
            {
                //halt software while app isnt in focus
                bool checkFocus = CheckFocusedApp();
                bool test = true;
                if (!checkFocus)
                {
                    test = false;
                    _cancelationToken.Cancel();
                }
                while (!checkFocus) { Thread.Sleep(5000); checkFocus = CheckFocusedApp(); }
                if (!test)
                {
                    //reup the cancelation token if it was canceled
                    _cancelationToken = new CancellationTokenSource();
                }
            }
            else
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException,
                    $"Warning! There is no app package set, events will not be paused when app loses focus. Check documentation for more info."), once: true);
            }
            ShellCommand($"input keyevent {(int)code}");
        }
        /// <summary>
        /// Send text via "input text 'string'"
        /// </summary>
        /// <param name="text"></param>
        public void SendText(string text)
        {
            if (AppPackage != "")
            {
                //halt software while app isnt in focus
                bool checkFocus = CheckFocusedApp();
                bool test = true;
                if (!checkFocus)
                {
                    test = false;
                    _cancelationToken.Cancel();
                }
                while (!checkFocus) { Thread.Sleep(5000); checkFocus = CheckFocusedApp(); }
                if (!test)
                {
                    //reup the cancelation token if it was canceled
                    _cancelationToken = new CancellationTokenSource();
                }
            }
            else
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException,
                    $"Warning! There is no app package set, events will not be paused when app loses focus. Check documentation for more info."), once: true);
            }
            ShellCommand($"input text {text.Replace(" ","%s")}");
        }
        public void Swipe(int x1, int y1, int x2, int y2, int duration)
        {
            if (AppPackage != "")
            {
                //halt software while app isnt in focus
                bool checkFocus = CheckFocusedApp();
                bool test = true;
                if (!checkFocus)
                {
                    test = false;
                    _cancelationToken.Cancel();
                }
                while (!checkFocus) { Thread.Sleep(5000); checkFocus = CheckFocusedApp(); }
                if (!test)
                {
                    //reup the cancelation token if it was canceled
                    _cancelationToken = new CancellationTokenSource();
                }
            }
            else
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException,
                    $"Warning! There is no app package set, events will not be paused when app loses focus. Check documentation for more info."), once: true);
            }
            ShellCommand($"input swipe {x1} {y1} {x2} {y2} {duration}");
        }
        public void LongPress(int x, int y, int duration)
        {
            if (AppPackage != "")
            {
                //halt software while app isnt in focus
                bool checkFocus = CheckFocusedApp();
                bool test = true;
                if (!checkFocus)
                {
                    test = false;
                    _cancelationToken.Cancel();
                }
                while (!checkFocus) { Thread.Sleep(5000); checkFocus = CheckFocusedApp(); }
                if (!test)
                {
                    //reup the cancelation token if it was canceled
                    _cancelationToken = new CancellationTokenSource();
                }
            }
            else
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException,
                    $"Warning! There is no app package set, events will not be paused when app loses focus. Check documentation for more info."), once: true);
            }
            ShellCommand($"input swipe {x} {y} {x} {y} {duration}");
        }
        public static void Test(string cmd)
        {
            using (AdbSocket socket = new AdbSocket(AdbClient.Instance.EndPoint))
            {
                socket.SendAdbRequest(cmd);
                var response = socket.ReadAdbResponse();
                IO.Output.Print(response.Message,ConsoleColor.Magenta);
            }
        }
        public Image GetScreenshot()
        {
            if (AppPackage != "")
            {
                //halt software while app isnt in focus
                bool checkFocus = CheckFocusedApp();
                bool test = true;
                if (!checkFocus)
                {
                    test = false;
                    _cancelationToken.Cancel();
                }
                while (!checkFocus) { Thread.Sleep(5000); checkFocus = CheckFocusedApp(); }
                if (!test)
                {
                    //reup the cancelation token if it was canceled
                    _cancelationToken = new CancellationTokenSource();
                }
            }
            else
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException,
                    $"Warning! There is no app package set, events will not be paused when app loses focus. Check documentation for more info."), once: true);
            }
            var screenshot = AdbClient.Instance.GetFrameBufferAsync(Device, CancellationToken.None).Result;
            return screenshot;
        }
        //a wrapper for the bulk of the shell command requirements
        private async void ShellCommand(string command)
        {
            try
            {
                var token = _cancelationToken.Token;
                var receiver = new ConsoleOutputReceiver();
                //AdbClient.Instance.ExecuteRemoteCommand(command, Device, receiver);
                await AdbClient.Instance.ExecuteRemoteCommandAsync(command, Device, receiver, token, 10);
            }
            catch
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException, $"Command {command} was canceled and will not be retried."));
            }
        }
        public string SendShellCommand(string c)
        {
            var receiver = new ConsoleOutputReceiver();
            AdbClient.Instance.ExecuteRemoteCommand(c, Device, receiver);
            return receiver.ToString();
        }
        public static void PrintAllDevices()
        {
            var devices = AdbClient.Instance.GetDevices();
            foreach (var device in devices)
            {
                IO.Output.Print(device.Serial);
            }
            
        }
        private bool CheckFocusedApp()
        {
            
                string command = $"dumpsys window windows | grep -E 'mCurrentFocus'";
                var receiver = new ConsoleOutputReceiver();
                AdbClient.Instance.ExecuteRemoteCommand(command, Device, receiver);
                var echo = receiver.ToString();
                if (echo.Contains(AppPackage))
                {
                    return true;
                }
                else
                {
                    //_cancelationToken.Cancel();
                    return false;
                }
            
        }
    }
    public enum AndroidKeyCode
    {
        Menu = 1,
        SoftRight = 2,
        Home = 3,
        Back = 4,
        Call = 5,
        EndCall = 6,
        Zero = 7,
        One = 8,
        Two = 9,
        Three = 10,
        Four = 11,
        Five = 12,
        Six = 13,
        Seven = 14,
        Eight = 15,
        Nine = 16,
        Star = 17,
        Pound = 18,
        DPadUp = 19,
        DPadDown = 20,
        DPadLeft = 21,
        DPadRight = 22,
        DPadCenter = 23,
        VolumeUp = 24,
        VolumeDown = 25,
        Power = 26,
        Camera = 27,
        Clear = 28,
        A = 29,
        B = 30,
        C = 31,
        D = 32,
        E = 33,
        F = 34,
        G = 35,
        H = 36,
        I = 37,
        J = 38,
        K = 39,
        L = 40,
        M = 41,
        N = 42,
        O = 43,
        P = 44,
        Q = 45,
        R = 46,
        S = 47,
        T = 48,
        U = 49,
        V = 50,
        W = 51,
        X = 52,
        Y = 53,
        Z = 54,
        Comma = 55,
        Period = 56,
        AltLeft = 57,
        AltRight = 58,
        ShiftLeft = 59,
        ShiftRight = 60,
        Tab = 61,
        Space = 62,
        Sym = 63,
        Explorer = 64,
        Envelope = 65,
        Enter = 66,
        Del = 67,
        Grave = 68,
        Minus = 69,
        Equals = 70,
        LeftBracket = 71,
        RightBracket = 72,
        BackSlash = 73,
        SemiColon = 74,
        Apostrophe = 75,
        Slash = 76,
        At = 77,
        Num = 78,
        HeadSetHook = 79,
        Focus = 80,
        Plus = 81,
        Menu2 = 82,
        Notification = 83,
        Search = 84
    }
}
