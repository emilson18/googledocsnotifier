using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleDocsNotifier.Events
{
    public class ProgressChangedEventArgs : EventArgs
    {
        private int _percentage;

        public ProgressChangedEventArgs(int current, int maximum)
            : base()
        {
            _percentage = (int)(((double)current / maximum) * 100);
        }

        public int ProgressValue
        {
            get { return _percentage; }
        }
    }
}
