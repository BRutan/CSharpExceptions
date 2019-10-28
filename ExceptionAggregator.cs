/* ExceptionAggregator.cs
Description:
    * Object aggregates all exceptions that occur at run time, then can convert exceptions into a log file and print to user in message box or any other available medium.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CSharpObjectLibrary.FileTypes.Files;

namespace CSharpObjectLibrary.Exceptions
{
    /// <summary>
    /// Object aggregates all exceptions that occur at run time, then can convert exceptions into a log file and print to user in message box or any other available medium.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("CSharpObjectLibrary.Exceptions.ExceptionAggregator")]
    public class ExceptionAggregator : GenericException, IMultipleElements
    {
        #region Static Members
        private static HashSet<Type> CustomTypes;
        private static int MessageBufferLength;
        #endregion
        #region Class Members
        private string Application;
        private Dictionary<Type, Dictionary<Type, GenericException>> _CustomExceptions;
        private Dictionary<DateTime, Exception> _SystemExceptions;
        #endregion
        #region Constructors
        static ExceptionAggregator()
        {
            ExceptionAggregator.CustomTypes = new HashSet<Type>() { typeof(Fatals.Fatal), typeof(NonFatals.NonFatal), typeof(SemiFatals.SemiFatal) };
            ExceptionAggregator.MessageBufferLength = 90;
        }
        /// <summary>
        /// Construct empty aggregator using optional parameter to indicate the name of the application in
        /// output messages.
        /// </summary>
        /// <param name="applicationName"></param>
        public ExceptionAggregator(string applicationName = "") : base()
        {
            this.Application = applicationName;
            this._CustomExceptions = new Dictionary<Type, Dictionary<Type, GenericException>>();
            this._SystemExceptions = new Dictionary<DateTime, Exception>();
        }
        #endregion
        #region Accessors
        /// <summary>
        /// If any semifatal/fatal/system exceptions occurred then indicate that should cancel the main operation.
        /// </summary>
        /// <returns></returns>
        public bool CancelOperation()
        {
            return this.HasSemiFatals() || this.HasFatals() || this.HasSysExcepts();
        }
        /// <summary>
        /// Return number of exceptions in this container.
        /// </summary>
        /// <returns></returns>
        public int ContainerCount()
        {
            return ((IMultipleElements)this).ContainerCount();
        }
        /// <summary>
        /// Return number of fatal exceptions in this container.
        /// </summary>
        /// <returns></returns>
        public int FatalCount()
        {
            if (!this._CustomExceptions.ContainsKey(typeof(Fatals.Fatal)))
            {
                return 0;
            }
            return this._CustomExceptions[typeof(Fatals.Fatal)].Count;
        }
        /// <summary>
        /// Indicate if any exceptions have occurred.
        /// </summary>
        /// <returns></returns>
        public bool HasErrors()
        {
            return this.HasFatals() || this.HasNonFatals() || this.HasSemiFatals() || this.HasSysExcepts();
        }
        /// <summary>
        /// Indicate if fatal exceptions occurred.
        /// </summary>
        /// <returns></returns>
        public bool HasFatals()
        {
            return this._CustomExceptions.ContainsKey(typeof(Exceptions.Fatals.Fatal)) && this._CustomExceptions[typeof(Exceptions.Fatals.Fatal)].Count > 0;
        }
        /// <summary>
        /// Indicate if any NonFatal exceptions have occurred.
        /// </summary>
        /// <returns></returns>
        public bool HasNonFatals()
        {
            return this._CustomExceptions.ContainsKey(typeof(Exceptions.NonFatals.NonFatal)) && this._CustomExceptions[typeof(Exceptions.NonFatals.NonFatal)].Count > 0;
        }
        /// <summary>
        /// Indicate if any semi-fatal exceptions occurred.
        /// </summary>
        /// <returns></returns>
        public bool HasSemiFatals()
        {
            return this._CustomExceptions.ContainsKey(typeof(Exceptions.SemiFatals.SemiFatal)) && this._CustomExceptions[typeof(Exceptions.SemiFatals.SemiFatal)].Count > 0;
        }
        /// <summary>
        /// Indicate if any system exceptions have occurred.
        /// </summary>
        /// <returns></returns>
        public bool HasSysExcepts()
        {
            return this._SystemExceptions.Count > 0;
        }
        /// <summary>
        /// Return number of nonfatal exceptions in this container.
        /// </summary>
        /// <returns></returns>
        public int NonFatalCount()
        {
            if (!this._CustomExceptions.ContainsKey(typeof(NonFatals.NonFatal)))
            {
                return 0;
            }
            return this._CustomExceptions[typeof(NonFatals.NonFatal)].Count;
        }
        /// <summary>
        /// Return number of semi fatal exceptions in this container.
        /// </summary>
        /// <returns></returns>
        public int SemiFatalCount()
        {
            if (!this._CustomExceptions.ContainsKey(typeof(SemiFatals.SemiFatal)))
            {
                return 0;
            }
            return this._CustomExceptions[typeof(SemiFatals.SemiFatal)].Count;
        }
        #endregion
        #region Mutators
        /// <summary>
        /// Append new exception to the exception container. 
        /// </summary>
        /// <param name="exception"></param>
        public void Append(Exception exception)
        {
            ((IMultipleElements)this).Merge(exception);
        }
        /// <summary>
        /// Clear out all exceptions. 
        /// </summary>
        public void ClearContents()
        {
            this._CustomExceptions.Clear();
            this._SystemExceptions.Clear();
        }
        /// <summary>
        /// Enable calling IMultipleElements::Merge() without upcasting.
        /// </summary>
        /// <param name="ex"></param>
        public void Merge(Exception ex)
        {
            ((IMultipleElements)this).Merge(ex);
        }
        #endregion
        #region Class Methods
        /// <summary>
        /// Return single concise message using all the stored exceptions in this object.
        /// </summary>
        /// <returns></returns>
        public override string ConciseMessage()
        {
            StringBuilder message = new StringBuilder();
            // Exit if no nonfatal/semifatal/system exceptions occurred:
            if (!(this.HasErrors(ExceptionType.NonFatal) || this.HasErrors(ExceptionType.SemiFatal) || this.HasErrors(ExceptionType.System)))
            {
                return message.ToString();
            }
            if (!String.IsNullOrEmpty(this.Application))
            {
                message.AppendLine(new String('-', 70));
                message.AppendLine(this.Application + " Error:");
                message.AppendLine(new String('-', 70));
            }
            foreach (var key in this._CustomExceptions.Keys)
            {
                // Skip fatal error message. To return fatal error message, use FatalErrorMessage():
                if (!typeof(Fatals.Fatal).IsAssignableFrom(key))
                {
                    if (this._CustomExceptions[key].Count > 0)
                    {
                        message.AppendLine(ExceptionMessageHeader(key));
                        message.AppendLine("{");
                        // Append all of the concise error messages for each loaded exception:
                        foreach (var pair in this._CustomExceptions[key])
                        {
                            var concise = pair.Value.ConciseMessage();
                            concise = concise.Replace("\n", String.Empty).Replace("\t", String.Empty);
                            var split = concise.Split(',');
                            if (split.Length > 0)
                            {
                                foreach(var msg in split)
                                {
                                    message.AppendLine("\t" + msg);
                                }
                            }
                            else
                            {
                                message.AppendLine("\t" + concise);
                            }
                            
                        }
                        message.AppendLine("}");
                    }
                }
            }
            if (this._SystemExceptions.Count > 0)
            {
                message.AppendLine(String.Format("{0} system exceptions occurred.", this._SystemExceptions.Count));
            }
            message.AppendLine("Please see log file for more details.");
            // Reformat the message to fit within message box:
            return message.ToString();
        }
        /// <summary>
        /// Return fatal error message to caller.
        /// </summary>
        /// <returns></returns>
        public string FatalErrorMessage()
        {
            StringBuilder message = new StringBuilder();
            if (this.HasErrors(ExceptionType.Fatal))
            {
                if (!String.IsNullOrWhiteSpace(this.Application))
                {
                    message.AppendLine(new String('-', 50));
                    message.AppendLine(this.Application + " Error:");
                    message.AppendLine(new String('-', 50));
                }
                message.AppendLine(ExceptionMessageHeader(typeof(Fatals.Fatal)));
                message.AppendLine("{");
                foreach (var pair in this._CustomExceptions[typeof(Fatals.Fatal)])
                {
                    message.AppendLine("\t" + pair.Value.ConciseMessage());
                }
                message.AppendLine("}");
                message.Append("Exiting application.");
            }
            // Resize message so looks acceptable in message box:
            return message.ToString();
        }
        /// <summary>
        /// Print relevant exception messages if occurred. If fatal exceptions occurred then exit the application after generating log file and printing messages.
        /// </summary>
        public void HandleAll()
        {
            this.HandleNonFatals();
            this.HandleFatals();
        }
        /// <summary>
        /// Print fatal exception message to screen, generate log file and exit application if fatal exceptions occurred.
        /// </summary>
        public void HandleFatals()
        {
            if (this.HasFatals())
            {
                System.Windows.Forms.MessageBox.Show(this.FatalErrorMessage());
                // Generate log file:
                this.OutputLogFile();
                System.Environment.Exit(1);
            }
        }
        /// <summary>
        /// Print non-fatal + semi-fatal exception message to screen if occurred.
        /// </summary>
        public void HandleNonFatals()
        {
            if (this.HasSemiFatals() || this.HasNonFatals())
            {
                System.Windows.Forms.MessageBox.Show(this.ConciseMessage());
            }
        }
        /// <summary>
        /// This method does not get used. Use OutputLogFile() instead to output granular messages.
        /// </summary>
        /// <returns></returns>
        public override string Message()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Generate log file on file system using all exceptions stored in this object, to default path. 
        /// </summary>
        public void OutputLogFile(string logPath = "")
        {
            var InOrder = new SortedList<DateTime, Exception>();
            FileTypes.Files.LogFile logFile;

            var attributes = new Utilities.ApplicationAttributes(DateTime.Now, false, DateTime.Now, Utilities.RunTime.AM, "");
            if (!String.IsNullOrWhiteSpace(logPath))
            {
                logFile = new FileTypes.Files.LogFile(logPath, attributes);
            }
            else
            {
                logFile = new FileTypes.Files.LogFile(attributes);
            }
            // Merge all types into single list such that they are printed in chronological order:
            foreach (var key in this._CustomExceptions.Keys)
            {
                foreach (var pair in this._CustomExceptions[key])
                {
                    var currTimeStamp = pair.Value.TimeStamp;
                    while (InOrder.ContainsKey(currTimeStamp))
                    {
                        currTimeStamp = currTimeStamp.AddMilliseconds(1);
                    }
                    InOrder.Add(currTimeStamp, pair.Value);
                }
            }
            foreach (var pair in this._SystemExceptions)
            {
                var currTimeStamp = pair.Key;
                while (InOrder.ContainsKey(currTimeStamp))
                {
                    currTimeStamp = currTimeStamp.AddMilliseconds(1);
                }
                InOrder.Add(currTimeStamp, pair.Value);
            }
            // Add all exceptions into the log file:
            foreach (var pair in InOrder)
            {
                logFile.AppendException(pair.Value, pair.Key);
            }
            // Output log file to file system:
            logFile.WriteContents();
        }
        #endregion
        #region Private Helpers
        /// <summary>
        /// Merge all custom exceptions in passed aggregator with this object.
        /// </summary>
        /// <param name="agg"></param>
        private void MergeAllCustomExcepts(ExceptionAggregator agg)
        {
            foreach (var baseType in agg._CustomExceptions.Keys)
            {
                if (!this._CustomExceptions.ContainsKey(baseType))
                {
                    this._CustomExceptions.Add(baseType, new Dictionary<Type, GenericException>());
                }
                foreach (var exceptPair in agg._CustomExceptions[baseType])
                {
                    // Create new exception key if does not exist yet:
                    if (exceptPair.Value is GenericException)
                    {
                        if (!this._CustomExceptions[baseType].ContainsKey(exceptPair.Key))
                        {
                            // Create new element in dictionary and set exception as value:
                            this._CustomExceptions[baseType].Add(exceptPair.Key, exceptPair.Value);
                        }
                        else if (exceptPair.Value is IMultipleElements)
                        {
                            ((IMultipleElements)this._CustomExceptions[baseType][exceptPair.Key]).Merge(exceptPair.Value);
                        }
                        else
                        {
                            // Replace the non-container exception with passed one:
                            this._CustomExceptions[baseType][exceptPair.Key] = exceptPair.Value;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Merge all stored system exceptions in passed aggregator into this object.
        /// </summary>
        /// <param name="agg"></param>
        private void MergeAllSystemExcepts(ExceptionAggregator agg)
        {
            foreach (var pair in agg._SystemExceptions)
            {
                while (this._SystemExceptions.ContainsKey(pair.Key))
                {
                    pair.Key.AddMilliseconds(1);
                }
                this._SystemExceptions.Add(pair.Key, pair.Value);
            }
        }
        /// <summary>
        /// Merge passed system exception into the _SystemExceptions container. Prevent collision with other keys.
        /// </summary>
        /// <param name="sysExcept"></param>
        private void MergeSystemExcept(Exception sysExcept)
        {
            DateTime key = DateTime.Now;
            while (this._SystemExceptions.ContainsKey(key))
            {
                key = key.AddMilliseconds(1);
            }
            this._SystemExceptions.Add(key, sysExcept);
        }
        #endregion
        #region Interface Implementation
        /// <summary>
        /// Return the number of elements in the stored container. 
        /// </summary>
        /// <returns></returns>
        int IMultipleElements.ContainerCount()
        {
            int totalCount = 0;
            foreach (var pair in this._CustomExceptions)
            {
                if (pair.Value is IMultipleElements)
                {
                    totalCount += ((IMultipleElements)pair.Value).ContainerCount();
                }
                else
                {
                    totalCount++;
                }
            }
            return totalCount + this._SystemExceptions.Keys.Count;
        }
        /// <summary>
        /// Indicate if exception of passed type enumeration is present in the exception list. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasErrors(ExceptionType type)
        {
            switch (type)
            {
                case ExceptionType.Fatal:
                    return this._CustomExceptions.ContainsKey(typeof(Fatals.Fatal)) && this._CustomExceptions[typeof(Fatals.Fatal)].Count > 0;
                case ExceptionType.NonFatal:
                    return this._CustomExceptions.ContainsKey(typeof(NonFatals.NonFatal)) && this._CustomExceptions[typeof(NonFatals.NonFatal)].Count > 0;
                case ExceptionType.SemiFatal:
                    return this._CustomExceptions.ContainsKey(typeof(SemiFatals.SemiFatal)) && this._CustomExceptions[typeof(SemiFatals.SemiFatal)].Count > 0;
                case ExceptionType.System:
                    return this._SystemExceptions.Count > 0;
                default:
                    throw new NonFatals.GenericValueErrors("The passed type is not derived from GenericException.", "ExceptionAggregator::HasErrors()");
            }
        }
        /// <summary>
        /// Merge passed Exception with this object, depending upon type.
        /// </summary>
        /// <param name="except"></param>
        void IMultipleElements.Merge(Exception except)
        {
            // Add new dictionary if have not observed passed type yet:
            if (IsCustomType(except) && !this._CustomExceptions.ContainsKey(GetCustomType(except)) && !(except is ExceptionAggregator))
            {
                this._CustomExceptions.Add(GetCustomType(except), new Dictionary<Type, GenericException>());
            }
            if (except is ExceptionAggregator)
            {
                // Merge passed ExceptionAggregator with this object:
                this.MergeAllCustomExcepts((ExceptionAggregator)except);
                this.MergeAllSystemExcepts((ExceptionAggregator)except);
            }
            else if (except is IMultipleElements && ((IMultipleElements)except).ContainerCount() > 0)
            {
                // Add container exception to aggregator:
                if (!this._CustomExceptions[GetCustomType(except)].ContainsKey(except.GetType()))
                {
                    this._CustomExceptions[GetCustomType(except)].Add(except.GetType(), (GenericException)except);
                }
                else
                {
                    // Merge all elements in container exception into single object:
                    ((IMultipleElements)this._CustomExceptions[GetCustomType(except)][except.GetType()]).Merge(except);
                }
            }
            else if (IsCustomType(except) && !this._CustomExceptions[GetCustomType(except)].ContainsKey(except.GetType()))
            {
                // Add to the container for derived GenericExceptions:
                this._CustomExceptions[GetCustomType(except)].Add(except.GetType(), (GenericException)except);
            }
            else if (IsCustomType(except))
            {
                // Replace existing non-container exception with passed exception:
                this._CustomExceptions[GetCustomType(except)][except.GetType()] = (GenericException)except;
            }
            else
            {
                // Add system unhandled exception to container:
                this.MergeSystemExcept(except);
            }
        }
        /// <summary>
        /// Return all elements of container as log file rows.
        /// </summary>
        /// <returns></returns>
        List<LogFileRow> IMultipleElements.ToLogFileRows()
        {
            uint row = 0;
            var output = new List<LogFileRow>();

            foreach (var elem in this._CustomExceptions.Keys)
            {
                foreach (var except in this._CustomExceptions[elem])
                {
                    if (except.Value is IMultipleElements)
                    {
                        output.AddRange(((IMultipleElements)except.Value).ToLogFileRows());
                    }
                    else
                    {
                        output.Add(new LogFileRow(except.Value.Message(), row++, except.Value.TimeStamp, except.Value.CallingFunction));
                    }
                }
            }

            return output;
        }
        #endregion
        #region Static Methods
        /// <summary>
        /// Get the message header necessary for printing exception message to user at run time. 
        /// </summary>
        /// <param name="exceptionType"></param>
        /// <returns></returns>
        private static string ExceptionMessageHeader(Type exceptionType)
        {
            if (typeof(Fatals.Fatal).IsAssignableFrom(exceptionType))
            {
                return "The application will close due to the following reasons:";
            }
            else if (typeof(NonFatals.NonFatal).IsAssignableFrom(exceptionType))
            {
                return "The following nonfatal exceptions occurred at runtime:";
            }
            else if (typeof(SemiFatals.SemiFatal).IsAssignableFrom(exceptionType))
            {
                return "The following exceptions stopped the main operation:";
            }
            else
            {
                throw new Exceptions.NonFatals.GenericValueErrors("Passed type is not derived from GenericException.", "ExceptionAggregator::ExceptionMessageHeader()");
            }
        }
        /// <summary>
        /// Return derived GenericException, or null if is not a custom type.  
        /// </summary>
        /// <param name="except"></param>
        /// <returns></returns>
        public static Type GetCustomType(Exception except)
        {
            if (except is NonFatals.NonFatal)
            {
                return typeof(NonFatals.NonFatal);
            }
            if (except is Fatals.Fatal)
            {
                return typeof(Fatals.Fatal);
            }
            if (except is SemiFatals.SemiFatal)
            {
                return typeof(SemiFatals.SemiFatal);
            }
            return null;
        }
        /// <summary>
        /// Determine if passed exception is derived from GenericException. 
        /// </summary>
        /// <param name="except"></param>
        /// <returns></returns>
        public static bool IsCustomType(Exception except)
        {
            if (except is NonFatals.NonFatal)
            {
                return true;
            }
            if (except is Fatals.Fatal)
            {
                return true;
            }
            if (except is SemiFatals.SemiFatal)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Split up long messages so easily readable in message box. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string ResizeMessage(string message)
        {
            //// TODO: Experiment with lengths in message box.
            StringBuilder output = new StringBuilder();
            string remainingMessage = message;
            for (int index = 0; index < message.Length; index += MessageBufferLength)
            {
                if (index + MessageBufferLength >= message.Length)
                {
                    output.AppendLine(remainingMessage);
                }
                else
                {
                    output.AppendLine(message.Substring(index, MessageBufferLength));
                    remainingMessage = remainingMessage.Substring(MessageBufferLength, remainingMessage.Length - (MessageBufferLength));
                }
            }
            return output.ToString();
        }
        #endregion
    }
    #region ExceptionAggregator Enums
    public enum ExceptionType : int
    {
        Fatal = 0,
        SemiFatal = 1,
        NonFatal = 2,
        System = 3
    }
    #endregion
}
