using Kbg.NppPluginNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TastyScript.TastyScriptNPP
{
    public partial class SettingsPanel : Form
    {
        private Color currentBgColor;
        private Color currentTextColor;

        public SettingsPanel()
        {
            InitializeComponent();

            this.loglevelCombo.Items.Clear();
            this.loglevelCombo.Items.AddRange(new string[] { "warn", "error", "throw", "none" });

            this.aboutLabel.Text = "Developed by Matthew Andrews 2018\ngithub.com/Betweenmatt/TastyScript\n" + TastyScript.ParserManager.Manager.Title;
            this.bgColorPicture.BackColor = Settings.OutputPanel.DefaultBGColor;
            currentBgColor = Settings.OutputPanel.DefaultBGColor;
            this.defaultTextPicture.BackColor = Settings.OutputPanel.DefaultTextColor;
            currentTextColor = Settings.OutputPanel.DefaultTextColor;
            this.boldCheck.Checked = Settings.OutputPanel.Bold;
            this.italicCheck.Checked = Settings.OutputPanel.Italic;
            this.fontName.Text = Settings.OutputPanel.FontName;
            this.fontSize.Text = Settings.OutputPanel.FontSize.ToString();
            this.loglevelCombo.SelectedIndex = this.loglevelCombo.FindStringExact(Settings.OutputPanel.LogLevel);
            this.colorOverrideBox.Text = Settings.OutputPanel.ColorOverrides;
            this.tsfolder_input.Text = Settings.OutputPanel.TSFolder;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.OutputPanel.DefaultBGColor = currentBgColor;
                Settings.OutputPanel.DefaultTextColor = currentTextColor;
                Settings.OutputPanel.Bold = this.boldCheck.Checked;
                Settings.OutputPanel.Italic = this.italicCheck.Checked;
                Settings.OutputPanel.LogLevel = this.loglevelCombo.Text;
                Settings.OutputPanel.TSFolder = this.tsfolder_input.Text;
                try
                {
                    Settings.OutputPanel.FontSize = int.Parse(this.fontSize.Text);
                }
                catch
                {
                    MessageBox.Show("Font size must be an integer");
                    return;
                }
                Settings.OutputPanel.FontName = this.fontName.Text;
                Settings.OutputPanel.ColorOverrides = this.colorOverrideBox.Text;
                //reload settings if output is not null
                if (Main.output != null)
                    Main.output.LoadSettings();
                Main.HideSettings();
            }catch(Exception ex)
            {
                MessageBox.Show($"There was an error saving settings: {ex}");
            }
        }

        private void bgColorPicture_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                currentBgColor = colorDialog1.Color;
                this.bgColorPicture.BackColor = currentBgColor;
            }
        }

        private void defaultTextPicture_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                currentTextColor = colorDialog1.Color;
                this.defaultTextPicture.BackColor = currentTextColor;
            }
        }

        private void browse_button_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.tsfolder_input.Text = folderBrowserDialog1.SelectedPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"There was an error selecting a folder: {ex}");
                }
            }
        }
    }
}