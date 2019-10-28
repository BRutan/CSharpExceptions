/* GenericException.cs
Description:
    * Base class exception for all applications that can be derived into Fatal, NonFatal and SemiFatal exceptions. Can be implemented in this library's LogFile class.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpObjectLibrary.Exceptions
{
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class GenericException : Exception
    {
        #region Class Members
        public string CallingFunction { get { return this.Info.CallingFunction; } }
        public DateTime TimeStamp { get { return this.Info.TimeStamp;  } }
        public ExceptionInfo Info;
        #endregion
        #region Constructors
        public GenericException()
        {
            this.Info = new ExceptionInfo();
        }
        public GenericException(GenericException except)
        {
            this.Info = except.Info;
        }
        public GenericException(string CallingFunction, DateTime TimeStamp, params string[] Messages)
        {
            this.Info = new ExceptionInfo(CallingFunction, TimeStamp, Messages);
        }
        public GenericException(ExceptionInfo info)
        {
            this.Info = info;
        }
        #endregion
        #region Class Methods
        public abstract new string Message();
        public abstract string ConciseMessage();
        public string TimeStampToString()
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////
            // Description:
            ///////////////////////////////////////////////////////////////////////////////////////////////
            // Return string version of Time Stamp.
            return this.Info.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss");
        }
        #endregion
    }
    public struct ExceptionInfo
    {
        public string CallingFunction;
        public List<string> Messages;
        public DateTime TimeStamp;
        public ExceptionInfo(string callingFunc, DateTime timeStamp, params string[] messages)
        {
            this.CallingFunction = callingFunc;
            this.TimeStamp = timeStamp;
            this.Messages = messages.ToList();
        }
    }
}