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
        public Driver(string input)
        {
            Device = AdbClient.Instance.GetDevices().FirstOrDefault(f => f.Serial == input);
            if (Device == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException, $"The device {input} could not be found. Make sure your adb client is loaded!\n Type the command 'devices' to see all the connected devices"));
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Device {input} has been connected");
            Console.ResetColor();
            Console.Title = Program.Title + $" | Device {Device.Serial}";
        }
        public void Tap(int x, int y)
        {
            ShellCommand($"input tap {x} {y}");
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
            var receiver = new ConsoleOutputReceiver();
            //AdbClient.Instance.ExecuteRemoteCommand(command, Device, receiver);
            await AdbClient.Instance.ExecuteRemoteCommandAsync(command, Device, receiver, CancellationToken.None, 10);
        }
        public static void PrintAllDevices()
        {
            var devices = AdbClient.Instance.GetDevices();
            foreach (var device in devices)
            {
                Console.WriteLine(device.Serial);
            }
        }
    }
    public enum AndroidKeyCode
    {
        KEYCODE_HOME = 3,
        KEYCODE_BACK = 4
    }
}
