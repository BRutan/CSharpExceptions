/* SemiFatals.cs
Description:
    * "Semi" fatal exception that if raised will stop key operation from finishing and display detailed error message, but keep application open.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CSharpObjectLibrary.FileTypes.Files;

namespace CSharpObjectLibrary.Exceptions.SemiFatals
{
    /// <summary>
    /// "Semi" fatal exception that if raised will stop key operation from finishing and display detailed error message, but keep application open.
    /// </summary>
    public abstract class SemiFatal : GenericException
    {
        #region Constructors
        public SemiFatal() : base()
        {

        }
        public SemiFatal(ExceptionInfo info) : base(info)
        {

        }
        public SemiFatal(string callingFunc, DateTime timeStamp, params string[] messages) : base(new ExceptionInfo(callingFunc, timeStamp, messages))
        {

        }
        #endregion
        #region Class Methods
        protected string IndicatorMessage()
        {
            return "(SemiFatal) ";
        }
        #endregion
    }
    #region Derived SemiFatal Exceptions
    /// <summary>
    /// Details any data files that do not exist, but are not critical to the application running. 
    /// </summary>
    public class MissingInputFiles : SemiFatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, ExceptionInfo> _MissingFiles;
        #endregion
        #region Constructors
        /// <summary>
        /// Generate new GenericValueErrors container with single issue. Message is expected to contain the full path. 
        /// </summary>
        /// <param name="missingFile"></param>
        /// <param name="filePath"></param>
        /// <param name="callingFunc"></param>
        public MissingInputFiles(string missingFile, string filePath, string callingFunc, string reason = "") : base("", DateTime.Now)
        {
            this._MissingFiles = new Dictionary<string, ExceptionInfo>() { { missingFile, new ExceptionInfo(callingFunc, DateTime.Now, filePath, reason) } };
        }
        /// <summary>
        /// Use container containing messages indicating which files were missing to instantiate exception.  
        /// </summary>
        /// <param name="MissingFiles_in"></param>
        public MissingInputFiles(Dictionary<string, ExceptionInfo> MissingFiles_in)
        {
            this._MissingFiles = MissingFiles_in;
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return compact message for use in message box.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            StringBuilder message = new StringBuilder();
            if (this._MissingFiles.Count > 0)
            {
                message.Append(String.Format("{0} data files could not loaded.", this._MissingFiles.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return granular message for log file, only if any exceptions were actually loaded. 
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            StringBuilder message = new StringBuilder();

            if (this._MissingFiles.Count > 0)
            {
                message.Append(this.IndicatorMessage());
                message.AppendLine("The following data files could not be found:");
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
        #region Interface Implementation
        /// <summary>
        /// Return number of exceptions in the container.
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this.Data.Count;
        }
        /// <summary>
        /// Merge with passed MissingInputFiles if acceptable. 
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is MissingInputFiles)
            {
                foreach (var pair in ((MissingInputFiles)except)._MissingFiles)
                {
                    if (!this._MissingFiles.ContainsKey(pair.Key))
                    {
                        this._MissingFiles.Add(pair.Key, pair.Value);
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
            foreach (var pair in this._MissingFiles)
            {
                message.Clear();
                message.Append(this.IndicatorMessage());
                message.Append("Missing Config File: { ");
                message.Append(pair.Value.Messages[0]);
                message.Append(" }");
                output.Add(new LogFileRow(message.ToString(), row++, pair.Value.TimeStamp, pair.Value.CallingFunction));
            }
            return output;
        }
        #endregion
    }
    /// <summary>
    /// Exception details all reports that failed to be generated.
    /// </summary>
    public class FailedToGenerateReports : SemiFatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, ExceptionInfo> _FailedReports;
        #endregion
        #region Constructors
        public FailedToGenerateReports(string reportName, string callingFunc, string reason = "")
        {
            this._FailedReports = new Dictionary<string, ExceptionInfo>() { { reportName, new ExceptionInfo(callingFunc, DateTime.Now, reportName, reason) } };
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return concise message detailing reports that failed to generate.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            var message = new StringBuilder();
            if (this._FailedReports.Count > 0)
            {
                message.Append(String.Format("{0} reports failed to generate.", this._FailedReports.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return detailed message for use in log file.
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            var message = new StringBuilder();
            if (this._FailedReports.Count > 0)
            {
                message.AppendLine("The following reports failed to generate: ");
                message.AppendLine("{");
                foreach (var pair in this._FailedReports)
                {
                    message.Append(String.Format("Report: {0}", pair.Value.Messages[0]));
                    // Append the reason if provided:
                    if (String.IsNullOrWhiteSpace(pair.Value.Messages[1]))
                    {
                        message.Append(String.Format(", Reason: {0}", pair.Value.Messages[1]));
                    }
                    message.Append("\n");
                }
                message.Append("\n}");

            }
            return message.ToString();
        }
        #endregion
        #region Interface Implementations
        /// <summary>
        /// Return number reports that failed to generate.
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._FailedReports.Count;
        }
        /// <summary>
        /// Merge passed exception with this one.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is FailedToGenerateReports)
            {
                foreach (var pair in ((FailedToGenerateReports)except)._FailedReports)
                {
                    if (!this._FailedReports.ContainsKey(pair.Key))
                    {
                        this._FailedReports.Add(pair.Key, pair.Value);
                    }
                }
            }
        }
        /// <summary>
        /// Return all failed reports as log file rows.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            uint row = 0;
            List<LogFileRow> output = new List<LogFileRow>();
            StringBuilder message = new StringBuilder();
            foreach (var pair in this._FailedReports)
            {
                message.Clear();
                message.Append(this.IndicatorMessage());
                message.Append("{Report: ");
                message.Append(pair.Key);
                // Append reason that report was not generated if provided:
                if (!String.IsNullOrWhiteSpace(pair.Value.Messages[1]))
                {
                    message.Append(String.Format(", Reason: {0}", pair.Value.Messages[1]));
                }
                message.Append("}");
                output.Add(new LogFileRow(message.ToString(), row++, pair.Value.TimeStamp, pair.Value.CallingFunction));
            }
            return output;
        }
        #endregion
    }
    /// <summary>
    /// Exception details all sheets that could not be started/completed in the derived ReportType object. Primary key is document name, values are all necessary exception 
    /// info for failed sheets.
    /// </summary>
    public class FailedToCreateSheets : SemiFatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, List<ExceptionInfo>> _FailedSheetsByDocument;
        #endregion
        #region Constructors
        /// <summary>
        /// Create new exception container using information about single document -> sheet pair.
        /// </summary>
        /// <param name="failedSheet"></param>
        /// <param name="callingFunc"></param>
        /// <param name="documentName"></param>
        /// <param name="reason"></param>
        public FailedToCreateSheets(string failedSheet, string callingFunc, string documentName, string reason = "") : base("", DateTime.Now)
        {
            this._FailedSheetsByDocument = new Dictionary<string, List<ExceptionInfo>>()
            {
                { documentName, new List<ExceptionInfo>() { new ExceptionInfo(callingFunc, DateTime.Now, documentName, failedSheet, reason) } }
            };
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return concise message fit for printing to user in MessageBox.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            StringBuilder message = new StringBuilder();
            if (this._FailedSheetsByDocument.Count > 0)
            {
                int failedSheetsCount = 0;
                foreach (var pair in this._FailedSheetsByDocument)
                {
                    failedSheetsCount += pair.Value.Count;
                }
                message.Append(String.Format("{0} sheets failed to be created across {1} documents.", failedSheetsCount, this._FailedSheetsByDocument.Keys.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return more detailed message.
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            StringBuilder message = new StringBuilder();
            if (this._FailedSheetsByDocument.Count > 0)
            {
                foreach (var docPair in this._FailedSheetsByDocument)
                {
                    message.Append(String.Format("Could not generate following sheets for {0}: {", docPair.Key));
                    message.Append(String.Join(",", docPair.Value.Select((info) => info.Messages[1])));
                    message.Append("}\n");
                }
            }
            return message.ToString();
        }
        #endregion
        #region Interface Implementations
        /// <summary>
        /// Get number of exceptions in container.
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._FailedSheetsByDocument.Keys.Count;
        }
        /// <summary>
        /// Merge passed exception with this object, if applicable.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is FailedToCreateSheets)
            {
                foreach (var pair in ((FailedToCreateSheets)except)._FailedSheetsByDocument)
                {
                    if (!this._FailedSheetsByDocument.ContainsKey(pair.Key))
                    {
                        this._FailedSheetsByDocument.Add(pair.Key, new List<ExceptionInfo>());
                    }
                    this._FailedSheetsByDocument[pair.Key].AddRange(pair.Value);
                }
            }
        }
        /// <summary>
        /// Output all exceptions in container as log file rows.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            List<LogFileRow> output = new List<LogFileRow>();
            var message = new StringBuilder();
            uint row = 0;
            foreach (var pair in this._FailedSheetsByDocument)
            {
                foreach (var sheetInfo in pair.Value)
                {
                    message.Clear();
                    message.Append(this.IndicatorMessage());
                    message.Append("Failed to Generate Sheet: {");
                    message.Append(String.Format("Document: {0}, Sheet: {1}", pair.Key, sheetInfo.Messages[1]));
                    if (!String.IsNullOrWhiteSpace(sheetInfo.Messages[2]))
                    {
                        message.Append(String.Format(", Reason: {0}", sheetInfo.Messages[2]));
                    }
                    message.Append("}");
                    output.Add(new LogFileRow(message.ToString(), row, sheetInfo.TimeStamp, sheetInfo.CallingFunction));
                }
            }
            return output;
        }
        #endregion
    }
    /// <summary>
    /// Details any files that had formatting issues (ex: missing columns or rows), but are not critical to the application running.
    /// </summary>
    public class FileFormatIssues : SemiFatal, IMultipleElements
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
        public FileFormatIssues(Dictionary<string, ExceptionInfo> filesWithIssues) : base()
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
                message.AppendLine(" The following files had formatting issues: ");
                message.AppendLine("{");
                foreach (var pair in this._FilesWithIssues)
                {
                    message.AppendLine("\t" + pair.Key + ":");
                    message.AppendLine("\t  Path:" + pair.Value.Messages[0]);
                    message.AppendLine("\t  Issue:" + pair.Value.Messages[1]);
                }
                message.AppendLine("\n}");
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
        /// <summary>
        /// Return all elements of container as log file rows.
        /// </summary>
        /// <returns></returns>
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
    /// Details any files that could not be loaded due to their being used by other applications, but are not critical to the application running.
    /// </summary>
    public class FilesInUse : SemiFatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, ExceptionInfo> _FilesInUse;
        #endregion
        #region Constructors
        /// <summary>
        /// Generate new GenericValueErrors container with single issue.
        /// </summary>
        /// <param name="fileInUse"></param>
        /// <param name="filePath"></param>
        /// <param name="callingFunc"></param>
        public FilesInUse(string fileInUse, string filePath, string callingFunc) : base("", DateTime.Now)
        {
            this._FilesInUse = new Dictionary<string, ExceptionInfo>() { { fileInUse, new ExceptionInfo(callingFunc, DateTime.Now, fileInUse, filePath) } };
        }
        /// <summary>
        /// Use container containing messages indicating which files were missing to instantiate exception.  
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
                    message.AppendLine(pair.Key + ", Path: " + pair.Value.Messages[0]);
                }

                message.AppendLine("\n}");
            }
            return message.ToString();
        }
        /// <summary>
        /// Return compact message for use in messagebox.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            StringBuilder message = new StringBuilder();
            if (this._FilesInUse.Count > 0)
            {
                message.Append(String.Format("{0} files are in-use and could not be opened.", this._FilesInUse.Count));
            }
            return message.ToString();
        }
        #endregion
        #region Interface Implementation
        /// <summary>
        /// Return number of exceptions in the exception container.
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._FilesInUse.Count;
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
    /// Details any folders that are missing, and are required for a key process in the application.
    /// </summary>
    public class MissingFolders : SemiFatal, IMultipleElements
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
            StringBuilder message = new StringBuilder(this.IndicatorMessage());
            message.AppendLine("The following folders are missing: {");
            foreach(var pair in this._MissingFolders)
            {
                message.Append(pair.Value.Messages[0] + ((!String.IsNullOrWhiteSpace(pair.Value.Messages[1]) ? ", reason: " + pair.Value.Messages[1] : "")));
            }
            message.AppendLine("\n}");

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
    /// Details any files that could not be generated, and were part of a key application process.
    /// </summary>
    public class FailedToGenerateFiles : SemiFatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, ExceptionInfo> _FailedFiles;
        #endregion
        #region Constructors
        /// <summary>
        /// Generate new MissingFolders container with single issue. 
        /// </summary>
        /// <param name="missingFolder"></param>
        /// <param name="folderPath"></param>
        /// <param name="callingFunction"></param>
        /// <param name="reason"></param>
        public FailedToGenerateFiles(string fileName, string filePath, string callingFunction, string reason = "") : base("", DateTime.Now)
        {
            this._FailedFiles = new Dictionary<string, ExceptionInfo>() { { fileName, new ExceptionInfo(callingFunction, DateTime.Now, fileName, filePath, reason) } };
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

            if (this._FailedFiles.Count > 0)
            {
                message.Append(String.Format("{0} files could not be generated.", this._FailedFiles.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return fully detailed message for printing in log file.
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            StringBuilder message = new StringBuilder(this.IndicatorMessage());
            message.AppendLine("The following files could not be generated: {");
            foreach (var pair in this._FailedFiles)
            {
                message.Append(pair.Value.Messages[0] + ((!String.IsNullOrWhiteSpace(pair.Value.Messages[2]) ? ", reason: " + pair.Value.Messages[2] : "")));
            }
            message.AppendLine("\n}");

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
            return this._FailedFiles.Count;
        }
        /// <summary>
        /// Merge the passed exception container with this one.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is FailedToGenerateFiles)
            {
                foreach (var fileInfo in ((FailedToGenerateFiles)except)._FailedFiles)
                {
                    if (!this._FailedFiles.ContainsKey(fileInfo.Key))
                    {
                        this._FailedFiles.Add(fileInfo.Key, new ExceptionInfo());
                    }
                    this._FailedFiles[fileInfo.Key] = fileInfo.Value;
                }
            }
        }
        /// <summary>
        /// Return all elements of container as log file rows.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            var output = new List<LogFileRow>();
            var message = new StringBuilder();
            uint row = 0;

            foreach (var pair in this._FailedFiles)
            {
                message.Clear();
                message.Append(this.IndicatorMessage());
                message.Append("Failed to generate file: { ");
                message.Append(pair.Value.Messages[0]);
                message.Append(", Path: ");
                message.Append(pair.Value.Messages[1]);
                if (!String.IsNullOrWhiteSpace(pair.Value.Messages[2]))
                {
                    message.Append(", Reason: ");
                    message.Append(pair.Value.Messages[2]);
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