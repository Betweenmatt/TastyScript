using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

//using TastyScript.ParserManager;
using TastyScript.TastyScriptNPP;
using TastyScript;

namespace Kbg.NppPluginNET
{
    internal class Main
    {
        internal const string PluginName = "TastyScriptNPP";
        private static string iniFilePath = null;
        internal static Output output = null;
        public static string IniPath { get; private set; }
        private static int outputDialogId = -1;
        private static int settingsDialogId = -1;

        private static Bitmap playButton = TastyScript.TastyScriptNPP.Properties.Resources.icoRaw;

        private static Bitmap consoleButton = TastyScript.TastyScriptNPP.Properties.Resources.console_24;
        private static Icon tbIcon = null;
        private static bool IsRunning;

        public static void OnNotification(ScNotification notification)
        {
            // This method is invoked whenever something is happening in notepad++ use eg. as if
            // (notification.Header.Code == (uint)NppMsg.NPPN_xxx) { ... } or
            //
            // if (notification.Header.Code == (uint)SciMsg.SCNxxx) { ... }
        }

        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            IniPath = iniFilePath;
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");

            Settings.OutputPanel.SetDefaultBGColor(Win32.GetPrivateProfileInt("OutputPanel", "BackgroundColor", Color.LightGray.ToArgb(), iniFilePath));
            Settings.OutputPanel.SetDefaultTextColor(Win32.GetPrivateProfileInt("OutputPanel", "DefaultTextColor", Color.Black.ToArgb(), iniFilePath));
            Settings.OutputPanel.Bold = (Win32.GetPrivateProfileInt("OutputPanel", "BoldStyle", 0, iniFilePath) != 0);
            Settings.OutputPanel.Italic = (Win32.GetPrivateProfileInt("OutputPanel", "ItalicStyle", 0, iniFilePath) != 0);
            Settings.OutputPanel.FontSize = (Win32.GetPrivateProfileInt("OutputPanel", "FontSize", 12, iniFilePath));
            Settings.OutputPanel.ClearOutputOnRun = (Win32.GetPrivateProfileInt("OutputPanel", "AutoClearOutput", 1, iniFilePath) != 0);
            //
            StringBuilder fontNameBuilder = new StringBuilder(32767);
            Win32.GetPrivateProfileString("OutputPanel", "FontName", "Consolas", fontNameBuilder, 32767, iniFilePath);
            Settings.OutputPanel.FontName = fontNameBuilder.ToString();
            //
            StringBuilder tsFolderBuilder = new StringBuilder(32767);
            Win32.GetPrivateProfileString("OutputPanel", "TSFolder", "", tsFolderBuilder, 32767, iniFilePath);
            Settings.OutputPanel.TSFolder = tsFolderBuilder.ToString();
            //
            StringBuilder colorOverrides = new StringBuilder(32767);
            string coloroverridedefault = "Black,LightGray;Blue,Blue;Cyan,Cyan;DarkBlue,DarkBlue;DarkCyan,DarkCyan;DarkGray,DarkGray;DarkGreen,DarkGreen;" +
                "DarkMagenta,DarkMagenta;DarkRed,DarkRed;DarkYellow,YellowGreen;Green,Green;Magenta,Magenta;Red,Red;White,White;Yellow,Yellow;";
            Win32.GetPrivateProfileString("OutputPanel", "ColorOverrides", coloroverridedefault, colorOverrides, 32767, iniFilePath);
            Settings.OutputPanel.ColorOverrides = colorOverrides.ToString();
            //
            StringBuilder llStrBuilder = new StringBuilder(32767);
            Win32.GetPrivateProfileString("OutputPanel", "LogLevel", "warn", llStrBuilder, 32767, iniFilePath);
            Settings.OutputPanel.LogLevel = llStrBuilder.ToString();

            PluginBase.SetCommand(0, "Run/Stop Script", RunStopTS, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(1, "Output Panel", OutputDockableDialog); outputDialogId = 1;
            PluginBase.SetCommand(3, "---", null);
            PluginBase.SetCommand(4, "Settings", SettingsDialog); settingsDialogId = 4;
        }

        internal static void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = playButton.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[0]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        internal static void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("OutputPanel", "BackgroundColor", Settings.OutputPanel.DefaultBGColor.ToArgb().ToString(), iniFilePath);
            Win32.WritePrivateProfileString("OutputPanel", "DefaultTextColor", Settings.OutputPanel.DefaultTextColor.ToArgb().ToString(), iniFilePath);
            Win32.WritePrivateProfileString("OutputPanel", "BoldStyle", Settings.OutputPanel.Bold ? "1" : "0", iniFilePath);
            Win32.WritePrivateProfileString("OutputPanel", "ItalicStyle", Settings.OutputPanel.Italic ? "1" : "0", iniFilePath);
            Win32.WritePrivateProfileString("OutputPanel", "FontSize", Settings.OutputPanel.FontSize.ToString(), iniFilePath);
            Win32.WritePrivateProfileString("OutputPanel", "FontName", Settings.OutputPanel.FontName, iniFilePath);
            Win32.WritePrivateProfileString("OutputPanel", "ColorOverrides", Settings.OutputPanel.ColorOverrides, iniFilePath);
            Win32.WritePrivateProfileString("OutputPanel", "LogLevel", Settings.OutputPanel.LogLevel, iniFilePath);
            Win32.WritePrivateProfileString("OutputPanel", "TSFolder", Settings.OutputPanel.TSFolder, iniFilePath);
            Win32.WritePrivateProfileString("OutputPanel", "AutoClearOutput", Settings.OutputPanel.ClearOutputOnRun ? "1" : "0", iniFilePath);
        }

