using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Google.GData.Documents;
using Google.GData.Client;
using Google.GData.Extensions;
using System.Xml;

namespace GoogleDocsNotifier
{
    public partial class MainWindow : Form
    {
        DocumentsService myService = new DocumentsService("GoogleDocs-Notifier");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateChecker();
        }

        private void updateChecker() 
        {
            listview_documents.Items.Clear();

            DateTime timeNow = DateTime.Now;

            label2.Text = "";

            try
            {
                DocumentsListQuery query = new DocumentsListQuery();
                DocumentsFeed feed = myService.Query(query);

                int number_of_newly_updated_docs = 0;

                foreach (DocumentEntry entry in feed.Entries)
                {
                    int timestamp_difference = (int)((TimeSpan)(timeNow - entry.Updated.ToLocalTime())).TotalSeconds;
                    
                    //List all documents which was updated in the past one hour (3600 seconds).
                    if (timestamp_difference < 3600)
                    {
                        //List all the authors of the document.
                        String authors = "";
                        int counter = 0;
                        foreach (AtomPerson author in entry.Authors){

                            //Display the name and email of the author.
                            authors += author.Name + " (" + author.Email + ")";

                            //Add a divider if there are still more authors
                            if(counter < entry.Authors.Count - 1)
                            {
                                authors += "; ";
                            }

                            counter++;
                        }

                        //Add a new item to the listView.
                        ListViewItem item = new ListViewItem(entry.Title.Text);
                        item.Name = entry.AlternateUri.ToString();
                        item.SubItems.Add(entry.Updated.ToString());
                        item.SubItems.Add(authors);
                        item.Tag = entry.AlternateUri.ToString();
                        listview_documents.Items.Add(item);

                        //Recently updated documents (those which are updated in the past 30 seconds).
                        if (timestamp_difference < 30)
                        {
                            number_of_newly_updated_docs++;
                        }
                    }
                }

                if (listview_documents.Items.Count <= 0)
                {
                    label2.Text = "No newly updated documents found";
                }

                //Display the notify icon.
                notify_icon.BalloonTipTitle = "Your Google Docs is updated"; //Title of the balloon tooltip.
                if (number_of_newly_updated_docs > 0)
                {
                    if (number_of_newly_updated_docs == 1)
                    {
                        notify_icon.BalloonTipText = "There is one document newly updated.";
                    }
                    else 
                    {
                        notify_icon.BalloonTipText = "There are " + number_of_newly_updated_docs + " documents just updated.";
                    }
                    notify_icon.ShowBalloonTip(500);
                }
            }
            catch(Google.GData.Client.InvalidCredentialsException)
            {
                throw;
            }
            catch (Google.GData.Client.CaptchaRequiredException)
            {
                throw;
            }
            catch (Google.GData.Client.AuthenticationException)
            {
                throw;
            }
            catch(Exception e)
            {
                label2.Text = "Oops... Cannot access the Google Docs";
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == System.Windows.Forms.FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            login();
        }

        private void login()
        {
            try
            {
                myService.setUserCredentials(textbox_username.Text, textbox_password.Text);
                updateChecker();
                panel_signin.Visible = false;
                timer_update.Enabled = true;
            }
            catch(Exception e)
            {
                //Clear the input fields.
                textbox_username.Text = "";
                textbox_password.Text = "";

                //Show the error message.
                lbl_error.Text = e.Message;
            }
        }

        private void textbox_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                login();
            }
        }

        private void btn_signout_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to sign out?",
                "Log out from Google Docs Notifier",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                logout();
            }
        }

        private void logout()
        {
            //Display the sign-in panel.
            panel_signin.Visible = true;
            timer_update.Enabled = false;

            //Reset the input fields and error message.
            textbox_username.Text = "";
            textbox_password.Text = "";
            lbl_error.Text = "";
        }

        private void listview_documents_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < listview_documents.SelectedItems.Count; i++ )
            {
                //Show the selected documents in the browser.
                Process.Start(listview_documents.Items[listview_documents.SelectedIndices[i]].Tag.ToString());
            }
        }
    }
}
