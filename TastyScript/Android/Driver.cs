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
                _cancelationToken = new CancellationTokenSource();
            }
            
            ShellCommand($"input tap {x} {y}");
            //trying to prevent event bursts, especailly on load
            Thread.Sleep(300);
        }
        public void KeyEvent(AndroidKeyCode code)
        {
            ShellCommand($"input keyevent {(int)code}");
        }
        public Image GetScreenshot()
        {
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
            catch (Exception e)
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
            if (AppPackage == "")
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.DriverException,
                    $"Warning! There is no app package set, events will not be paused when app loses focus. Check documentation for more info."),once:true);
                return true;
            }
            else
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
    }
    public enum AndroidKeyCode
    {
        KEYCODE_HOME = 3,
        KEYCODE_BACK = 4
    }
}
