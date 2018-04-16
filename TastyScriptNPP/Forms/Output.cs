using Kbg.NppPluginNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TastyScript.TastyScriptNPP;

namespace TastyScript.TastyScriptNPP
{
    public partial class Output : Form
    {
        public Output()
        {
            InitializeComponent();
            LoadSettings();
        }

        public void LoadSettings()
        {
            this.outTextBox.BackColor = Settings.OutputPanel.DefaultBGColor;
            this.outTextBox.ForeColor = Settings.OutputPanel.DefaultTextColor;
            this.outTextBox.Font = new Font(
                Settings.OutputPanel.FontName,
                Settings.OutputPanel.FontSize,
                ((FontStyle)((((Settings.OutputPanel.Bold) ? FontStyle.Bold : FontStyle.Regular) |
                ((Settings.OutputPanel.Italic) ? FontStyle.Italic : FontStyle.Regular))))
                );
        }

        public void Pipe(string msg, bool nline = true)
        {
            if (nline)
                msg = msg + "\n";
            this.outTextBox.Invoke((MethodInvoker)delegate
            {
                this.outTextBox.AppendText(msg);
                if (this.outTextBox.Visible && nline)
                {
                    this.outTextBox.ScrollToCaret();
                }
            });
        }

        public void Pipe(string msg, Color color, bool nline = true)
        {
            if (nline)
                msg = msg + "\n";

            this.outTextBox.Invoke((MethodInvoker)delegate
            {
                outTextBox.Selectable = false;
                int start = this.outTextBox.TextLength;
                this.outTextBox.AppendText(msg);
                if (this.outTextBox.Visible && nline)
                {
                    this.outTextBox.ScrollToCaret();
                }
                int end = this.outTextBox.TextLength;

                this.outTextBox.Select(start, end - start);

                this.outTextBox.SelectionColor = color;

                this.outTextBox.SelectionLength = 0; // clear
                outTextBox.Selectable = true;
            });
        }

        public void Clear()
        {
            this.outTextBox.Clear();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void SendLine()
        {
            var str = this.inputBox.Text.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            IOStream.Instance.SendStdIn(str);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            SendLine();
            this.inputBox.Text = "";
        }

        private void inputBox_TextChanged(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SendLine();
            }
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            Main.RunStopTS();
        }

        private void clear_output_CheckedChanged(object sender, EventArgs e)
        {
            Settings.OutputPanel.ClearOutputOnRun = this.clear_output.Checked;
        }
    }
}