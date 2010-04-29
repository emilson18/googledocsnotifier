using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Google.GData.Documents;
using Google.GData.Client;
using Google.GData.Extensions;

namespace GoogleDocsNotifier
{
    public partial class Form1 : Form
    {
        DocumentsService myService = new DocumentsService("GoogleDocs-Notifier");

        public Form1()
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
            listbox_updated_documents.Items.Clear();

            DateTime timeNow = DateTime.Now;

            label2.Text = "";

            try
            {
                DocumentsListQuery query = new DocumentsListQuery();
                DocumentsFeed feed = myService.Query(query);

                int number_of_newly_updated_docs = 0;

                foreach (DocumentEntry entry in feed.Entries)
                {
                    //((timeNow.Hour - entry.Updated.Hour) <= 1 && timeNow.Date.Equals(entry.Updated.Date))
                    int timestamp_difference = (int)((TimeSpan)(timeNow - entry.Updated.ToLocalTime())).TotalSeconds;
                    if (timestamp_difference < 3600)
                    {
                        //MessageBox.Show(timestamp_difference.ToString());
                        listbox_updated_documents.Items.Add(entry.Title.Text);
                        if (timestamp_difference < 300)
                        {
                            number_of_newly_updated_docs++;
                            //newly_updated_docs_name += entry.Title.Text + "\r\n";
                        }
                        //updated_docs_name += entry.Title.Text + "\r\n";
                    }
                }

                if (listbox_updated_documents.Items.Count <= 0)
                {
                    label2.Text = "No newly updated documents found";
                }
                notifyIcon1.BalloonTipTitle = "Your Google Docs is updated";
                if (number_of_newly_updated_docs > 0)
                {
                    notifyIcon1.BalloonTipText = "There are " + number_of_newly_updated_docs + " documents updated.";
                    notifyIcon1.ShowBalloonTip(500);
                }
            }catch(Exception e)
            {
                label2.Text = "Oops... Cannot access the Google Docs";
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.WindowState = FormWindowState.Normal;
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
            textbox_password.Enabled = false;
            panel1.Visible = false;
            panel2.Visible = true;
            myService.setUserCredentials(textbox_username.Text, textbox_password.Text);
            updateChecker();
            timer1.Enabled = true;
        }

        private void textbox_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                login();
            }
        }
    }
}