        internal static Process TsProcess;

        internal static void RunStopTS()
        {
            if (!IsRunning)
            {
                OutputDockableDialog();
                if (output != null)
                {
                    var str = new IOStream(output);
                    if(Settings.OutputPanel.ClearOutputOnRun)
                        output.Clear();
                    //check for correct extension before continuing
                    StringBuilder ext = new StringBuilder(Win32.MAX_PATH);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETEXTPART, 0, ext);
                    if (ext.ToString() != ".ts")
                    {
                        DialogResult dialogResult = MessageBox.Show("This file is not `.ts`, are you sure you wish to run it?", "Unknown extension", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            //continue
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }
                    if(Settings.OutputPanel.TSFolder == "" || !File.Exists(Settings.OutputPanel.TSFolder + @"\TastyScript.exe"))
                    {
                        DialogResult dialogResult = MessageBox.Show("Your TastyScript folder has not been set yet! Would you like to go to settings now to set it?", "TS Folder Not Set", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            SettingsDialog();
                            return;
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }
                    IsRunning = true;
                    StringBuilder path = new StringBuilder(Win32.MAX_PATH);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFULLCURRENTPATH, 0, path);
                    str.Print("File Path : " + path, ConsoleColor.DarkGray);
                    StringBuilder dir = new StringBuilder(Win32.MAX_PATH);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTDIRECTORY, 0, dir);
                    str.Print("Directory : " + dir, ConsoleColor.DarkGray);
                    Thread th = new Thread(() =>
                    {
                       try
                        {
                            StartTsProcess(dir.ToString(), path.ToString(), str);
                            StopTsProcess();
                        }catch(Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                        TsProcess.WaitForExit();
                        str.Print("Execution is complete.", ConsoleColor.Green);
                        IsRunning = false;
                    });
                    th.Start();
                }
            }
            else
            {
                StopTsProcess();
                IsRunning = false;
            }
        }

        private static void StartTsProcess(string dir, string path, IOStream iostream)
        {
            TsProcess = new Process();

            // Stop the process from opening a new window
            TsProcess.StartInfo.RedirectStandardOutput = true;
            TsProcess.StartInfo.RedirectStandardInput = true;
            TsProcess.StartInfo.UseShellExecute = false;
            TsProcess.StartInfo.CreateNoWindow = true;

            // Setup executable and parameters
            TsProcess.StartInfo.FileName = Settings.OutputPanel.TSFolder + @"\TastyScript.exe";
            TsProcess.StartInfo.Arguments = $"-r \"{path}\" -d \"{dir}\" -ll {Settings.OutputPanel.LogLevel}";

            // Go
            TsProcess.Start();
            TastyScript.ParserManager.ChildProcessTracker.AddProcess(TsProcess);
            while (!TsProcess.StandardOutput.EndOfStream)
            {
                string line = TsProcess.StandardOutput.ReadLine();
                var xml = TastyScript.ParserManager.IOStream.XmlStreamObj.ReadStreamXml(line);
                if (xml == null)
                {
                    iostream.Print(line);
                }
                else
                {
                    iostream.PrintXml(xml);
                }
            }
        }

        private static void StopTsProcess()
        {
            StreamWriter streamWriter = TsProcess.StandardInput;
            streamWriter.WriteLine("".ToStreamXml(id: "PROCESS_SCRIPT_ESCAPE"));
        }

        internal static void OutputDockableDialog()
        {
            if (output == null)
            {
                output = new Output();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(playButton, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = output.Handle;
                _nppTbData.pszName = "Output - " + TastyScript.ParserManager.Manager.Title;
                _nppTbData.dlgID = outputDialogId;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, output.Handle);
            }
        }

        private static SettingsPanel settings = null;

        internal static void SettingsDialog()
        {
            if (settings == null)
            {
                settings = new SettingsPanel();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(playButton, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = settings.Handle;
                _nppTbData.pszName = "TastyScript Settings";
                _nppTbData.dlgID = settingsDialogId;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, settings.Handle);
            }
        }

        internal static void HideSettings()
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, settings.Handle);
            PluginCleanUp();
        }
    }
}