namespace TastyScript.TastyScriptNPP
{
    partial class SettingsPanel
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
            this.aboutLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bgColorPicture = new System.Windows.Forms.PictureBox();
            this.defaultTextPicture = new System.Windows.Forms.PictureBox();
            this.boldCheck = new System.Windows.Forms.CheckBox();
            this.italicCheck = new System.Windows.Forms.CheckBox();
            this.fontSize = new System.Windows.Forms.TextBox();
            this.fontName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.saveButton = new System.Windows.Forms.Button();
            this.colorOverrideBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.loglevelCombo = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tsfolder_input = new System.Windows.Forms.TextBox();
            this.browse_button = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.bgColorPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaultTextPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // aboutLabel
            // 
            this.aboutLabel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.aboutLabel.Location = new System.Drawing.Point(2, 222);
            this.aboutLabel.Name = "aboutLabel";
            this.aboutLabel.Size = new System.Drawing.Size(183, 77);
            this.aboutLabel.TabIndex = 0;
            this.aboutLabel.Text = "About";
            this.aboutLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Background Color";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Default Text Color";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bgColorPicture
            // 
            this.bgColorPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bgColorPicture.Location = new System.Drawing.Point(32, 27);
            this.bgColorPicture.Name = "bgColorPicture";
            this.bgColorPicture.Size = new System.Drawing.Size(60, 46);
            this.bgColorPicture.TabIndex = 3;
            this.bgColorPicture.TabStop = false;
            this.bgColorPicture.Click += new System.EventHandler(this.bgColorPicture_Click);
            // 
            // defaultTextPicture
            // 
            this.defaultTextPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.defaultTextPicture.Location = new System.Drawing.Point(32, 98);
            this.defaultTextPicture.Name = "defaultTextPicture";
            this.defaultTextPicture.Size = new System.Drawing.Size(60, 50);
            this.defaultTextPicture.TabIndex = 4;
            this.defaultTextPicture.TabStop = false;
            this.defaultTextPicture.Click += new System.EventHandler(this.defaultTextPicture_Click);
            // 
            // boldCheck
            // 
            this.boldCheck.AutoSize = true;
            this.boldCheck.Location = new System.Drawing.Point(340, 38);
            this.boldCheck.Name = "boldCheck";
            this.boldCheck.Size = new System.Drawing.Size(47, 17);
            this.boldCheck.TabIndex = 5;
            this.boldCheck.Text = "Bold";
            this.boldCheck.UseVisualStyleBackColor = true;
            // 
            // italicCheck
            // 
            this.italicCheck.AutoSize = true;
            this.italicCheck.Location = new System.Drawing.Point(340, 61);
            this.italicCheck.Name = "italicCheck";
            this.italicCheck.Size = new System.Drawing.Size(48, 17);
            this.italicCheck.TabIndex = 6;
            this.italicCheck.Text = "Italic";
            this.italicCheck.UseVisualStyleBackColor = true;
            // 
            // fontSize
            // 
            this.fontSize.Location = new System.Drawing.Point(230, 42);
            this.fontSize.Name = "fontSize";
            this.fontSize.Size = new System.Drawing.Size(33, 20);
            this.fontSize.TabIndex = 7;
            // 
            // fontName
            // 
            this.fontName.Location = new System.Drawing.Point(230, 12);
            this.fontName.Name = "fontName";
            this.fontName.Size = new System.Drawing.Size(130, 20);
            this.fontName.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(169, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Text Font:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Text Size:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(313, 276);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // colorOverrideBox
            // 
            this.colorOverrideBox.Location = new System.Drawing.Point(172, 119);
            this.colorOverrideBox.Multiline = true;
            this.colorOverrideBox.Name = "colorOverrideBox";
            this.colorOverrideBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.colorOverrideBox.Size = new System.Drawing.Size(215, 86);
            this.colorOverrideBox.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(169, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Color Overrides: ";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loglevelCombo
            // 
            this.loglevelCombo.FormattingEnabled = true;
            this.loglevelCombo.Location = new System.Drawing.Point(231, 79);
            this.loglevelCombo.Name = "loglevelCombo";
            this.loglevelCombo.Size = new System.Drawing.Size(121, 21);
            this.loglevelCombo.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(170, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "LogLevel:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(191, 222);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Tasty Script Folder: ";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tsfolder_input
            // 
            this.tsfolder_input.Location = new System.Drawing.Point(194, 238);
            this.tsfolder_input.Name = "tsfolder_input";
            this.tsfolder_input.Size = new System.Drawing.Size(130, 20);
            this.tsfolder_input.TabIndex = 16;
            // 
            // browse_button
            // 
            this.browse_button.Location = new System.Drawing.Point(330, 236);
            this.browse_button.Name = "browse_button";
            this.browse_button.Size = new System.Drawing.Size(57, 23);
            this.browse_button.TabIndex = 18;
            this.browse_button.Text = "Browse";
            this.browse_button.UseVisualStyleBackColor = true;
            this.browse_button.Click += new System.EventHandler(this.browse_button_Click);
            // 
            // SettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 308);
            this.Controls.Add(this.browse_button);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tsfolder_input);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.loglevelCombo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.colorOverrideBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fontName);
            this.Controls.Add(this.fontSize);
            this.Controls.Add(this.italicCheck);
            this.Controls.Add(this.boldCheck);
            this.Controls.Add(this.defaultTextPicture);
            this.Controls.Add(this.bgColorPicture);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.aboutLabel);
            this.Name = "SettingsPanel";
            this.Text = "SettingsPanel";
            ((System.ComponentModel.ISupportInitialize)(this.bgColorPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaultTextPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label aboutLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox bgColorPicture;
        private System.Windows.Forms.PictureBox defaultTextPicture;
        private System.Windows.Forms.CheckBox boldCheck;
        private System.Windows.Forms.CheckBox italicCheck;
        private System.Windows.Forms.TextBox fontSize;
        private System.Windows.Forms.TextBox fontName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox colorOverrideBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox loglevelCombo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tsfolder_input;
        private System.Windows.Forms.Button browse_button;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}