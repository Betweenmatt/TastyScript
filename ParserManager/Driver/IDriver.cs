using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.Driver
{
    public interface IDriver
    {
        string SetAppPackage(string appPackage);
        void SendCommand(string cmd);
        Task<Image> GetScreenshot();
        string Connect();
        string Connect(string input);
        string ScreenWidth { get; }
        string ScreenHeight { get; }
        void PrintAllDevices();
        bool IsConnected();
        string GetName();
    }
}
