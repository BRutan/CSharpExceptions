/* IMultipleElements.cs
Description:
    * Interface used to indicate that exception uses a container to store multiple issues that have occurred (for example, multiple Caliber IDs could not be found).
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpObjectLibrary.Exceptions
{
    /// <summary>
    /// Interface used to indicate that exception uses a container to store multiple issues that have occurred (for example, multiple Caliber IDs could not be found).
    /// </summary>
    public interface IMultipleElements 
    {
        int ContainerCount();
        void Merge(Exception except);
        List<FileTypes.Files.LogFileRow> ToLogFileRows();
    }
}
