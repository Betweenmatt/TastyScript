namespace TastyScriptNPP.Forms
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
            ((System.ComponentModel.ISupportInitialize)(this.bgColorPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaultTextPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // aboutLabel
            // 
            this.aboutLabel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.aboutLabel.Location = new System.Drawing.Point(2, 174);
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
            this.boldCheck.Location = new System.Drawing.Point(218, 59);
            this.boldCheck.Name = "boldCheck";
            this.boldCheck.Size = new System.Drawing.Size(47, 17);
            this.boldCheck.TabIndex = 5;
            this.boldCheck.Text = "Bold";
            this.boldCheck.UseVisualStyleBackColor = true;
            // 
            // italicCheck
            // 
            this.italicCheck.AutoSize = true;
            this.italicCheck.Location = new System.Drawing.Point(277, 59);
            this.italicCheck.Name = "italicCheck";
            this.italicCheck.Size = new System.Drawing.Size(48, 17);
            this.italicCheck.TabIndex = 6;
            this.italicCheck.Text = "Italic";
            this.italicCheck.UseVisualStyleBackColor = true;
            // 
            // fontSize
            // 
            this.fontSize.Location = new System.Drawing.Point(165, 57);
            this.fontSize.Name = "fontSize";
            this.fontSize.Size = new System.Drawing.Size(33, 20);
            this.fontSize.TabIndex = 7;
            // 
            // fontName
            // 
            this.fontName.Location = new System.Drawing.Point(165, 27);
            this.fontName.Name = "fontName";
            this.fontName.Size = new System.Drawing.Size(108, 20);
            this.fontName.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(104, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Text Font:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(104, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Text Size:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(263, 228);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // colorOverrideBox
            // 
            this.colorOverrideBox.Location = new System.Drawing.Point(191, 98);
            this.colorOverrideBox.Multiline = true;
            this.colorOverrideBox.Name = "colorOverrideBox";
            this.colorOverrideBox.Size = new System.Drawing.Size(149, 110);
            this.colorOverrideBox.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(188, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Color Overrides: ";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 263);
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
    }
}