using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BallanceTasEditor.Backend {
    public class FileWatcher : IDisposable {
        /// <summary>
        /// Create a file watcher.
        /// </summary>
        /// <remarks>
        /// This new created file watcher is not watching specified file
        /// unless you explicitly call <see cref="Start"/> method.
        /// </remarks>
        /// <param name="filepath">The path to watching file.</param>
        public FileWatcher(string filepath) {
            m_FilePath = filepath;
            m_IsWatching = false;
            m_EventMutex = new Mutex();
            m_IsEventProcessing = false;
            
            // Get directory and file info
            string directory = Path.GetDirectoryName(filepath) ?? throw new ArgumentException("Invalid file path", nameof(filepath));
            string filename = Path.GetFileName(filepath);
            
            // Create FileSystemWatcher
            m_FileSystemWatcher = new FileSystemWatcher(directory, filename) {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
                EnableRaisingEvents = false
            };
            m_FileSystemWatcher.Changed += OnFileSystemChanged;
            m_FileSystemWatcher.Deleted += OnFileSystemDeleted;
            m_FileSystemWatcher.Renamed += OnFileSystemRenamed;
        }

        /// <summary>
        /// The path to watching file.
        /// </summary>
        private string m_FilePath;
        /// <summary>
        /// Whether the file watcher is watching file.
        /// </summary>
        private bool m_IsWatching;
        /// <summary>
        /// The FileSystemWatcher instance.
        /// </summary>
        private readonly FileSystemWatcher m_FileSystemWatcher;
        
        /// <summary>
        /// Flag to indicate if disposed.
        /// </summary>
        private bool m_Disposed = false;

        /// <summary>
        /// Start watching file.
        /// </summary>
        public void Start() {
            if (m_Disposed) throw new ObjectDisposedException(nameof(FileWatcher));
            
            if (m_IsWatching) {
                throw new InvalidOperationException("File watcher is already watching file.");
            } else {
                m_FileSystemWatcher.EnableRaisingEvents = true;
                m_IsWatching = true;
            }
        }

        /// <summary>
        /// Stop watching file.
        /// </summary>
        public void Stop() {
            if (m_Disposed) throw new ObjectDisposedException(nameof(FileWatcher));
            
            if (m_IsWatching) {
                m_FileSystemWatcher.EnableRaisingEvents = false;
                m_IsWatching = false;
            } else {
                throw new InvalidOperationException("File watcher is not watching file.");
            }
        }

        /// <summary>
        /// Dispose the file watcher and release resources.
        /// </summary>
        public void Dispose() {
            if (!m_Disposed) {
                // Stop watching.
                if (m_IsWatching) {
                    Stop();
                }
                // Dispose members
                m_FileSystemWatcher.Dispose();
                m_EventMutex.Dispose();
                m_Disposed = true;
            }
        }

        /// <summary>
        /// The event handler when file is modified.
        /// </summary>
        public delegate void FileModifiedHandler();
        /// <summary>
        /// The event handler when file is deleted.
        /// </summary>
        public delegate void FileDeletedHandler();

        /// <summary>
        /// The event when file is modified.
        /// </summary>
        /// <remarks>
        /// Before user process this event completely,
        /// there is no any other event will be triggered.
        /// </remarks>
        public event FileModifiedHandler? FileModified;
        /// <summary>
        /// The event when file is deleted.
        /// </summary>
        /// <remarks>
        /// Before user process this event completely,
        /// there is no any other event will be triggered.
        /// </remarks>
        public event FileDeletedHandler? FileDeleted;

        private Mutex m_EventMutex;
        private bool m_IsEventProcessing;

        private void OnFileModified() {
            if (FileModified is not null) {
                lock (m_EventMutex) {
                    if (m_IsEventProcessing) return;
                    else m_IsEventProcessing = true;
                }
                try {
                    FileModified.Invoke();
                }
                finally {
                    lock (m_EventMutex) {
                        m_IsEventProcessing = false;
                    }
                }
            }

        }

        private void OnFileDeleted() {
            if (FileDeleted is not null) {
                lock (m_EventMutex) {
                    if (m_IsEventProcessing) return;
                    else m_IsEventProcessing = true;
                }
                try {
                    FileDeleted.Invoke();
                }
                finally {
                    lock (m_EventMutex) {
                        m_IsEventProcessing = false;
                    }
                }
            }
        }
        
        /// <summary>
        /// Handler for FileSystemWatcher Changed event.
        /// </summary>
        private void OnFileSystemChanged(object sender, FileSystemEventArgs e) {
            // Filter out our own change notifications to avoid infinite loops
            if (e.ChangeType == WatcherChangeTypes.Changed) {
                OnFileModified();
            }
        }
        
        /// <summary>
        /// Handler for FileSystemWatcher Deleted event.
        /// </summary>
        private void OnFileSystemDeleted(object sender, FileSystemEventArgs e) {
            OnFileDeleted();
        }
        
        /// <summary>
        /// Handler for FileSystemWatcher Renamed event.
        /// </summary>
        private void OnFileSystemRenamed(object sender, RenamedEventArgs e) {
            // Treat rename as a delete since the original file is gone
            OnFileDeleted();
        }
    }
}
