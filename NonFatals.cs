/* NonFatals.cs
Description:
    * Nonfatal exception details errors that are neither critical to the application nor to a key application process. 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CSharpObjectLibrary.FileTypes.Files;

namespace CSharpObjectLibrary.Exceptions.NonFatals
{
    /// <summary>
    /// Nonfatal exception details errors that are neither critical to the application nor to a key application process. 
    /// </summary>
    public abstract class NonFatal : GenericException
    {
        #region Constructors
        public NonFatal() : base()
        {

        }
        public NonFatal(ExceptionInfo info) : base(info)
        {

        }
        public NonFatal(string callingFunc, DateTime timeStamp, params string[] messages) : base(new ExceptionInfo(callingFunc, timeStamp, messages))
        {

        }
        protected string IndicatorMessage()
        {
            return "(NonFatal) ";
        }
        #endregion
    }
    #region Derived NonFatal Exceptions

    /// <summary>
    /// Details any files that had formatting issues (ex: missing columns or rows), but are not critical to a process or the application running.
    /// </summary>
    public class FileFormatIssues : NonFatal, IMultipleElements
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
            this._FilesWithIssues = new Dictionary<string, ExceptionInfo>() { { file, new ExceptionInfo(callingFunc, DateTime.Now, filePath, issue)} };
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
        /// Return concise message describing file formatting issues. 
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
                    message.AppendLine("\t " + pair.Key + ":");
                    message.AppendLine("\t Path:" + pair.Value.Messages[0]);
                    message.AppendLine("\t Issue:" + pair.Value.Messages[1]);
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
        /// Merge passed FileFormatIssues object.
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
    public class FilesInUse : NonFatal, IMultipleElements
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
        /// Append new exception to container. Will skip if file name already present in container. Assumes that ExceptionInfo::Messages[0] contains the full path.
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
        /// Return concise message detailing which files were missing. 
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
        /// Return number of elements in exception container. 
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._FilesInUse.Count;
        }
        /// <summary>
        /// Merge passed exception with this container if valid.
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
    /// Exception occurs if a conversion error or some other ValueError (or similar) exception occurs. Essentially a programming mistake.
    /// </summary>
    public class GenericValueErrors : NonFatal, IMultipleElements
    {
        #region Class Members
        private List<ExceptionInfo> _Messages;
        #endregion
        #region Constructors
        /// <summary>
        /// Generate new GenericValueErrors container with single issue. 
        /// </summary>
        /// <param name="runtimeError"></param>
        /// <param name="callingFunc"></param>
        public GenericValueErrors(string runtimeError, string callingFunc) : base("", DateTime.Now)
        {
            this._Messages = new List<ExceptionInfo>() { new ExceptionInfo(callingFunc, DateTime.Now, runtimeError) };
        }
        /// <summary>
        /// Generate new GenericValueErrors container with multiple issues. 
        /// </summary>
        /// <param name="messages"></param>
        public GenericValueErrors(List<ExceptionInfo> messages) : base()
        {
            this._Messages = messages;
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return concise message of issues that can fit in a message box.
        /// </summary>
        public override string ConciseMessage()
        {
            StringBuilder message = new StringBuilder();
            if (this._Messages.Count > 0)
            {
                message.Append(String.Format("{0} run time programming logic issues occurred.", this._Messages.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return granular message for use in log file. 
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            StringBuilder message = new StringBuilder();
            // Return blank string if no issues were loaded:
            if (this._Messages.Count == 0)
            {
                // Return granular message:
                message.Append(this.IndicatorMessage());
                message.AppendLine("The following programming issues occurred at run time:");
                message.AppendLine("{");
                foreach (var elem in this._Messages)
                {
                    message.AppendLine(elem.Messages[0]);
                }
                message.AppendLine("\n}");
            }

            return message.ToString();
        }
        #endregion
        #region Interface Implementation
        /// <summary>
        /// Return number of issues that occurred. 
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._Messages.Count;
        }
        /// <summary>
        /// Merge with passed exception if valid.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (!(except is GenericValueErrors))
            {
                return;
            }

            foreach (var elem in ((GenericValueErrors)except)._Messages)
            {
                this._Messages.Add(elem);
            }
        }
        /// <summary>
        /// Return all elements of container as log file rows.
        /// </summary>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            List<LogFileRow> output = new List<LogFileRow>();
            StringBuilder message = new StringBuilder();
            uint row = 0;
            foreach (var except in this._Messages)
            {
                message.Clear();
                message.Append(this.IndicatorMessage());
                message.Append("Runtime error: { ");
                message.Append(except.Messages[0]);
                message.Append(" }");
                output.Add(new LogFileRow(message.ToString(), row++, except.TimeStamp, except.CallingFunction));
            }
            return output;
        }
        #endregion
    }
    /// <summary>
    /// Nonfatal exception that details all Merlin Curves that were missing from production locations.
    /// </summary>
    public class MissingCalibers : NonFatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, List<ExceptionInfo>> _MissingCalibersByFile;
        private HashSet<string> _UniqueMissingCalibersByFile;
        #endregion
        #region Constructors
        public MissingCalibers(string missingCaliber, string callingFunc, string file) : base("", DateTime.Now)
        {
            this._MissingCalibersByFile = new Dictionary<string, List<ExceptionInfo>>() { { file, new List<ExceptionInfo>() } };
            this._MissingCalibersByFile[file].Add(new ExceptionInfo(callingFunc, DateTime.Now, file, missingCaliber));
            this._UniqueMissingCalibersByFile = new HashSet<string>();
            this._UniqueMissingCalibersByFile.Add(missingCaliber + file);
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return concise message detailing the number of curves that could not be found in production locations.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            var message = new StringBuilder();
            if (this._MissingCalibersByFile.Count > 0)
            {
                message.Append(String.Format("{0} Caliber IDs were missing across {1} files.", this._UniqueMissingCalibersByFile.Count, this._MissingCalibersByFile.Keys.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return detailed message for log file.
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            var message = new StringBuilder();
            // Return blank string if no curves were missing:
            if (this._MissingCalibersByFile.Count > 0)
            {
                foreach (var pair in this._MissingCalibersByFile)
                {
                    message.Append(this.IndicatorMessage());
                    message.AppendLine(String.Format("The following Caliber IDs could not be found in {0}", pair.Key));
                    message.AppendLine("{");
                    message.AppendLine(String.Join(",", pair.Value.Select((item) => item.Messages[1])));
                    message.AppendLine("\n}");
                }
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
            return this._UniqueMissingCalibersByFile.Count;
        }
        /// <summary>
        /// Merge passed exception with this object.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is MissingCalibers)
            {
                foreach (var pair in ((MissingCalibers)except)._MissingCalibersByFile)
                {
                    if (!this._MissingCalibersByFile.ContainsKey(pair.Key))
                    {
                        this._MissingCalibersByFile.Add(pair.Key, new List<ExceptionInfo>());
                    }
                    this._MissingCalibersByFile[pair.Key].AddRange(pair.Value);
                    // Add all unique missing caliber ID + file pairs to the unique counter:
                    foreach (var info in pair.Value)
                    {
                        this._UniqueMissingCalibersByFile.Add(info.Messages[0]);
                    }
                }
            }
        }
        /// <summary>
        /// Output all stored missing curves as log file rows.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            List<LogFileRow> output = new List<LogFileRow>();
            var message = new StringBuilder();
            uint row = 0;
            foreach (var pair in this._MissingCalibersByFile)
            {
                foreach (var info in pair.Value)
                {
                    message.Clear();
                    message.Append(this.IndicatorMessage());
                    message.Append("Missing Caliber ID: {");
                    message.Append(String.Format("CaliberID: {0}, File: {1}", info.Messages[1], pair.Key));
                    message.Append("}");
                    output.Add(new LogFileRow(message.ToString(), row++, info.TimeStamp, info.CallingFunction));
                }
            }
            return output;
        }
        #endregion
    }
    /// <summary>
    /// Nonfatal exception that details all Merlin Curves that were missing from production locations.
    /// </summary>
    public class MissingCurves : NonFatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, ExceptionInfo> _MissingCurves;
        private string _ProductionFolder;
        #endregion
        #region Constructors
        public MissingCurves(string missingCurve, string callingFunc, string productionFolder, DateTime valueDate) : base("", DateTime.Now)
        {
            this._MissingCurves = new Dictionary<string, ExceptionInfo>() { { missingCurve, new ExceptionInfo(callingFunc, DateTime.Now, missingCurve, valueDate.ToString("MM/dd/yyyy")) } };
            this._ProductionFolder = productionFolder;
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return concise message detailing the number of curves that could not be found in production locations.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            var message = new StringBuilder();
            if (this._MissingCurves.Count > 0)
            {
                message.Append(String.Format("{0} merlin curves were missing.", this._MissingCurves.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return detailed message for log file.
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            var message = new StringBuilder();
            // Return blank string if no curves were missing:
            if (this._MissingCurves.Count > 0)
            {
                message.Append(this.IndicatorMessage());
                message.AppendLine("The following Merlin Curves could not be found in the production folder at ");
                message.AppendLine(this._ProductionFolder);
                message.AppendLine("{");
                message.AppendLine(String.Join(", ", this._MissingCurves));
                message.AppendLine("\n}");
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
            return this._MissingCurves.Count;
        }
        /// <summary>
        /// Merge passed exception with this object.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is MissingCurves)
            {
                foreach (var pair in ((MissingCurves)except)._MissingCurves)
                {
                    if (!this._MissingCurves.ContainsKey(pair.Key))
                    {
                        this._MissingCurves.Add(pair.Key, pair.Value);
                    }
                }
            }
        }
        /// <summary>
        /// Output all stored missing curves as log file rows.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            List<LogFileRow> output = new List<LogFileRow>();
            var message = new StringBuilder();
            uint row = 0;
            foreach (var pair in this._MissingCurves)
            {
                message.Clear();
                message.Append(this.IndicatorMessage());
                message.Append("Missing Merlin Curve: {");
                message.Append(String.Format("Curve: {0}, ValueDate: {1}, Path: {2}", pair.Key, pair.Value.Messages[1], this._ProductionFolder));
                message.Append("}");
                output.Add(new LogFileRow(message.ToString(), row++, pair.Value.TimeStamp, pair.Value.CallingFunction));
            }
            return output;
        }
        #endregion
    }
    /// <summary>
    /// Exception details all inflation indices that were missing in a particular file.
    /// </summary>
    public class MissingInflationIndices : NonFatal, IMultipleElements
    {
        #region Class Members
        private Dictionary<string, List<ExceptionInfo>> _MissingIndicesByFile;
        private HashSet<string> _UniqueIndicesByFile;
        #endregion
        #region Constructors
        public MissingInflationIndices(string missingIndex, string callingFunc, string file, DateTime? valueDate = null) : base("", DateTime.Now)
        {
            var valueDateString = ((valueDate.HasValue) ? valueDate.Value.ToString("MM/dd/yyyy") : "");
            this._MissingIndicesByFile = new Dictionary<string, List<ExceptionInfo>>();
            this._MissingIndicesByFile.Add(file, new List<ExceptionInfo>());
            this._MissingIndicesByFile[file].Add(new ExceptionInfo(callingFunc, DateTime.Now, file, missingIndex, valueDateString));
            this._UniqueIndicesByFile = new HashSet<string>() { missingIndex + file };
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return concise message detailing the number of curves that could not be found in production locations.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            var message = new StringBuilder();
            if (this._MissingIndicesByFile.Count > 0)
            {
                // Count the number of unique inflation indices that were missing across all files:
                message.Append(String.Format("{0} Inflation Indices were missing across {1} files.", this._UniqueIndicesByFile.Count, this._MissingIndicesByFile.Count));
            }
            return message.ToString();
        }
        /// <summary>
        /// Return detailed message for log file.
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            var message = new StringBuilder();
            // Return blank string if no indices were missing:
            if (this._MissingIndicesByFile.Count > 0)
            {
                // Generate the error message by grouping all inflation indices by file that they are missing from:
                foreach (var pair in this._MissingIndicesByFile)
                {
                    message.Append(this.IndicatorMessage());
                    message.AppendLine(String.Format("The following Inflation Indices could not be found in {0} file:", pair.Key));
                    message.AppendLine("{");
                    message.AppendLine(String.Join(",", pair.Value.Select((item) => item.Messages[0]).ToList()));
                    message.AppendLine("\n}");
                }
            }
            return message.ToString();
        }
        #endregion
        #region Interface Implementation
        /// <summary>
        /// Return number of missing inflation indices in container.
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            return this._MissingIndicesByFile.Count;
        }
        /// <summary>
        /// Merge passed exception with this object.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            if (except is MissingInflationIndices)
            {
                foreach (var pair in ((MissingInflationIndices)except)._MissingIndicesByFile)
                {
                    if (!this._MissingIndicesByFile.ContainsKey(pair.Key))
                    {
                        this._MissingIndicesByFile.Add(pair.Key, new List<ExceptionInfo>());
                    }
                    this._MissingIndicesByFile[pair.Key].AddRange(pair.Value);
                    // Append all missing indices to the unique container:
                    foreach (var item in pair.Value)
                    {
                        this._UniqueIndicesByFile.Add(item.Messages[0] + pair.Key);
                    }
                }
            }
        }
        /// <summary>
        /// Output all stored missing inflation indices with relevant information as log file rows.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            List<LogFileRow> output = new List<LogFileRow>();
            var message = new StringBuilder();
            uint row = 0;
            foreach (var pair in this._MissingIndicesByFile)
            {
                foreach (var info in pair.Value)
                {
                    message.Clear();
                    message.Append(this.IndicatorMessage());
                    message.Append("Missing Inflation Index: {");
                    message.Append(String.Format("IndexName: {0}, File: {1}", info.Messages[0], pair.Key));
                    if (String.IsNullOrWhiteSpace(info.Messages[2]))
                    {
                        message.Append(String.Join(", ValueDate: {0}", info.Messages[2]));
                    }
                    message.Append("}");
                    output.Add(new LogFileRow(message.ToString(), row++, info.TimeStamp, info.CallingFunction));
                }
            }
            return output;
        }
        #endregion
    }
    #endregion
}