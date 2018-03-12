using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript
{
    internal class Settings
    {
        private static string quickDir;
        public static string QuickDirectory { get { return quickDir; } }
        private static string logLevel;
        public static string LogLevel { get { return logLevel; } }
        private static bool remoteToggle;
        public static bool RemoteToggle { get { return remoteToggle; } }
        
        public static void SetQuickDirectory(string dir)
        {
            quickDir = dir;
            Properties.Settings.Default.dir = dir;
            Properties.Settings.Default.Save();
        }
        public static void SetLogLevel(string level)
        {
            logLevel = level;
            Properties.Settings.Default.loglevel = level;
            Properties.Settings.Default.Save();
        }
        public static void SetRemoteToggle(bool toggle)
        {
            remoteToggle = toggle;
            Properties.Settings.Default.remote = toggle;
            Properties.Settings.Default.Save();
        }
        public static void LoadSettings()
        {
            quickDir = Properties.Settings.Default.dir;
            logLevel = Properties.Settings.Default.loglevel;
            remoteToggle = Properties.Settings.Default.remote;
        }
    }
}
