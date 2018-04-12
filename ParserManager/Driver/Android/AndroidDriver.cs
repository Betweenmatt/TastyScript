using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.Driver.Android
{
    public class AndroidDriver : IDriver
    {
        public DeviceData Device { get; private set; }
        private string _appPackage = "";
        public string AppPackage { get { return _appPackage; } }
        private CancellationTokenSource _cancelationToken;
        //generic constant data about the device that may be called during script execution
        public string ScreenWidth { get; private set; }
        public string ScreenHeight { get; private set; }

        public string Connect()
        {
            try
            {
                Device = AdbClient.Instance.GetDevices().FirstOrDefault();
            }
            catch
            {
                Manager.Throw($"There was an error with the driver. Make sure ADB has been started, and your device is connected!");
                return "";
            }
            if (Device == null)
            {
                Manager.Throw($"The device could not be found. Make sure your adb client is loaded!\n Type the command 'devices' to see all the connected devices");
                return "";
            }
            FetchDeviceData();
            Manager.Print($"The first device found has been connected", ConsoleColor.DarkGreen);
            //if (Main.IsConsole)
            //    Console.Title = Main.Title + $" | Device {Device.Serial}";
            Manager.IOStream.ChangeConsoleTitle($"Device: {Device.Serial}");
            _cancelationToken = new CancellationTokenSource();
            return Device.Serial;
        }
        public string Connect(string input)
        {
            try
            {
                Device = AdbClient.Instance.GetDevices().FirstOrDefault(f => f.Serial == input);
            }
            catch
            {
                Manager.Throw($"There was an error with the driver. Make sure ADB has been started, and your device is connected!");
                return "";
            }
            if (Device == null)
            {
                Manager.Throw($"The device {input} could not be found. Make sure your adb client is loaded!\n Type the command 'devices' to see all the connected devices");
                return "";
            }
            FetchDeviceData();
            Manager.Print($"Device {input} has been connected", ConsoleColor.DarkGreen);
            //if(Main.IsConsole)
            //    Console.Title = Main.Title + $" | Device {Device.Serial}";
            Manager.IOStream.ChangeConsoleTitle($"Device: {Device.Serial}");
            _cancelationToken = new CancellationTokenSource();
            return Device.Serial;
        }
        public bool IsConnected()
        {
            if (Device == null)
                return false;
            return true;
        }
        public string GetName()
        {
            if (Device != null)
                return Device.Serial;
            return "";
        }
        public string SetAppPackage(string appPackage)
        {
            if (appPackage != "")
            {
                _appPackage = appPackage;
                Manager.Print($"App Package set to {appPackage}", ConsoleColor.DarkGreen);
            }
            else
            {
                string command = $"dumpsys window windows | grep -E 'mCurrentFocus'";
                var receiver = new ConsoleOutputReceiver();
                AdbClient.Instance.ExecuteRemoteCommand(command, Device, receiver);
                var echo = receiver.ToString();
                _appPackage = echo;
                Manager.Print($"App Package set to the currently opened app.", ConsoleColor.DarkGreen);
            }
            return _appPackage;
        }
        /// <summary>
        /// This method fetches static device information that might be needed at runtime like screen size
        /// </summary>
        private void FetchDeviceData()
        {
            var width = SendShellCommand("dumpsys display | grep -E 'mDisplayWidth'");
            ScreenWidth = width.Split('=').ElementAtOrDefault(1).Replace("\r", "").Replace("\n", "").Replace("\t", "");

            var height = SendShellCommand("dumpsys display | grep -E 'mDisplayHeight'");
            ScreenHeight = height.Split('=').ElementAtOrDefault(1).Replace("\r", "").Replace("\n", "").Replace("\t", "");
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
                Manager.ThrowSilent($"Warning! There is no app package set, events will not be paused when app loses focus. Check documentation for more info.", once:true);
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
                Manager.Print(response.Message, ConsoleColor.Magenta);
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
                    if (watch.ElapsedMilliseconds >= 30000)
                    {
                        source.Cancel();
                    }
                }
                watch.Stop();
                return screenshot;
            }
            catch
            {
                Manager.ThrowSilent($"[131]GetFrameBuffer timed out.");
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
                Manager.ThrowSilent($"Command {command} was canceled and will not be retried.");
            }
        }
        public string SendShellCommand(string c)
        {
            var receiver = new ConsoleOutputReceiver();
            AdbClient.Instance.ExecuteRemoteCommand(c, Device, receiver);
            return receiver.ToString();
        }
        public void PrintAllDevices()
        {
            var devices = AdbClient.Instance.GetDevices();
            foreach (var device in devices)
            {
                Manager.Print(device.Serial);
            }

        }
        public bool CheckFocusedApp()
        {

            string command = $"dumpsys window windows | grep -E 'mCurrentFocus'";
            var receiver = new ConsoleOutputReceiver();
            try
            {
                AdbClient.Instance.ExecuteRemoteCommand(command, Device, receiver);
            }
            catch
            {
                Manager.ThrowSilent($"Focus check timed out. Please check your device connection.", once:true);
                return false;
            }
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
