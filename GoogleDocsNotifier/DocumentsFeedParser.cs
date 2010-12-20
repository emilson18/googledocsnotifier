using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Documents;
using Google.GData.Extensions;
using GoogleDocsNotifier.Events;
using Google.GData.Extensions.AppControl;

namespace GoogleDocsNotifier
{
    #region Delegates
    //Delegates for event handler
    public delegate void ListViewUpdatedHandler(object sender, ListViewUpdatedEventArgs e);
    public delegate void StatusChangedHandler(object sender, StatusChangedEventArgs e);
    public delegate void ProgressChangedHandler(object sender, ProgressChangedEventArgs e);
    public delegate void NotifyIconUpdatedHandler(object sender, NotifyIconUpdatedEventArgs e);
    #endregion

    class DocumentsFeedParser
    {
        #region Event Handlers
        //Event: Updating the listView.
        public event ListViewUpdatedHandler ListViewUpdated;
        //Event: Changing the status message.
        public event StatusChangedHandler StatusChanged;
        //Event: Changing the progress value.
        public event ProgressChangedHandler ProgressChanged;
        //Event: Updating the balloon tooltip.
        public event NotifyIconUpdatedHandler NotifyIconUpdated;
        #endregion

        #region Local Variables
        private DocumentsService _myService;

        //The editedTime of the most recent updated document in Google Docs.
        private DateTime latestEditedTime = new DateTime();
        #endregion

        public DocumentsFeedParser(DocumentsService myService) 
        {
            _myService = myService;
        }

        public void parse()
        {
            //The date and time in this moment.
            DateTime timeNow = DateTime.Now;

            //Indicate if there are new updates in Google Docs.
            bool isNewUpdate = false;
            //The title of the most recent updated document.
            string latestEditedTitle = "";
            //The number of documents in total.
            int numOfDocuments = 0;
            //The number of unviewed documents.
            int unviewedDocuments = 0;

            //Inform the GUI to change the progress value.
            UpdateProgressBar(25);

            try
            {
                //Download the feed from Google Docs server.
                DocumentsListQuery query = new DocumentsListQuery();
                DocumentsFeed feed = _myService.Query(query);

                //Inform the GUI to change the progress value.
                UpdateProgressBar(50);

                foreach (DocumentEntry entry in feed.Entries)
                {
                    DateTime lastEditedTime = new DateTime(); //No use for now.
                    DateTime lastViewedTime = timeNow;
                    //Note: There are some Google Docs type has no lastViewed tag.
                    //      Hence, it is better to set the default value of the
                    //      lastViewedTime to be the current date and time.

                    //Retrieve lastEditedTime and lastViewedTime from
                    //the Extension Elements of the feeds.
                    HandleExtensionElements(entry, ref lastEditedTime, ref lastViewedTime);

                    //If the document is newly created document or unread updated document...
                    if (VerifyNewDocuments(entry) || VerifyUnviewedUpdatedDocuments(entry, lastViewedTime))
                    {
                        HandleNewOrUnreadUpdatedDocuments(
                            entry,
                            ref isNewUpdate, ref latestEditedTime,
                            ref latestEditedTitle, ref unviewedDocuments);
                    }

                    //Inform the GUI to change the progress value.
                    UpdateProgressBar(((int)(((double)numOfDocuments++ / feed.Entries.Count()) * 50)) + 50);
                }

                //Inform the GUI to change the status message.
                UpdateStatusMessage(unviewedDocuments.ToString() + " documents listed above.");

                //Inform the GUI to update and show the balloon tooltip.
                updateNotifyIcon(
                            "Your Google Docs is updated",
                            "Latest edited document: " + latestEditedTitle,
                            unviewedDocuments,
                            isNewUpdate);
            }
            catch (Google.GData.Client.InvalidCredentialsException)
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
            catch (Exception)
            {
                //Inform the GUI to change the status message.
                UpdateStatusMessage("Unable to connect to Google Docs server.");

                //Inform the GUI to UPDATE ONLY the balloon tooltip icon
                //and say there is no new update found.
                updateNotifyIcon(
                            "",
                            "",
                            0,
                            false);
            }
        }

