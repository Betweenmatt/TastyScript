namespace TastyScript.TastyScriptNPP
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
            this.outTextBox = new TastyScript.TastyScriptNPP.RTBToggleSelect();
            this.inputBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.startStopButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.clear_output = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // outTextBox
            // 
            this.outTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.outTextBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outTextBox.Location = new System.Drawing.Point(0, 29);
            this.outTextBox.Name = "outTextBox";
            this.outTextBox.ReadOnly = true;
            this.outTextBox.Size = new System.Drawing.Size(437, 427);
            this.outTextBox.TabIndex = 0;
            this.outTextBox.Text = "";
            // 
            // inputBox
            // 
            this.inputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputBox.Location = new System.Drawing.Point(254, 3);
            this.inputBox.Name = "inputBox";
            this.inputBox.Size = new System.Drawing.Size(122, 20);
            this.inputBox.TabIndex = 1;
            this.inputBox.WordWrap = false;
            this.inputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputBox_TextChanged);
            // 
            // sendButton
            // 
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.Location = new System.Drawing.Point(382, 3);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(46, 23);
            this.sendButton.TabIndex = 2;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // startStopButton
            // 
            this.startStopButton.Location = new System.Drawing.Point(0, 3);
            this.startStopButton.Name = "startStopButton";
            this.startStopButton.Size = new System.Drawing.Size(72, 23);
            this.startStopButton.TabIndex = 3;
            this.startStopButton.Text = "Start/Stop";
            this.startStopButton.UseVisualStyleBackColor = true;
            this.startStopButton.Click += new System.EventHandler(this.startStopButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(78, 3);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(44, 23);
            this.clearButton.TabIndex = 4;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // clear_output
            // 
            this.clear_output.Checked = true;
            this.clear_output.CheckState = System.Windows.Forms.CheckState.Checked;
            this.clear_output.Location = new System.Drawing.Point(128, 2);
            this.clear_output.Name = "clear_output";
            this.clear_output.Size = new System.Drawing.Size(104, 24);
            this.clear_output.TabIndex = 5;
            this.clear_output.Text = "Clear every run";
            this.clear_output.UseVisualStyleBackColor = true;
            this.clear_output.CheckedChanged += new System.EventHandler(this.clear_output_CheckedChanged);
            // 
            // Output
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 453);
            this.Controls.Add(this.clear_output);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.startStopButton);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.inputBox);
            this.Controls.Add(this.outTextBox);
            this.Name = "Output";
            this.Text = "TS Output";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RTBToggleSelect outTextBox;
        private System.Windows.Forms.TextBox inputBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Button startStopButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.CheckBox clear_output;
    }
}