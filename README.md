# DirectoryTraceMonitor
An application focused on watching a group of directories and writing a log file documenting all changes happening in any of those directories.

This is intended as a debugging tool, where you are trying to figure out what events are happening (and in what order the events occurred).

The way you use the tool is this:
1. Add directories, one per line, to the text box.  You can use the directory finder at the top, and hit "Add Watch Path" or you can edit the textbox directly, one directory per line.  It only supports 20 directories being watched at a time.
2. Define a log file.  The log file should be on the fastest drive you have available so that writing doesn't slow down the capture of events.
3. Hit the start watching button.
4. When you have captured the information you need hit the Stop Watching button.
5. Now go through the log file that it generated looking for clues (you can tail it as the app is running to see current activity).

This application uses FileSystemWatcher which works, but has the limitation that it can be overwhelmed with the number of events per directory.  Because of that the application does as little work as possible for each message that is coming in.  The code does have recovery for when the FileSystemWatcher fails (for whatever reason) and it will log any errors that it gets (which could also be interesting for debugging).

If any of the watched directories are deleted the application will attempt to re-apply that watcher, but it will only wait for a couple of minutes (checking about every 10 seconds).  It logs these attempts to reconnect a watcher to the directory.

Future:
I plan to add watching the files in the directories for file locks to log those.

Suggestions:
If you have suggestions for features or a better way to do something please do create an issue on the project.

This code is open source and free for any use whatsoever.
