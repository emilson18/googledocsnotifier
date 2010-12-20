using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Google.GData.Documents;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Security.Cryptography;

namespace GoogleDocsNotifier
{
    public partial class MainWindow : Window
    {
        #region Local Variables
        DocumentsService myService = new DocumentsService("GoogleDocs-Notifier");
        private ObservableCollection<DocItem> _docItems = new ObservableCollection<DocItem>();

        private BackgroundWorker parseWorker;

        //A timer to check the latest updates of Google Docs site.
        private DispatcherTimer timerChecker;

        //The notification icon at system tray.
        System.Windows.Forms.NotifyIcon trayNotifyIcon;

        //Context menu strip for the notification icon at system tray.
        private System.Windows.Forms.ContextMenuStrip ctx_trayMenu;
        private System.Windows.Forms.ToolStripMenuItem mnu_runProgram;
        private System.Windows.Forms.ToolStripMenuItem mnu_goSite;
        private System.Windows.Forms.ToolStripMenuItem mnu_about;
        private System.Windows.Forms.ToolStripSeparator separator;
        private System.Windows.Forms.ToolStripMenuItem mnu_signOut;
        private System.Windows.Forms.ToolStripMenuItem mnu_exit;

        private DocumentsFeedParser parser;
        #endregion

        public MainWindow()
        {
            this.InitializeComponent();

            listViewDataBinding();
            parseWorkerSetup();
            timerCheckerSetup();
            trayNotifyIconSetup();
            autoLoginPreparation();
        }

        #region App Setup
        private void autoLoginPreparation()
        {
            //Load the user name and the ENCRYPTED password.
            string userName = Properties.Settings.Default.UserName;

            try
            {
                //Decrypt the password.
                byte[] userNameAsEntropy = UnicodeEncoding.Unicode.GetBytes(userName);
                byte[] toDecryptPassword = Convert.FromBase64String(Properties.Settings.Default.Password);
                byte[] decryptedPassword =
                    ProtectedData.Unprotect(
                    toDecryptPassword,
                    userNameAsEntropy,
                    DataProtectionScope.CurrentUser);
                string password = System.Text.Encoding.Unicode.GetString(decryptedPassword);

                //If there are username and password stored in local machine,
                //then do the auto-login.
                if (!(userName.Equals("") || password.Equals("")))
                {
                    tb_accountName.Text = userName;
                    tb_password.Password = password;
                    cb_rememberMe.IsChecked = true;

                    login();
                }
            }
            catch (CryptographicException) 
            {
                tb_accountName.Text = userName;
                tb_password.Password = "";
                cb_rememberMe.IsChecked = false;
            }
        }