        #region Find New Documents, Unviewed Documents and Latest Changed Documents
        /// <summary>
        /// Check if the current document is a new document or not.
        /// </summary>
        /// <param name="entry">The current document entry.</param>
        /// <returns>True if the current document is newly created.</returns>
        private static bool VerifyNewDocuments(DocumentEntry entry)
        {
            bool isDocumentNew = true;
            for (int i = 0; i < entry.Categories.Count(); i++)
            {
                //New documents will not be labelled as "viewed" in Google feeds.
                if (entry.Categories[i].Label.Equals("viewed"))
                {
                    isDocumentNew = false;
                    break;
                }
            }

            return isDocumentNew;
        }

        /// <summary>
        /// Responsible for verifying if the document is updated and unviewed.
        /// </summary>
        /// <param name="entry">The document entry.</param>
        /// <param name="lastViewedTime">The last viewed time of the document entry.</param>
        /// <returns>True if the document is updated and unviewed.</returns>
        private bool VerifyUnviewedUpdatedDocuments(DocumentEntry entry, DateTime lastViewedTime)
        {
            return timeStampDifference(lastViewedTime.ToLocalTime(), entry.Updated.ToLocalTime()) < 0;
        }

        /// <summary>
        /// Verify if the document is the newest document among the updated documents.
        /// </summary>
        /// <param name="entry">The current document entry.</param>
        /// <param name="latestEditedTime">The edited time of the newest document.</param>
        /// <param name="latestEditedTitle">The name of the newest document.</param>
        /// <returns>Returns true if the document is the newest one among all the known documents so far.</returns>
        private bool CheckIfDocumentIsNewest(DocumentEntry entry, ref DateTime latestEditedTime, ref string latestEditedTitle)
        {
            if (timeStampDifference(latestEditedTime.ToLocalTime(), entry.Updated.ToLocalTime()) < 0)
            {
                latestEditedTime = entry.Updated;
                latestEditedTitle = entry.Title.Text;
                return true;
            }
            return false;
        }
        #endregion

        #region Inform GUI to Make UI Changes
        /// <summary>
        /// Inform the GUI to add one new entry to the ListView control.
        /// </summary>
        /// <param name="entry">The document entry that will be added to the ListView.</param>
        private void AddNewEntryToListView(DocumentEntry entry)
        {
            if (ListViewUpdated != null)
                ListViewUpdated(this, new ListViewUpdatedEventArgs(entry));
        }

        /// <summary>
        /// Inform the GUI to update the ProgressBar value.
        /// </summary>
        /// <param name="progressValue">The new ProgressBar value (Max: 100).</param>
        private void UpdateProgressBar(int progressValue)
        {
            if (ProgressChanged != null)
                ProgressChanged(this,
                    new ProgressChangedEventArgs(
                        progressValue, 100));
        }

        /// <summary>
        /// Inform the GUI to update the status message.
        /// </summary>
        /// <param name="message">The new status message.</param>
        private void UpdateStatusMessage(string message)
        {
            if (StatusChanged != null)
                StatusChanged(this,
                    new StatusChangedEventArgs(message));
        }

