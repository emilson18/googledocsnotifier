using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleDocsNotifier.Events
{
    public class NotifyIconUpdatedEventArgs : EventArgs
    {
        private string _title;
        private string _message;
        private int _numOfUnviewedDocuments;
        private bool _isDisplayTooltip;

        public NotifyIconUpdatedEventArgs(string title, string message, int numOfUnviewedDocuments, bool isDisplayTooltip)
            : base() 
        {
            _title = title;
            _message = message;
            _numOfUnviewedDocuments = numOfUnviewedDocuments;
            _isDisplayTooltip = isDisplayTooltip;
        }

        public string Title 
        {
            get { return _title; }
        }

        public string Message
        {
            get { return _message; }
        }

        public int NumOfUnviewedDocuments
        {
            get { return _numOfUnviewedDocuments; }
        }

        public bool IsDisplayTooltip 
        {
            get { return _isDisplayTooltip; }
        }
    }
}
