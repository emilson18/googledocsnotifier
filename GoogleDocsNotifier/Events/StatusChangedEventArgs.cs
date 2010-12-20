using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleDocsNotifier.Events
{
    public class StatusChangedEventArgs : EventArgs
    {
        private string _message;

        public StatusChangedEventArgs(string msg)
            : base()
        {
            _message = msg;
        }

        public string Message
        {
            get { return _message; }
        }
    }
}
