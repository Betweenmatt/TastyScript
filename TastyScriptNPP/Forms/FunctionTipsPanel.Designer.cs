namespace TastyScriptNPP.Forms
{
    partial class FunctionTipsPanel
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
            this.fTipsList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // fTipsList
            // 
            this.fTipsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fTipsList.FormattingEnabled = true;
            this.fTipsList.Location = new System.Drawing.Point(0, 38);
            this.fTipsList.Name = "fTipsList";
            this.fTipsList.Size = new System.Drawing.Size(801, 407);
            this.fTipsList.TabIndex = 0;
            // 
            // FunctionTipsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.fTipsList);
            this.Name = "FunctionTipsPanel";
            this.Text = "FunctionTipsPanel";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox fTipsList;
    }
}