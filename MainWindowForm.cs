using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace DirectoryTraceMonitor
{
    /// <summary>
    /// This is the application's main form and the only UI.
    /// </summary>
    public class MainWindowForm : Form
    {
        private const int WatchPathLimit = 20;

        #region Form controls
        private TextBox _watchFolders;
        private FolderBrowserDialog _watchFolderBrowserDialog;
        private OpenFileDialog _logFileBrowserDialog;
        private Label _watchFolderLabel;
        private Label _logFileLabel;
        private Label _recentLogEntriesLabel;
        private TextBox _watchPaths;
        private TextBox _logfilePath;
        private Button _watchFolderBrowse;
        private Button _logFileBrowse;
        private Button _watchButton;
        private Button _addWatchPath;
        #endregion Form controls

        private readonly Dictionary<string, FileSystemWatcher> _fileSystemWatchers = new Dictionary<string, FileSystemWatcher>();

        private bool   _isWatching = false;
        private string _logFilePath = null;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MainWindowForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAllWatchers();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._watchFolders = new System.Windows.Forms.TextBox();
            this._watchFolderBrowse = new System.Windows.Forms.Button();
            this._watchButton = new System.Windows.Forms.Button();
            this._watchFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._watchFolderLabel = new System.Windows.Forms.Label();
            this._watchPaths = new System.Windows.Forms.TextBox();
            this._logFileLabel = new System.Windows.Forms.Label();
            this._logfilePath = new System.Windows.Forms.TextBox();
            this._logFileBrowse = new System.Windows.Forms.Button();
            this._recentLogEntriesLabel = new System.Windows.Forms.Label();
            this._addWatchPath = new System.Windows.Forms.Button();
            this._logFileBrowserDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // WatchFolderPath
            // 
            this._watchFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._watchFolders.Location = new System.Drawing.Point(127, 12);
            this._watchFolders.Name = "_watchFolders";
            this._watchFolders.Size = new System.Drawing.Size(393, 20);
            this._watchFolders.TabIndex = 0;
            this._watchFolders.Text = "D:\\temp";
            // 
            // WatchFolderBrowse
            // 
            this._watchFolderBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._watchFolderBrowse.Location = new System.Drawing.Point(526, 10);
            this._watchFolderBrowse.Name = "_watchFolderBrowse";
            this._watchFolderBrowse.Size = new System.Drawing.Size(32, 23);
            this._watchFolderBrowse.TabIndex = 1;
            this._watchFolderBrowse.Text = "...";
            this._watchFolderBrowse.Click += new System.EventHandler(this.WatchFolderBrowseClick);
            // 
            // WatchButton
            // 
            this._watchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._watchButton.Location = new System.Drawing.Point(526, 445);
            this._watchButton.Name = "_watchButton";
            this._watchButton.Size = new System.Drawing.Size(140, 23);
            this._watchButton.TabIndex = 2;
            this._watchButton.Text = "Start Watching";
            this._watchButton.Click += new System.EventHandler(this.ToggleWatchingStatus);
            // 
            // WatchFolderLabel
            // 
            this._watchFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._watchFolderLabel.Location = new System.Drawing.Point(12, 9);
            this._watchFolderLabel.Name = "_watchFolderLabel";
            this._watchFolderLabel.Size = new System.Drawing.Size(109, 23);
            this._watchFolderLabel.TabIndex = 4;
            this._watchFolderLabel.Text = "Folder to watch:";
            this._watchFolderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _watchPaths
            // 
            this._watchPaths.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._watchPaths.Location = new System.Drawing.Point(15, 105);
            this._watchPaths.Multiline = true;
            this._watchPaths.Name = "_watchPaths";
            this._watchPaths.Size = new System.Drawing.Size(651, 334);
            this._watchPaths.TabIndex = 5;
            // 
            // LogFileLabel
            // 
            this._logFileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._logFileLabel.Location = new System.Drawing.Point(12, 43);
            this._logFileLabel.Name = "_logFileLabel";
            this._logFileLabel.Size = new System.Drawing.Size(109, 23);
            this._logFileLabel.TabIndex = 6;
            this._logFileLabel.Text = "Log to file:";
            this._logFileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogfilePath
            // 
            this._logfilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._logfilePath.Location = new System.Drawing.Point(127, 46);
            this._logfilePath.Name = "_logfilePath";
            this._logfilePath.Size = new System.Drawing.Size(501, 20);
            this._logfilePath.TabIndex = 7;
            this._logfilePath.Text = "C:\\temp\\test.log";
            // 
            // LogFileBrowse
            // 
            this._logFileBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._logFileBrowse.Location = new System.Drawing.Point(634, 46);
            this._logFileBrowse.Name = "_logFileBrowse";
            this._logFileBrowse.Size = new System.Drawing.Size(32, 23);
            this._logFileBrowse.TabIndex = 8;
            this._logFileBrowse.Text = "...";
            this._logFileBrowse.Click += new System.EventHandler(this.LogFileBrowseClick);
            // 
            // RecentLogEntriesLabel
            // 
            this._recentLogEntriesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._recentLogEntriesLabel.Location = new System.Drawing.Point(15, 79);
            this._recentLogEntriesLabel.Name = "_recentLogEntriesLabel";
            this._recentLogEntriesLabel.Size = new System.Drawing.Size(106, 23);
            this._recentLogEntriesLabel.TabIndex = 9;
            this._recentLogEntriesLabel.Text = "Watched Paths:";
            this._recentLogEntriesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _addWatchPath
            // 
            this._addWatchPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._addWatchPath.Location = new System.Drawing.Point(564, 10);
            this._addWatchPath.Name = "_addWatchPath";
            this._addWatchPath.Size = new System.Drawing.Size(102, 23);
            this._addWatchPath.TabIndex = 10;
            this._addWatchPath.Text = "Add Watch Path";
            this._addWatchPath.UseVisualStyleBackColor = true;
            this._addWatchPath.Click += new System.EventHandler(this._addWatchPath_Click);
            // 
            // LogFileBrowserDialog
            // 
            this._logFileBrowserDialog.CheckFileExists = false;
            this._logFileBrowserDialog.Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            this._logFileBrowserDialog.RestoreDirectory = true;
            // 
            // MainWindowForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(678, 480);
            this.Controls.Add(this._addWatchPath);
            this.Controls.Add(this._recentLogEntriesLabel);
            this.Controls.Add(this._logFileBrowse);
            this.Controls.Add(this._logfilePath);
            this.Controls.Add(this._logFileLabel);
            this.Controls.Add(this._watchPaths);
            this.Controls.Add(this._watchFolderLabel);
            this.Controls.Add(this._watchButton);
            this.Controls.Add(this._watchFolderBrowse);
            this.Controls.Add(this._watchFolders);
            this.Name = "MainWindowForm";
            this.Text = "Directory Trace Monitor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
        {
            System.Windows.Forms.Application.Run(new MainWindowForm());
        }

        /// <summary>
        /// Create one FileSystemWatcher for the passed folder and add it to the
        /// Dictionary.
        /// </summary>
        /// <param name="folder">The folder to watch (it must exist).</param>
        /// <returns>The watcher created.</returns>
        private FileSystemWatcher CreateFileSystemWatcher(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                // Silently ignore empty lines in the list.
                return null;
            }

            if (_fileSystemWatchers.ContainsKey(folder))
            {
                // Dispose and remove the previous file watcher
                var defunctFileSystemWatcher = _fileSystemWatchers[folder];

                if (null != defunctFileSystemWatcher)
                {
                    _fileSystemWatchers.Remove(folder);
                    defunctFileSystemWatcher.EnableRaisingEvents = false;
                    defunctFileSystemWatcher.Dispose();
                }
            }

            if (!Directory.Exists(folder))
            {
                MessageBox.Show($"Attempted to start a watcher on directory '{folder}' but the directory did not exist.");
                return null;
            }

            var fileSystemWatcher = new FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(fileSystemWatcher)).BeginInit();
            fileSystemWatcher.Path = folder;
            fileSystemWatcher.Filter = "*.*";
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess 
                                           | NotifyFilters.LastWrite
                                           | NotifyFilters.FileName
                                           | NotifyFilters.Attributes
                                           | NotifyFilters.CreationTime
                                           | NotifyFilters.Security
                                           | NotifyFilters.Size;
            fileSystemWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            fileSystemWatcher.Created += new FileSystemEventHandler(OnFileChanged);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            fileSystemWatcher.Error += new ErrorEventHandler(OnError);
            fileSystemWatcher.EnableRaisingEvents = true;
            ((System.ComponentModel.ISupportInitialize)(fileSystemWatcher)).EndInit();

            // Track the new file watcher
            _fileSystemWatchers[folder] = fileSystemWatcher;
            return fileSystemWatcher;
        }

        /// <summary>
        /// Stop all active watchers.
        /// </summary>
        private void StopAllWatchers()
        {
            if (null == _fileSystemWatchers) return; // Nothing to stop

            foreach (var watcher in _fileSystemWatchers)
            {
                watcher.Value.EnableRaisingEvents = false;
                watcher.Value.Dispose();
            }

            // Empty the watcher queue
            _fileSystemWatchers.Clear();
        }

        /// <summary>
        /// If we are watching, stop watching.  If we are not watching, start
        /// watching.
        /// </summary>
        private void ToggleWatchingStatus(object sender, System.EventArgs e)
        {
            if (_isWatching)
            {
                WriteLogEntry($"Attempting to stop all watchers");

                StopAllWatchers();

                _isWatching = false;
                WriteLogEntry("All watchers have been stopped.");
                _watchButton.Text = "Start Watching";
                _logFilePath = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_watchPaths.Text))
                {
                    WriteLogEntry("No directories were found in the watch list, unable to start.");
                    return;
                }
                
                // Save off the current log file path and attempt to log
                _logFilePath = _logfilePath.Text;
                WriteLogEntry("Attempting to start all directory watchers.");

                // Start all watchers
                var watchPaths = _watchPaths.Text.Split(new string[] { Environment.NewLine }, 
                                                        StringSplitOptions.None);

                foreach (var watchPath in watchPaths)
                {
                    CreateFileSystemWatcher(watchPath);
                }

                if (_fileSystemWatchers.Count > 0)
                {
                    _isWatching = true;
                    _watchButton.Text = "Stop Watching";
                    WriteLogEntry("All directory watchers started.");
                }
                else
                {
                    MessageBox.Show("No directory watchers actually started!");
                }
            }
        }

        /// <summary>
        /// Write the passed message to the configured logfile.  This method
        /// closes and flushes the log file after each write, so that if the
        /// computer crashes minimal data is lost.
        /// </summary>
        /// <param name="msg">The message to write.</param>
        private void WriteLogEntry(string msg)
        {
            using (var fileWriter = new StreamWriter(_logFilePath, true))
            {
                // Generate a sortable date for now and output the message
                var dateString = DateTime.Now.ToString("u");
                fileWriter.WriteLine($"{dateString} : {msg}");
                fileWriter.Close();
            }
        }

        /// <summary>
        /// A generic file changed handler, handles everything except renamed
        /// and error.
        /// </summary>
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            WriteLogEntry($"{e.FullPath} was {e.ChangeType}!");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            WriteLogEntry($"{e.FullPath} was renamed from {e.OldFullPath}!");
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            {
                // This happens if Windows reports lots of file change events
                // and the FileSystemWatcher buffer wasn't large enough to
                // handle them. The InternalBufferOverflowException error
                // informs the application that some of the file system events
                // were lost.
                WriteLogEntry(($"The watcher generated a buffer overflow: {e.GetException().Message}"));
            }
            else
            {
                WriteLogEntry($"Error: Watched directory not accessible at {DateTime.Now}");
            }

            HandleInternalBufferOverflowException((FileSystemWatcher)sender, e);
        }

        /// <summary>
        /// This method attempts to recover a FileSystemWatcher that has failed
        /// either due to buffer overflow (in which case the watcher has lost an
        /// unknown amount of messages and may, or may not, recover) or if the
        /// watched directory was deleted (in which case we try to wait until
        /// the directory comes back, which it often does).
        /// </summary>
        /// <param name="brokenFsw">The FileSystemWatcher that is, or may be, broken</param>
        /// <param name="e">The event that occurred</param>
        private void HandleInternalBufferOverflowException(FileSystemWatcher brokenFsw, ErrorEventArgs e)
        {
            Debug.Assert(brokenFsw != null, $"{nameof(brokenFsw)} != null");

            const int iMaxAttempts = 120;
            const int iTimeOut = 10000;

            var brokenPath = brokenFsw.Path;

            var i = 0;
            while (   (null == brokenFsw || !(Directory.Exists(brokenPath) && brokenFsw.EnableRaisingEvents)) 
                   && i < iMaxAttempts)
            {
                i += 1;
                try
                {
                    if (null != brokenFsw) brokenFsw.EnableRaisingEvents = false;

                    if (!Directory.Exists(brokenPath))
                    {
                        WriteLogEntry($"Directory '{brokenPath}' still inaccessible, pausing");
                    }
                    else
                    { 
                        // ReInitialize the Component, which should work
                        WriteLogEntry($"Trying to restart watcher for '{brokenPath}'");
                        var newFsw = CreateFileSystemWatcher(brokenPath);

                        // brokenFsw is disposed after this CreateFileSystemWatcher, even if it failed.
                        brokenFsw = null;

                        // Check to see if we were able to restart it.
                        if (null != newFsw && newFsw.EnableRaisingEvents)
                        {
                            WriteLogEntry($"Successfully restarted watcher for '{brokenPath}'");
                            return;
                        }
                    }
                }
                catch (Exception error)
                {
                    WriteLogEntry($"An unexpected error occurred while restarting the watcher for '{brokenPath}' {error.StackTrace}");
                    // One more attempt to stop events, to restart it
                    if (null != brokenFsw) brokenFsw.EnableRaisingEvents = false;
                }
                System.Threading.Thread.Sleep(iTimeOut);
            }

            // If we couldn't restart the file watcher, log it and get it out of the queue
            WriteLogEntry($"Unable to restart service for {brokenPath}.  Other watchers may still be active.");
            _fileSystemWatchers.Remove(brokenPath);
        }

        /// <summary>
        /// Handler for the button that allows them to pick a watch folder with a dialog.
        /// </summary>
        private void WatchFolderBrowseClick(object sender, System.EventArgs e)
        {
            _watchFolderBrowserDialog.SelectedPath = _watchFolders.Text;

            if (_watchFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _watchFolders.Text = _watchFolderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Handler for the button that allows them to pick a log file with a dialog.
        /// </summary>
        private void LogFileBrowseClick(object sender, System.EventArgs e)
        {
            _logFileBrowserDialog.InitialDirectory = Path.GetDirectoryName(_logfilePath.Text);
            _logFileBrowserDialog.FileName = Path.GetFileName(_logfilePath.Text);

            if (_logFileBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // Before saving, prove that this is a valid file path and that we can write to this file.
                try
                {
                    if (!File.Exists(_logFileBrowserDialog.FileName))
                    {
                        File.Create(_logFileBrowserDialog.FileName);
                    }

                    // Prove we can write.
                    using (var fw = new StreamWriter(_logFileBrowserDialog.FileName, true))
                    {
                    }

                    _logfilePath.Text = _logFileBrowserDialog.FileName;
                }
                catch
                {
                    MessageBox.Show($"Unable to initialize or open log file '{_logFileBrowserDialog.FileName}'");
                }
            }
        }

        private void _addWatchPath_Click (object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_watchFolders.Text) && Directory.Exists(_watchFolders.Text))
            {
                var oldText = _watchPaths.Text;
                var lines = oldText.Split(new string[] { Environment.NewLine }, 
                        StringSplitOptions.None)
                    .ToList();

                if (lines.Contains(_watchFolders.Text))
                {
                    MessageBox.Show("That path is already in the watch list.", "Warning", MessageBoxButtons.OK);
                    return;
                }

                // Add the new path at the top
                lines.Insert(0, _watchFolders.Text);

                if (lines.Count > WatchPathLimit)
                {
                    MessageBox.Show("Only the last 20 paths will actually be watched.", "Warning", MessageBoxButtons.OK);
                    lines = lines.Take(WatchPathLimit).ToList();
                }

                _watchPaths.Text = string.Join(Environment.NewLine, lines);
            }
        }
    }
}
