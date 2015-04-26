namespace WindowsFormsApplication1
{
    partial class ConnectionForm
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
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.userTextBox = new System.Windows.Forms.TextBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.serverLabel = new System.Windows.Forms.Label();
            this.userLabel = new System.Windows.Forms.Label();
            this.catalogTextBox = new System.Windows.Forms.TextBox();
            this.catalogLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // serverTextBox
            // 
            this.serverTextBox.Location = new System.Drawing.Point(19, 61);
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(237, 20);
            this.serverTextBox.TabIndex = 0;
            this.serverTextBox.Text = "eos.inf.ug.edu.pl";
            // 
            // userTextBox
            // 
            this.userTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.userTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.userTextBox.Location = new System.Drawing.Point(19, 168);
            this.userTextBox.Name = "userTextBox";
            this.userTextBox.Size = new System.Drawing.Size(237, 20);
            this.userTextBox.TabIndex = 1;
            this.userTextBox.Text = "mciesielski";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(19, 228);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(237, 20);
            this.passwordTextBox.TabIndex = 2;
            this.passwordTextBox.Text = "224626";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(69, 268);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(138, 46);
            this.connectButton.TabIndex = 3;
            this.connectButton.Text = "Połącz";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(20, 212);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(36, 13);
            this.passwordLabel.TabIndex = 4;
            this.passwordLabel.Text = "Hasło";
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(16, 45);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(74, 13);
            this.serverLabel.TabIndex = 5;
            this.serverLabel.Text = "Adres serwera";
            // 
            // userLabel
            // 
            this.userLabel.AutoSize = true;
            this.userLabel.Location = new System.Drawing.Point(20, 152);
            this.userLabel.Name = "userLabel";
            this.userLabel.Size = new System.Drawing.Size(62, 13);
            this.userLabel.TabIndex = 6;
            this.userLabel.Text = "Użytkownik";
            // 
            // catalogTextBox
            // 
            this.catalogTextBox.Location = new System.Drawing.Point(19, 118);
            this.catalogTextBox.Name = "catalogTextBox";
            this.catalogTextBox.Size = new System.Drawing.Size(237, 20);
            this.catalogTextBox.TabIndex = 7;
            this.catalogTextBox.Text = "mciesielski";
            // 
            // catalogLabel
            // 
            this.catalogLabel.AutoSize = true;
            this.catalogLabel.Location = new System.Drawing.Point(20, 102);
            this.catalogLabel.Name = "catalogLabel";
            this.catalogLabel.Size = new System.Drawing.Size(43, 13);
            this.catalogLabel.TabIndex = 8;
            this.catalogLabel.Text = "Katalog";
            // 
            // ConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 326);
            this.Controls.Add(this.catalogLabel);
            this.Controls.Add(this.catalogTextBox);
            this.Controls.Add(this.userLabel);
            this.Controls.Add(this.serverLabel);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.userTextBox);
            this.Controls.Add(this.serverTextBox);
            this.Name = "ConnectionForm";
            this.Text = "Nowe Połączenie";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox serverTextBox;
        private System.Windows.Forms.TextBox userTextBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.TextBox catalogTextBox;
        private System.Windows.Forms.Label catalogLabel;
    }
}