        private void trayNotifyIconSetup()
        {
            //Configure the notification icon at system tray.
            trayNotifyIcon = new System.Windows.Forms.NotifyIcon();
            trayNotifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            trayNotifyIcon.Text = "Google Docs Notifier";
            trayNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(trayNotifyIcon_MouseDoubleClick);
            trayNotifyIcon.Visible = true;

            //Configure the context menu strip for the notification icon.
            //For mnu_runProgram
            mnu_runProgram = new System.Windows.Forms.ToolStripMenuItem();
            mnu_runProgram.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            mnu_runProgram.Text = "Show";
            mnu_runProgram.Click += new EventHandler(mnu_runProgram_Click);
            //For mnu_goSite
            mnu_goSite = new System.Windows.Forms.ToolStripMenuItem("View Google Docs website");
            mnu_goSite.Click += new EventHandler(mnu_goSite_Click);
            //For mnu_about
            mnu_about = new System.Windows.Forms.ToolStripMenuItem("About");
            mnu_about.Click += new EventHandler(mnu_about_Click);
            //For separator
            separator = new System.Windows.Forms.ToolStripSeparator();
            //For mnu_signOut
            mnu_signOut = new System.Windows.Forms.ToolStripMenuItem("Sign Out");
            mnu_signOut.Click += new EventHandler(mnu_signOut_Click);
            //For mnu_exit
            mnu_exit = new System.Windows.Forms.ToolStripMenuItem("Exit");
            mnu_exit.Click += new EventHandler(mnu_exit_Click);
            //For ctx_trayMenu
            ctx_trayMenu = new System.Windows.Forms.ContextMenuStrip();
            ctx_trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] 
            {
                mnu_runProgram, mnu_goSite, mnu_about,
                separator,
                mnu_signOut, mnu_exit
            });
            trayNotifyIcon.ContextMenuStrip = ctx_trayMenu;
        }

        private void timerCheckerSetup()
        {
            //Configure the timer to check the latest updates of Google Docs site.
            timerChecker = new DispatcherTimer();
            timerChecker.Tick += new EventHandler(timerChecker_Tick);
            //Check the status every 45 seconds.
            timerChecker.Interval = TimeSpan.FromSeconds(45);
        }

        private void parseWorkerSetup()
        {
            //Assign event handlers to the parseWorker.
            parseWorker = new BackgroundWorker();
            parseWorker.WorkerSupportsCancellation = true;
            parseWorker.DoWork += new DoWorkEventHandler(parseWorker_DoWork);
            parseWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(parseWorker_RunWorkerCompleted);
        }

        private void listViewDataBinding()
        {
            //Data binding for the listView.
            lv_documents.ItemsSource = _docItems;
        }
        #endregion

        #region ParseWorker and Worker
        private void parseWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Refresh the listView.
            this.Dispatcher.Invoke((Action)delegate
            {
                _docItems.Clear();
            });

            try
            {
                //Retrieve information from the feed.
                parser.parse();

                //Save the settings here (only if no error from parser.parse()).
                if ((bool)e.Argument)
                    Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    //Clear the input fields.
                    tb_accountName.Text = "";
                    tb_password.Password = "";

                    //Show the login page.
                    gb_loginPage.Visibility = Visibility.Visible;

                    //Stop the timer.
                    timerChecker.Stop();

                    //Show the error message.
                    lb_messageLabel.Content = "Unable to access Google server. Please try again.";
                });
            }
        }

        void parseWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                gb_progressbar.Visibility = Visibility.Hidden;
            });
        }

        void parser_ListViewUpdated(object sender, Events.ListViewUpdatedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                _docItems.Add(new DocItem(e.DocumentEntry));
            });
        }

        void parser_StatusChanged(object sender, Events.StatusChangedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                lb_messageLabel.Content = e.Message;
            });
        }

        void parser_ProgressChanged(object sender, GoogleDocsNotifier.Events.ProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                gb_progressbar.Visibility = Visibility.Visible;
                pb_downloadingProgress.Value = e.ProgressValue;
                lb_messageLabel.Content = "Connecting to Google Docs server...";
            });
        }

        void parser_NotifyIconUpdated(object sender, GoogleDocsNotifier.Events.NotifyIconUpdatedEventArgs e)
        {
            if (e.NumOfUnviewedDocuments > 0)
            {
                System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                //The original icon for the Google Docs Notifier.
                System.Drawing.Image originalAppIcon =
                System.Drawing.Icon.ExtractAssociatedIcon(
                System.Windows.Forms.Application.ExecutablePath).ToBitmap();
                //Create a bitmap and draw the number of unviewed documents on it.
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(16, 16);
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
                graphics.DrawImage(originalAppIcon, 0, 0, 16, 16);
                graphics.DrawString(e.NumOfUnviewedDocuments.ToString(), new System.Drawing.Font("Helvetica", 6), brush, 3, 3);
                graphics.Save();
                //Convert the bitmap with text to an Icon.
                IntPtr hIcon = bitmap.GetHicon();
                System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(hIcon);
                trayNotifyIcon.Icon = icon;
                //Dispose the graphics.
                graphics.Dispose();
                trayNotifyIcon.Text = e.NumOfUnviewedDocuments.ToString() + " unread documents";
            }
            else
            {
                trayNotifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
                trayNotifyIcon.Text = "No unread document";
            }

            //Show the balloon tooltip.
            if (e.IsDisplayTooltip)
            {
                //Title of the balloon tooltip.
                trayNotifyIcon.BalloonTipTitle = e.Title;
                trayNotifyIcon.BalloonTipText = e.Message;
                trayNotifyIcon.ShowBalloonTip(500);
            }
        }
        #endregion

        #region Timer
        void timerChecker_Tick(object sender, EventArgs e)
        {
            //If the background worker is still running, then don't run it.
            if (!parseWorker.IsBusy)
                //Run the parser.
                //False means there are no settings need to be saved.
                parseWorker.RunWorkerAsync(false);
        }
        #endregion

        #region Tray Icon and Its Menu
        void trayNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        void mnu_runProgram_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        void mnu_goSite_Click(object sender, EventArgs e)
        {
            Process.Start("http://docs.google.com");
        }

        void mnu_about_Click(object sender, EventArgs e)
        {
            Process.Start("http://googledocsnotifier.googlecode.com");
        }

        void mnu_signOut_Click(object sender, EventArgs e)
        {
            //Can only logout after you login
            if(gb_loginPage.Visibility == Visibility.Hidden)
                logout();
        }

        void mnu_exit_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
        #endregion

        #region GUI Events
        private void tb_password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                login();
        }

        private void btn_signIn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            login();
        }

        private void lb_signOut_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            logout();
        }
		
		private void ListViewItem_MouseDoubleClick(object sender, System.Windows.RoutedEventArgs e)
		{
			ListViewItem item = (ListViewItem)sender;
            DocItem entry = (DocItem)item.Content;
            Process.Start(entry.alternateURL);
        }

        private void win_main_StateChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState.Equals(System.Windows.WindowState.Minimized))
            {
                this.Hide();
            }
        }
        #endregion

        #region Login and Logout Methods
        private void login()
        {
            //Set the user credentials.
            myService.setUserCredentials(tb_accountName.Text, tb_password.Password);

            //Remember the username and password (if allowed by the user).
            //They will be saved later, not now because they are not yet varified.
            //Store the user name first without encryption.
            Properties.Settings.Default.UserName = tb_accountName.Text;
            try
            {
                //Encrypt the password before storing it!
                byte[] toEncryptPassword = System.Text.Encoding.Unicode.GetBytes(tb_password.Password);
                byte[] userNameAsEntropy = System.Text.Encoding.Unicode.GetBytes(tb_accountName.Text);
                byte[] encryptedPassword = ProtectedData.Protect(
                    toEncryptPassword,
                    userNameAsEntropy,
                    DataProtectionScope.CurrentUser);
                //Store the encrypted password.
                Properties.Settings.Default.Password =
                    Convert.ToBase64String(encryptedPassword);
            }
            catch (CryptographicException) 
            {
                Properties.Settings.Default.Password = "";
                cb_rememberMe.IsChecked = false;
            }
            
            //Setup the parser.
            parser = new DocumentsFeedParser(myService);
            parser.ListViewUpdated += new ListViewUpdatedHandler(parser_ListViewUpdated);
            parser.StatusChanged += new StatusChangedHandler(parser_StatusChanged);
            parser.ProgressChanged += new ProgressChangedHandler(parser_ProgressChanged);
            parser.NotifyIconUpdated += new NotifyIconUpdatedHandler(parser_NotifyIconUpdated);

            if (cb_rememberMe.IsChecked == false)
            {
                Properties.Settings.Default.UserName = "";
                Properties.Settings.Default.Password = "";
            }

            //Run the parser.
            //True means there are settings need to be saved, i.e. the
            //user name and password.
            parseWorker.RunWorkerAsync(true);

            gb_loginPage.Visibility = Visibility.Hidden;

            //Start the timer.
            timerChecker.Start();
        }

        private void logout()
        {
            //Stop the timer.
            timerChecker.Stop();

            //Stop the parser.
            parseWorker.CancelAsync();

            //Remove event handlers because they will be added to the parser again
            //during the process of login.
            parser.ListViewUpdated -= new ListViewUpdatedHandler(parser_ListViewUpdated);
            parser.StatusChanged -= new StatusChangedHandler(parser_StatusChanged);
            parser.ProgressChanged -= new ProgressChangedHandler(parser_ProgressChanged);
            parser.NotifyIconUpdated -= new NotifyIconUpdatedHandler(parser_NotifyIconUpdated);

            //Clear the input field.
            //Only clear the password, leave the user name there so that
            //the user does not need to re-enter the user name as well.
            tb_password.Password = "";
            //Uncheck the check box (if any).
            cb_rememberMe.IsChecked = false;
            //Show the login page.
            gb_loginPage.Visibility = Visibility.Visible;

            //Reset the message area.
            lb_messageLabel.Content = "You have been logged out.";
            //Reset the notification icon.
            trayNotifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            trayNotifyIcon.Text = "Google Docs Notifier";
        }
        #endregion
    }
}