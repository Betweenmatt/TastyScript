using Kbg.NppPluginNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TastyScriptNPP.Forms
{
    public partial class SettingsPanel : Form
    {
        private Color currentBgColor;
        private Color currentTextColor;
        public SettingsPanel()
        {
            InitializeComponent();
            this.aboutLabel.Text = "TastyScript - Matthew Andrews 2018\ngithub.com/TastyGod/TastyScript\n" + TastyScript.Main.Title;
            this.bgColorPicture.BackColor = Settings.OutputPanel.DefaultBGColor;
            currentBgColor = Settings.OutputPanel.DefaultBGColor;
            this.defaultTextPicture.BackColor = Settings.OutputPanel.DefaultTextColor;
            currentTextColor = Settings.OutputPanel.DefaultTextColor;
            this.boldCheck.Checked = Settings.OutputPanel.Bold;
            this.italicCheck.Checked = Settings.OutputPanel.Italic;
            this.fontName.Text = Settings.OutputPanel.FontName;
            this.fontSize.Text = Settings.OutputPanel.FontSize.ToString();
            this.colorOverrideBox.Text = Settings.OutputPanel.ColorOverrides;
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            Settings.OutputPanel.DefaultBGColor = currentBgColor;
            Settings.OutputPanel.DefaultTextColor = currentTextColor;
            Settings.OutputPanel.Bold = this.boldCheck.Checked;
            Settings.OutputPanel.Italic = this.italicCheck.Checked;
            try
            {
                Settings.OutputPanel.FontSize = int.Parse(this.fontSize.Text);
            }
            catch
            {
                MessageBox.Show("Font size must be an integer");
            }
            Settings.OutputPanel.FontName = this.fontName.Text;
            Settings.OutputPanel.ColorOverrides = this.colorOverrideBox.Text;
            //reload settings if output is not null
            if (Main.output != null)
                Main.output.LoadSettings();
            Main.HideSettings();
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

        
    }
}
