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
    internal class Driver
    {
        public DeviceData Device { get; private set; }
        private string _appPackage = "";
        public string AppPackage { get { return _appPackage; } }
        private CancellationTokenSource _cancelationToken;
        public Driver(string input)
        {
            if (input != "")
            {
                Device = AdbClient.Instance.GetDevices().FirstOrDefault(f => f.Serial == input);
                if (Device == null)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                        $"The device {input} could not be found. Make sure your adb client is loaded!\n Type the command 'devices' to see all the connected devices"));
                    return;
                }
                IO.Output.Print($"Device {input} has been connected", ConsoleColor.DarkGreen);
                Console.Title = Program.Title + $" | Device {Device.Serial}";
                _cancelationToken = new CancellationTokenSource();
            }
            else
            {
                Device = AdbClient.Instance.GetDevices().FirstOrDefault();
                if (Device == null)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                        $"The device could not be found. Make sure your adb client is loaded!\n Type the command 'devices' to see all the connected devices"));
                    return;
                }
                IO.Output.Print($"Device first device found has been connected", ConsoleColor.DarkGreen);
                Console.Title = Program.Title + $" | Device {Device.Serial}";
                _cancelationToken = new CancellationTokenSource();
            }
        }
        public void SetAppPackage(string appPackage)
        {
            if (appPackage != "")
            {
                _appPackage = appPackage;
                IO.Output.Print($"App Package set to {appPackage}", ConsoleColor.DarkGreen);
            }
            else
            {
                string command = $"dumpsys window windows | grep -E 'mCurrentFocus'";
                var receiver = new ConsoleOutputReceiver();
                AdbClient.Instance.ExecuteRemoteCommand(command, Device, receiver);
                var echo = receiver.ToString();
                _appPackage = echo;
                IO.Output.Print($"App Package set to the currently opened app.", ConsoleColor.DarkGreen);
            }
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
        
        public void SendCommand(string cmd)
        {
            CheckAppPackage();
            ShellCommand(cmd);
            Thread.Sleep(300);
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
        public bool CheckFocusedApp()
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
