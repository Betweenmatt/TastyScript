using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                    $"The device {input} could not be found. Make sure your adb client is loaded!\n Type the command 'devices' to see all the connected devices"));
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
        //all compile time shell commands(tap/getscreenshot/etc) check for app package before continuing
        private void CheckAppPackage()
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
        }
        public void Tap(int x, int y)
        {
            CheckAppPackage();
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
            CheckAppPackage();
            ShellCommand($"input keyevent {(int)code}");
        }
        /// <summary>
        /// Send text via "input text 'string'"
        /// </summary>
        /// <param name="text"></param>
        public void SendText(string text)
        {
            CheckAppPackage();
            ShellCommand($"input text {text.Replace(" ","%s")}");
        }
        public void Swipe(int x1, int y1, int x2, int y2, int duration)
        {
            CheckAppPackage();
            ShellCommand($"input swipe {x1} {y1} {x2} {y2} {duration}");
        }
        public void LongPress(int x, int y, int duration)
        {
            CheckAppPackage();
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
        public async Task<Image> GetScreenshot()
        {
            CheckAppPackage();
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                //run as async,but block the main thread with the timeout.
                var screenshot = await AdbClient.Instance.GetFrameBufferAsync(Device, source.Token);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (screenshot == null)
                {
                    if(watch.ElapsedMilliseconds >= 30000)
                    {
                        source.Cancel();
                    }
                }
                watch.Stop();
                return screenshot;
            }
            catch
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException,
                    $"[131]GetFrameBuffer timed out."));
            }
            return null;
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
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException,
                    $"Command {command} was canceled and will not be retried."));
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
                    return false;
                }
            
        }
    }
}
