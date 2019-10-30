/* ExceptionTest.cs
Description:
    * Test all exceptions in the CSharpObjectLibrary.Exceptions namespace.
 */

using FatalExceptions = CSharpObjectLibrary.Exceptions.Fatals;
using NonFatalExceptions = CSharpObjectLibrary.Exceptions.NonFatals;
using SemiFatalExceptions = CSharpObjectLibrary.Exceptions.SemiFatals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    class ExceptionTest
    {
        static void Main(string[] args)
        {
            var agg = new CSharpObjectLibrary.Exceptions.ExceptionAggregator("ExceptionTest");

            ////////////////////////////////
            // Test Fatal exceptions:
            ////////////////////////////////
            agg.Append(new FatalExceptions.FileFormatIssues("Test1.xlsx", @"C:\Nowhere\Test1.xlsx", "Missing two columns.", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.FileFormatIssues("Test1.xlsx", @"C:\Nowhere\Test1.xlsx", "Missing two columns.", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.FileFormatIssues("Test2.xlsx", @"C:\Nowhere2\Test2.xlsx", "Missing three columns.", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.FilesInUse("DataFile1.csv", @"C:\Nowhere\DataFile1.csv", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.FilesInUse("DataFile1.csv", @"C:\Nowhere\DataFile1.csv", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.FilesInUse("DataFile2.csv", @"C:\Nowhere2\DataFile2.csv", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.MissingConfigFiles("DataFile2.csv", @"C:\Nowhere2\DataFile2.csv", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.MissingConfigFiles("DataFile2.csv", @"C:\Nowhere2\DataFile2.csv", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.MissingConfigFiles("DataFile3.csv", @"C:\Nowhere2\DataFile3.csv", "Nowhere::Test()", "No columns in file."));
            agg.Append(new FatalExceptions.MissingFolders(@"C:\Nowhere2\MissingFolder\", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.MissingFolders(@"C:\Nowhere2\MissingFolder\", "Nowhere::Test()"));
            agg.Append(new FatalExceptions.MissingFolders(@"C:\Nowhere2\MissingFolder2\", "Nowhere::Test()"));

            ////////////////////////////////
            // Test SemiFatal exceptions:
            ////////////////////////////////
            agg.Append(new SemiFatalExceptions.FailedToCreateSheets("TestSheet", "Nowhere::Test()", "TestDoc", "TestReason"));
            agg.Append(new SemiFatalExceptions.FailedToCreateSheets("TestSheet", "Nowhere::Test()", "TestDoc", "TestReason"));
            agg.Append(new SemiFatalExceptions.FailedToCreateSheets("TestSheet2", "Nowhere::Test()", "TestDoc", "TestReason2"));
            agg.Append(new SemiFatalExceptions.FailedToGenerateFiles("TestWorkbook.xlsx", @"C:\Nowhere\TestWorkbook.xlsx", "Nowhere::Test()", "Folder does not exist."));
            agg.Append(new SemiFatalExceptions.FailedToGenerateFiles("TestWorkbook2.xlsx", @"C:\Nowhere\TestWorkbook2.xlsx", "Nowhere::Test()", "File has invalid extension."));
            agg.Append(new SemiFatalExceptions.FailedToGenerateReports("TestReport", "Nowhere::Test()", "TestDoc"));
            agg.Append(new SemiFatalExceptions.FailedToGenerateReports("TestReport", "Nowhere::Test()", "TestDoc"));
            agg.Append(new SemiFatalExceptions.FailedToGenerateReports("TestReport2", "Nowhere::Test()", "TestDoc2"));
            agg.Append(new SemiFatalExceptions.FileFormatIssues("Caliber1.txt", @"C:\Nowhere\Caliber1.txt", "Missing rows.", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.FileFormatIssues("Caliber1.txt", @"C:\Nowhere\Caliber1.txt", "Missing rows.", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.FileFormatIssues("Caliber2.txt", @"C:\Nowhere2\Caliber2.txt", "Missing columns.", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.FilesInUse("Caliber1.txt", @"C:\Nowhere\Caliber1.txt", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.FilesInUse("Caliber1.txt", @"C:\Nowhere\Caliber1.txt", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.FilesInUse("Caliber2.txt", @"C:\Nowhere2\Caliber2.txt", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.FilesInUse("Caliber3.txt", @"C:\Nowhere3\Caliber3.txt", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.MissingFolders(@"C:\Nowhere2\Missingfolder1\", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.MissingFolders(@"C:\Nowhere2\Missingfolder1\", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.MissingFolders(@"C:\Nowhere2\Missingfolder2\", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.MissingFolders(@"C:\Nowhere2\Missingfolder2\", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.MissingInputFiles("Caliber1.txt", @"C:\Nowhere\Caliber1.txt", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.MissingInputFiles("Caliber1.txt", @"C:\Nowhere\Caliber1.txt", "Nowhere::Test()"));
            agg.Append(new SemiFatalExceptions.MissingInputFiles("Caliber2.txt", @"C:\Nowhere2\Caliber2.txt", "Nowhere::Test()"));

            ////////////////////////////////
            // Test NonFatal exceptions:
            ////////////////////////////////
            agg.Append(new NonFatalExceptions.FileFormatIssues("Test.xlsx", @"C:\Nowhere\Test.xlsx", "Missing three columns.", "Nowhere::Test()"));
            agg.Append(new NonFatalExceptions.FileFormatIssues("Test.xlsx", @"C:\Nowhere2\Test2.xlsx", "Missing three columns.", "Nowhere::Test()"));
            agg.Append(new NonFatalExceptions.FileFormatIssues("Test1.xlsx", @"C:\Nowhere2\Test2.xlsx", "Missing three columns.", "Nowhere::Test()"));
            agg.Append(new NonFatalExceptions.FilesInUse("Test.xlsx", @"C:\Nowhere\Test.xlsx", "Nowhere::Test()"));
            agg.Append(new NonFatalExceptions.FilesInUse("Test.xlsx", @"C:\Nowhere2\Test2.xlsx", "Nowhere::Test()"));
            agg.Append(new NonFatalExceptions.FilesInUse("Test1.xlsx", @"C:\Nowhere2\Test2.xlsx", "Nowhere::Test()"));
            agg.Append(new NonFatalExceptions.GenericValueErrors("Cannot convert date.","Nowhere::Test()"));
            agg.Append(new NonFatalExceptions.GenericValueErrors("Cannot overwrite value.", "Nowhere::Test()"));
            agg.Append(new NonFatalExceptions.MissingCalibers("TestCaliber", "Nowhere::Test()", "TestFile"));
            agg.Append(new NonFatalExceptions.MissingCalibers("TestCaliber", "Nowhere::Test()", "TestFile2"));
            agg.Append(new NonFatalExceptions.MissingCalibers("TestCaliber1", "Nowhere::Test()", "TestFile2"));
            agg.Append(new NonFatalExceptions.MissingCurves("TestCurve", "Nowhere::Test()", "TestProdFolder", DateTime.Now));
            agg.Append(new NonFatalExceptions.MissingCurves("TestCurve", "Nowhere::Test()", "TestProdFolder", DateTime.Now));
            agg.Append(new NonFatalExceptions.MissingCurves("TestCurve1", "Nowhere::Test()", "TestProdFolder", DateTime.Now));
            agg.Append(new NonFatalExceptions.MissingInflationIndices("TestIndex","Nowhere::Test()", "TestFile", DateTime.Now));
            agg.Append(new NonFatalExceptions.MissingInflationIndices("TestIndex", "Nowhere::Test()", "TestFile", DateTime.Now));
            agg.Append(new NonFatalExceptions.MissingInflationIndices("TestIndex1", "Nowhere::Test()", "TestFile", DateTime.Now));

            ////////////////////////////////
            // Test system exceptions:
            ////////////////////////////////
            agg.Append(new KeyNotFoundException("Could not find key."));
            agg.Append(new TypeAccessException("Cannot access type."));
            agg.Append(new TypeLoadException("Could not load type."));
            agg.Append(new ArrayTypeMismatchException("Array type is mismatched."));
            
            ////////////////////////////////
            // Test ExceptionAggregator:
            ////////////////////////////////
            var agg2 = new CSharpObjectLibrary.Exceptions.ExceptionAggregator("ExceptionTest");
            agg2.Merge(agg);

            // Display concise message to user:
            if (!String.IsNullOrWhiteSpace(agg2.ConciseMessage()))
            {
                System.Windows.Forms.MessageBox.Show(agg2.ConciseMessage());
            }
            // Generate log file:
            agg2.OutputLogFile();

            // Display fatal error message and exit application 
            agg2.HandleFatals();

        }
    }
}
