using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Documents;

namespace GoogleDocsNotifier.Events
{
    public class ListViewUpdatedEventArgs : EventArgs
    {
        private DocumentEntry _docEntry;

        public ListViewUpdatedEventArgs(DocumentEntry entry)
            : base()
        {
            _docEntry = entry;
        }

        public DocumentEntry DocumentEntry
        {
            get { return _docEntry; }
        }
    }
}
