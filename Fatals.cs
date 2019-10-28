/* Fatals.cs
Description:
    * Fatal exception that if raised will stop application after displaying detailed error message.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CSharpObjectLibrary.FileTypes.Files;

namespace CSharpObjectLibrary.Exceptions.Fatals
{
    /// <summary>
    /// Fatal exception that if raised will stop application after displaying detailed error message to user and in the log file.
    /// </summary>
    public abstract class Fatal : GenericException
    {
        #region Constructors
        public Fatal() : base()
        {

        }
        public Fatal(ExceptionInfo info) : base(info)
        {

        }
        public Fatal(string callingFunc, DateTime timeStamp, params string[] messages) : base(new ExceptionInfo(callingFunc, timeStamp, messages))
        {

        }
        #endregion
        #region Class Methods
        protected string IndicatorMessage()
        {
            return "(Fatal) ";
        }
        #endregion
    }
    #region Derived Fatal Exceptions
    /// <summary>
    /// Exception details all files that could not be opened due to being used by another application, that prevent the
    /// application from functioning entirely.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("CSharpObjectLibrary.Exceptions.FilesInUse")]
    public class FilesInUse : Fatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, ExceptionInfo> _FilesInUse;
        #endregion
        #region Constructors
        /// <summary>
        /// Generate new GenericValueErrors container with single issue. SpecificMessage is expected to include the path. 
        /// </summary>
        /// <param name="fileInUse"></param>
        /// <param name="filePath"></param>
        /// <param name="callingFunc"></param>
        public FilesInUse(string fileInUse, string filePath, string callingFunc) : base("", DateTime.Now)
        { 
            this._FilesInUse = new Dictionary<string, ExceptionInfo>() { { fileInUse, new ExceptionInfo(callingFunc, DateTime.Now, fileInUse, filePath) } };
        }
        /// <summary>
        /// Use container containing messages indicating which files were in use to instantiate exception.  
        /// </summary>
        /// <param name="filesInUse"></param>
        public FilesInUse(Dictionary<string, ExceptionInfo> filesInUse)
        {
            this._FilesInUse = filesInUse;
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Append new exception to container. Will skip if file name already present in container.  
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="info"></param>
        public void AppendExcept(string fileName, ExceptionInfo info)
        {
            if (!this._FilesInUse.ContainsKey(fileName))
            {
                this._FilesInUse.Add(fileName, info);
            }
        }
        /// <summary>
        /// Return granular message for log file, only if any exceptions were actually loaded. 
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            StringBuilder message = new StringBuilder();
            if (this._FilesInUse.Count > 0)
            {
                message.Append(this.IndicatorMessage());
                message.AppendLine("The following files are in-use and could not be opened:");
                message.AppendLine("{");

                foreach (var pair in this._FilesInUse)
                {
                    message.AppendLine(pair.Key + ", Path: " + pair.Value.Messages[1]);
                }

                message.AppendLine("\n}");
            }
            return message.ToString();
        }
        /// <summary>
        /// Return concise message detailing which files were missing. 
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            StringBuilder message = new StringBuilder();
            if (this._FilesInUse.Count > 0)
            {
                message.Append(String.Format("{0} files are in-use and could not be opened.", this._FilesInUse.Keys.Count));
            }
            return message.ToString();
        }
        #endregion
        #region Interface Implementation
        /// <summary>
        /// Return number of elements in container.
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._FilesInUse.Count();
        }
        /// <summary>
        /// Merge passed exception with this object if acceptable.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is FilesInUse)
            {
                foreach (var pair in ((FilesInUse)except)._FilesInUse)
                {
                    if (!this._FilesInUse.ContainsKey(pair.Key))
                    {
                        this._FilesInUse.Add(pair.Key, pair.Value);
                    }
                }
            }
        }
        /// <summary>
        /// Return all elements of container as log file rows.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            List<LogFileRow> output = new List<LogFileRow>();
            StringBuilder message = new StringBuilder();
            uint row = 0;
            foreach (var pair in this._FilesInUse)
            {
                message.Clear();
                message.Append(this.IndicatorMessage());
                message.Append("File In Use: { Name: ");
                message.Append(pair.Value.Messages[0]);
                message.Append(", Path: ");
                message.Append(pair.Value.Messages[1]);
                message.Append(" }");
                output.Add(new LogFileRow(message.ToString(), row++, pair.Value.TimeStamp, pair.Value.CallingFunction));
            }
            return output;
        }
        #endregion
    }
    /// <summary>
    /// Exception details all files that had formats (ex: missing columns or rows) that differed from expected format, that 
    /// prevent the application from functioning entirely.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("CSharpObjectLibrary.Exceptions.FileFormatIssues")]
    public class FileFormatIssues : Fatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, ExceptionInfo> _FilesWithIssues;
        #endregion
        #region Constructors
        /// <summary>
        /// Construct single exception with passed attributes. 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filePath"></param>
        /// <param name="issue"></param>
        /// <param name="callingFunc"></param>
        public FileFormatIssues(string file, string filePath, string issue, string callingFunc) : base("", DateTime.Now)
        {
            this._FilesWithIssues = new Dictionary<string, ExceptionInfo>() { { file, new ExceptionInfo(callingFunc, DateTime.Now, filePath, issue) } };
        }
        /// <summary>
        /// Construct exception object with all exceptions contained in the passed map.  
        /// </summary>
        /// <param name="filesWithIssues"></param>
        public FileFormatIssues(Dictionary<string, ExceptionInfo> filesWithIssues) : base("", DateTime.Now)
        {
            this._FilesWithIssues = filesWithIssues;
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return concise message describind file formatting issues. 
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            StringBuilder message = new StringBuilder();
            if (this._FilesWithIssues.Count > 0)
            {
                message.Append(String.Format("{0} files had formatting issues.", this._FilesWithIssues.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return detailed message for use in log file. 
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            StringBuilder message = new StringBuilder();
            if (this._FilesWithIssues.Count > 0)
            {
                message.Append(this.IndicatorMessage());
                message.AppendLine("The following files had formatting issues: ");
                message.AppendLine("{");
                foreach (var pair in this._FilesWithIssues)
                {
                    message.AppendLine("\t" + pair.Key + ":");
                    message.AppendLine("\t  Path:" + pair.Value.Messages[0]);
                    message.AppendLine("\t  Issue:" + pair.Value.Messages[1]);
                }
                message.AppendLine("}");
            }
            return message.ToString();
        }
        #endregion
        #region Interface Implementation
        /// <summary>
        /// Return number of elements in this container. 
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._FilesWithIssues.Keys.Count;
        }
        /// <summary>
        /// Merge passed FileFormatIssues object if valid.  
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is FileFormatIssues)
            {
                foreach (var pair in ((FileFormatIssues)except)._FilesWithIssues)
                {
                    if (!this._FilesWithIssues.ContainsKey(pair.Key))
                    {
                        this._FilesWithIssues.Add(pair.Key, pair.Value);
                    }
                }
            }
        }
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            List<LogFileRow> output = new List<LogFileRow>();
            StringBuilder message = new StringBuilder();
            uint row = 0;
            foreach (var pair in this._FilesWithIssues)
            {
                message.Clear();
                message.Append(this.IndicatorMessage());
                message.Append("File Has Formatting Issues: { Issue: ");
                message.Append(pair.Value.Messages[1]);
                message.Append(", Path: ");
                message.Append(pair.Value.Messages[0]);
                message.Append(" }");
                output.Add(new LogFileRow(message.ToString(), row++, pair.Value.TimeStamp, pair.Value.CallingFunction));
            }
            return output;
        }
        #endregion
    }
    /// <summary>
    /// Exception details all critical folders that were missing.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("CSharpObjectLibrary.Exceptions.FileFormatIssues")]
    public class MissingFolders : Fatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, ExceptionInfo> _MissingFolders;
        #endregion
        #region Constructors
        /// <summary>
        /// Generate new MissingFolders container with single issue. 
        /// </summary>
        /// <param name="missingFolder"></param>
        /// <param name="folderPath"></param>
        /// <param name="callingFunction"></param>
        /// <param name="reason"></param>
        public MissingFolders(string folderPath, string callingFunction, string reason = "") : base("", DateTime.Now)
        {
            this._MissingFolders = new Dictionary<string, ExceptionInfo>() { { folderPath, new ExceptionInfo(callingFunction, DateTime.Now, folderPath, reason) } };
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return concise message fit for display in a messagebox to user.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            StringBuilder message = new StringBuilder();

            if (this._MissingFolders.Count > 0)
            {
                message.Append(String.Format("{0} folders are missing.", this._MissingFolders.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return fully detailed message for printing in log file.
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            StringBuilder message = new StringBuilder();
            if (this._MissingFolders.Count > 0)
            {
                message.Append(this.IndicatorMessage());
                message.AppendLine(" The following folders are missing:{\n");
                foreach (var pair in this._MissingFolders)
                {
                    message.Append(pair.Value.Messages[0] + ((!String.IsNullOrWhiteSpace(pair.Value.Messages[1]) ? ", reason: " + pair.Value.Messages[1] : "")));
                }
                message.Append("\n}");

            }
            return message.ToString();
        }
        #endregion
        #region Interface Implementation
        /// <summary>
        /// Return number of missing folders.
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._MissingFolders.Count;
        }
        /// <summary>
        /// Merge the passed exception container with this one.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is MissingFolders)
            {
                foreach (var missing in ((MissingFolders)except)._MissingFolders)
                {
                    if (!this._MissingFolders.ContainsKey(missing.Key))
                    {
                        this._MissingFolders.Add(missing.Key, new ExceptionInfo());
                    }
                    this._MissingFolders[missing.Key] = missing.Value;    
                }
            }
        }
        /// <summary>
        /// Return all elements of container as log file rows.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            List<LogFileRow> output = new List<LogFileRow>();
            StringBuilder message = new StringBuilder();
            uint row = 0;
            foreach (var pair in this._MissingFolders)
            {
                message.Clear();
                message.Append(this.IndicatorMessage());
                message.Append("Missing Folder: { ");
                message.Append(pair.Value.Messages[0]);
                message.Append(" }");
                output.Add(new LogFileRow(message.ToString(), row++, pair.Value.TimeStamp, pair.Value.CallingFunction));
            }

            return output;
        }
        #endregion
    }
    /// <summary>
    /// Exception details all missing configuration files for application.
    /// </summary>
    public class MissingConfigFiles : Fatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, ExceptionInfo> _MissingFiles;
        #endregion
        #region Constructors
        /// <summary>
        /// Construct new container with passed missing configuration file name and path.
        /// </summary>
        /// <param name="missingConfig"></param>
        /// <param name="path"></param>
        /// <param name="callingFunc"></param>
        public MissingConfigFiles(string missingConfig, string path, string callingFunc, string reason = "") : base(callingFunc, DateTime.Now, missingConfig, path, reason)
        {
            this._MissingFiles = new Dictionary<string, ExceptionInfo>() { { missingConfig, new ExceptionInfo(callingFunc, this.TimeStamp, missingConfig, path) } };
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return concise message for use in message box.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            StringBuilder message = new StringBuilder();
            if (this._MissingFiles.Count > 0)
            {
                message.Append(String.Format("{0} configuration files were missing.", this._MissingFiles.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return 
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            StringBuilder message = new StringBuilder();
            if (this._MissingFiles.Count > 0)
            {
                message.Append(this.IndicatorMessage());
                message.AppendLine("The following application configuration files could not be opened:");
                message.AppendLine("{");

                foreach (var pair in this._MissingFiles)
                {
                    message.AppendLine(pair.Key + ", Path: " + pair.Value.Messages[0]);
                }

                message.AppendLine("\n}");
            }
            return message.ToString();
        }
        #endregion
        #region Interface Implementations
        /// <summary>
        /// Return number of exceptions in the container.
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._MissingFiles.Count;
        }
        /// <summary>
        /// Merge passed exception with this object if valid.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is MissingConfigFiles)
            {
                foreach (var pair in ((MissingConfigFiles)except)._MissingFiles)
                {
                    if (!this._MissingFiles.ContainsKey(pair.Key))
                    {
                        this._MissingFiles.Add(pair.Key, pair.Value);
                    }
                }
            }
        }
        /// <summary>
        /// Convert all stored exceptions as rows for LogFile.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            List<LogFileRow> output = new List<LogFileRow>();
            StringBuilder message = new StringBuilder();
            uint row = 0;
            foreach (var pair in this._MissingFiles)
            {
                message.Clear();
                message.Append(this.IndicatorMessage());
                message.Append("Missing Config File: { ");
                message.Append(pair.Value.Messages[0]);
                if (!String.IsNullOrWhiteSpace(pair.Value.Messages[1]))
                {
                    message.Append(", Reason:" + pair.Value.Messages[1]);
                }
                message.Append(" }");
                output.Add(new LogFileRow(message.ToString(), row++, pair.Value.TimeStamp, pair.Value.CallingFunction));
            }
            return output;
        }
        #endregion
    }
    #endregion
}