        /// <summary>
        /// Inform the GUI to update the notifyIcon and its balloon tooltip.
        /// </summary>
        /// <param name="title">Title for the balloon tooltip.</param>
        /// <param name="message">Message to be shown in the balloon tooltip.</param>
        /// <param name="numOfUnviewedDocuments">Total number of unviewed documents.</param>
        /// <param name="isDisplayTooltip">True if balloon tooltip needs to be shown.</param>
        private void updateNotifyIcon(string title, string message, int numOfUnviewedDocuments, bool isDisplayTooltip)
        {
            if (NotifyIconUpdated != null)
                NotifyIconUpdated(this,
                    new NotifyIconUpdatedEventArgs(
                        title,
                        message,
                        numOfUnviewedDocuments,
                        isDisplayTooltip));
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// This part handles the ExtensionElement of the document entry.
        /// It will retrieve the lastEdited time and lastViewed time of the document.
        /// </summary>
        /// <param name="entry">The current document entry.</param>
        /// <param name="lastEditedTime">The latest edited time for this current document entry.</param>
        /// <param name="lastViewedTime">The latest viewed time for this current document entry.</param>
        private void HandleExtensionElements(
            DocumentEntry entry, 
            ref DateTime lastEditedTime, ref DateTime lastViewedTime)
        {
            foreach (object obj in entry.ExtensionElements)
            {
                //Retrieve the lastEdited time of the document.
                if (obj.GetType().Name.Equals("AppEdited")
                    && ((AppEdited)obj).XmlName.Equals("edited"))
                {
                    lastEditedTime = ((AppEdited)obj).DateValue;
                }

                //Retrieve the lastViewed time of the document.
                if (obj.GetType().Name.Equals("XmlExtension")
                    && ((XmlExtension)obj).XmlName.Equals("lastViewed"))
                {
                    lastViewedTime =
                        convertStringToDateTime(((XmlExtension)obj).Node.InnerText);

                    //Stop iterating through the ExtensionElements because
                    //lastViewed tag is the last tag in the XML feed that
                    //we are interested in.
                    break;
                }
            }
        }

        /// <summary>
        /// Perform the follwoing actions to those new or unread updated documents:
        /// 1. Check if the document is the newest among all;
        /// 2. Update GUI in order to add the document to the listView;
        /// 3. Increase the count of number of unviewed documents.
        /// </summary>
        /// <param name="entry">The document entry.</param>
        /// <param name="isNewUpdate">Indicator specifying if the documents is the newest among all.</param>
        /// <param name="latestEditedTime">The latest edited time of the newest document.</param>
        /// <param name="latestEditedTitle">The title of the newest document.</param>
        /// <param name="unviewedDocuments">The count of number of unviewed documents.</param>
        private void HandleNewOrUnreadUpdatedDocuments(
            DocumentEntry entry,
            ref bool isNewUpdate, ref DateTime latestEditedTime,
            ref string latestEditedTitle, ref int unviewedDocuments)
        {
            //Check if the document is the newest among all the documents.
            if (CheckIfDocumentIsNewest(entry, ref latestEditedTime, ref latestEditedTitle))
                isNewUpdate = true;

            //Inform the GUI to add the new document entry to the listView.
            AddNewEntryToListView(entry);

            //Count the number of unviewed documents.
            unviewedDocuments++;
        }

        /// <summary>
        /// Converts a DateTime string in Google DateTime format to DateTime object.
        /// </summary>
        /// <param name="googleDateTimeString">
        /// DateTime string in Google DateTime format.
        /// </param>
        /// <returns>DateTime object of the given DateTime string.</returns>
        private DateTime convertStringToDateTime(string googleDateTimeString) 
        {
            string[] googleDateTimeStringArray = googleDateTimeString.Split('T');
            string[] googleDateStringArray = googleDateTimeStringArray[0].Split('-');
            string[] googleTimeStringArray = googleDateTimeStringArray[1].Split(':');
            
            return new DateTime(
                Int32.Parse(googleDateStringArray[0]),
                Int32.Parse(googleDateStringArray[1]),
                Int32.Parse(googleDateStringArray[2]),
                Int32.Parse(googleTimeStringArray[0]),
                Int32.Parse(googleTimeStringArray[1]),
                Int32.Parse(googleTimeStringArray[2].Substring(0, 2)),
                DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the time difference (in seconds) of two given DateTime inputs.
        /// </summary>
        /// <param name="timeStamp1">The first DateTime input.</param>
        /// <param name="timeStamp2">The second DateTime input.</param>
        /// <returns>The time difference in seconds.</returns>
        private int timeStampDifference(DateTime timeStamp1, DateTime timeStamp2) 
        {
            return (int)
                ((TimeSpan)(timeStamp1 - timeStamp2))
                .TotalSeconds;
        }
        #endregion
    }
}
