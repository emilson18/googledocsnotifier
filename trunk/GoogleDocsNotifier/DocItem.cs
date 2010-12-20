using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using Google.GData.Documents;
using Google.GData.Client;
using Google.GData.Extensions;

namespace GoogleDocsNotifier
{
	public class DocItem : INotifyPropertyChanged
	{
		private string _docType;
        private string _docName;
        private string _modifiedDate;
        private string _updater;

        public string alternateURL;

        public event PropertyChangedEventHandler PropertyChanged;
		
		public DocItem(DocumentEntry docEntry)
		{
			//Retrieve the type of the document.
			if(docEntry.IsDocument)
				_docType = "[Document] ";
			else if(docEntry.IsPDF)
				_docType = "[PDF] ";
			else if(docEntry.IsSpreadsheet)
				_docType = "[Spreadsheet] ";
			else if(docEntry.IsPresentation)
				_docType = "[Presentation] ";
			else
				_docType = "[Other] ";
			
            //Retrieve the title of the document.
            _docName = docEntry.Title.Text;

            //Retrieve the modified date and time of the document.
            DateTime timeNow = DateTime.Now;
			int timestamp_difference = (int)((TimeSpan)(timeNow - docEntry.Updated.ToLocalTime())).TotalSeconds;
			if(timestamp_difference < 60)
				_modifiedDate = timestamp_difference.ToString() + " seconds ago";
			else if(timestamp_difference < 120)
				_modifiedDate = "1 minute ago";
            else if (timestamp_difference < 3600)
                _modifiedDate = (timestamp_difference / 60).ToString() + " minutes ago";
            else if (timestamp_difference < 7200)
                _modifiedDate = "1 hour ago";
            else if (timestamp_difference < 86400)
                _modifiedDate = (timestamp_difference / 3600).ToString() + " hours ago";
            else
			    _modifiedDate = docEntry.Updated.ToLocalTime().ToString();

            _modifiedDate += " ";

            //Retrieve the name of the person who updated the document.
            _updater = "Updated by: ";
            
            foreach (object obj in docEntry.ExtensionElements)
            {
                //Retrieve the name of the person who updated the document.
                if (obj.GetType().Name.Equals("XmlExtension")
                    && ((XmlExtension)obj).XmlName.Equals("lastModifiedBy"))
                {
                    _updater += ((XmlExtension)obj).Node.FirstChild.InnerText;

                    //Stop iterating through the ExtensionElements because
                    //lastModifiedBy tag is the last tag in the XML feed that
                    //we are interested in.
                    break;
                }
            }

            //Retrieve the URL to the document on the Internet.
            alternateURL = docEntry.AlternateUri.Content.ToString();
		}

        public string DocType
        {
            get { return _docType; }
            set
            {
                _docType = value;
                OnPropertyChanged("DocType");
            }
        }

        public string DocName
        {
            get { return _docName; }
            set
            {
                _docName = value;
                OnPropertyChanged("DocName");
            }
        }

        public string ModifiedDate
        {
            get { return _modifiedDate; }
            set
            {
                _modifiedDate = value;
                OnPropertyChanged("ModifiedDate");
            }
        }

        public string Updater
        {
            get { return _updater; }
            set
            {
                _updater = value;
                OnPropertyChanged("Updater");
            }
        }
		
		protected virtual void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
	}
}