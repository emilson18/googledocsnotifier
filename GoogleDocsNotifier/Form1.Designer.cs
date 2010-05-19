namespace GoogleDocsNotifier
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timer_update = new System.Windows.Forms.Timer(this.components);
            this.notify_icon = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel_signin = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.textbox_password = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textbox_username = new System.Windows.Forms.TextBox();
            this.btn_signin = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbl_error = new System.Windows.Forms.Label();
            this.btn_signout = new System.Windows.Forms.Button();
            this.listview_documents = new System.Windows.Forms.ListView();
            this.clm_document = new System.Windows.Forms.ColumnHeader();
            this.clm_updated_date = new System.Windows.Forms.ColumnHeader();
            this.clm_author = new System.Windows.Forms.ColumnHeader();
            this.label5 = new System.Windows.Forms.Label();
            this.panel_signin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(9, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Recently Updated Google Docs";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(218, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "<Google Docs Status Here>";
            // 
            // timer_update
            // 
            this.timer_update.Interval = 15000;
            this.timer_update.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notify_icon
            // 
            this.notify_icon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notify_icon.Icon = ((System.Drawing.Icon)(resources.GetObject("notify_icon.Icon")));
            this.notify_icon.Text = "Google Docs Notifier";
            this.notify_icon.Visible = true;
            this.notify_icon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // panel_signin
            // 
            this.panel_signin.Controls.Add(this.lbl_error);
            this.panel_signin.Controls.Add(this.pictureBox1);
            this.panel_signin.Controls.Add(this.btn_signin);
            this.panel_signin.Controls.Add(this.textbox_username);
            this.panel_signin.Controls.Add(this.label4);
            this.panel_signin.Controls.Add(this.textbox_password);
            this.panel_signin.Controls.Add(this.label3);
            this.panel_signin.Location = new System.Drawing.Point(0, 0);
            this.panel_signin.Name = "panel_signin";
            this.panel_signin.Size = new System.Drawing.Size(804, 271);
            this.panel_signin.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(258, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Google Docs Account";
            // 
            // textbox_password
            // 
            this.textbox_password.Location = new System.Drawing.Point(376, 122);
            this.textbox_password.Name = "textbox_password";
            this.textbox_password.PasswordChar = '*';
            this.textbox_password.Size = new System.Drawing.Size(150, 20);
            this.textbox_password.TabIndex = 3;
            this.textbox_password.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_password_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(258, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password";
            // 
            // textbox_username
            // 
            this.textbox_username.Location = new System.Drawing.Point(376, 96);
            this.textbox_username.Name = "textbox_username";
            this.textbox_username.Size = new System.Drawing.Size(150, 20);
            this.textbox_username.TabIndex = 2;
            // 
            // btn_signin
            // 
            this.btn_signin.Location = new System.Drawing.Point(376, 148);
            this.btn_signin.Name = "btn_signin";
            this.btn_signin.Size = new System.Drawing.Size(64, 21);
            this.btn_signin.TabIndex = 7;
            this.btn_signin.Text = "Sign In";
            this.btn_signin.UseVisualStyleBackColor = true;
            this.btn_signin.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GoogleDocsNotifier.Properties.Resources.Google_Docs_Notifier_Logo2;
            this.pictureBox1.Location = new System.Drawing.Point(26, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(204, 218);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // lbl_error
            // 
            this.lbl_error.AutoSize = true;
            this.lbl_error.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_error.ForeColor = System.Drawing.Color.Red;
            this.lbl_error.Location = new System.Drawing.Point(532, 125);
            this.lbl_error.Name = "lbl_error";
            this.lbl_error.Size = new System.Drawing.Size(0, 13);
            this.lbl_error.TabIndex = 8;
            // 
            // btn_signout
            // 
            this.btn_signout.Location = new System.Drawing.Point(730, 9);
            this.btn_signout.Name = "btn_signout";
            this.btn_signout.Size = new System.Drawing.Size(64, 21);
            this.btn_signout.TabIndex = 11;
            this.btn_signout.Text = "Sign Out";
            this.btn_signout.UseVisualStyleBackColor = true;
            this.btn_signout.Click += new System.EventHandler(this.btn_signout_Click);
            // 
            // listview_documents
            // 
            this.listview_documents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clm_document,
            this.clm_updated_date,
            this.clm_author});
            this.listview_documents.Location = new System.Drawing.Point(12, 60);
            this.listview_documents.Name = "listview_documents";
            this.listview_documents.Size = new System.Drawing.Size(782, 173);
            this.listview_documents.TabIndex = 12;
            this.listview_documents.UseCompatibleStateImageBehavior = false;
            this.listview_documents.View = System.Windows.Forms.View.Details;
            this.listview_documents.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listview_documents_MouseDoubleClick);
            // 
            // clm_document
            // 
            this.clm_document.Text = "Document";
            this.clm_document.Width = 300;
            // 
            // clm_updated_date
            // 
            this.clm_updated_date.Text = "Last Modified Date";
            this.clm_updated_date.Width = 150;
            // 
            // clm_author
            // 
            this.clm_author.Text = "Authors";
            this.clm_author.Width = 300;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 236);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(326, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Only the documents updated in the past one hour will be listed here.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(806, 271);
            this.Controls.Add(this.panel_signin);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.listview_documents);
            this.Controls.Add(this.btn_signout);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Google Docs Notifier";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panel_signin.ResumeLayout(false);
            this.panel_signin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timer_update;
        private System.Windows.Forms.NotifyIcon notify_icon;
        private System.Windows.Forms.Panel panel_signin;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_signin;
        private System.Windows.Forms.TextBox textbox_username;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textbox_password;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_error;
        private System.Windows.Forms.Button btn_signout;
        private System.Windows.Forms.ListView listview_documents;
        private System.Windows.Forms.ColumnHeader clm_document;
        private System.Windows.Forms.ColumnHeader clm_updated_date;
        private System.Windows.Forms.ColumnHeader clm_author;
        private System.Windows.Forms.Label label5;
    }
}

