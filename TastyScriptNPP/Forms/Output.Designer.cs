using TastyScriptNPP;

namespace Kbg.NppPluginNET
{
    partial class Output
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.outTextBox = new TastyScriptNPP.RTBToggleSelect();
            this.SuspendLayout();
            // 
            // outTextBox
            // 
            this.outTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.outTextBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outTextBox.Location = new System.Drawing.Point(0, 0);
            this.outTextBox.Name = "outTextBox";
            this.outTextBox.ReadOnly = true;
            this.outTextBox.Size = new System.Drawing.Size(287, 265);
            this.outTextBox.TabIndex = 0;
            this.outTextBox.Text = "";
            // 
            // Output
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.outTextBox);
            this.Name = "Output";
            this.Text = "TS Output";
            this.ResumeLayout(false);

        }

        #endregion

        private RTBToggleSelect outTextBox;
    }
}