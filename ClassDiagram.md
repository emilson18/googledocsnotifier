# MainWindow #
It is responsible for the user interface. Login and logout are done in this class.

# DocumentsFeedParser #
It will parse the Google Docs feed in order to retrieve relevant information and pass them to DocItem class.

# DocItem #
It stores information about a Google document entry. The content of listView control in MainWindow is binded to a collection of DocItem entries.

![https://googledocsnotifier.googlecode.com/svn/wiki/Google%20Docs%20Notifier%202%20Class%20Diagram.png](https://googledocsnotifier.googlecode.com/svn/wiki/Google%20Docs%20Notifier%202%20Class%20Diagram.png)