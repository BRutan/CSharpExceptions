# CSharpExceptions
•	ExceptionAggregator: Object stores derived Fatal, SemiFatal and NonFatal exceptions, using an derived Merge() function that works with all derived GenericExceptions (Fatal, SemiFatal and NonFatal), system exceptions as well as other ExceptionAggregators. Returns concise message fit for message boxes giving brief detail of all NonFatal and SemiFatal exceptions with the ConciseMessage() function. Also has ability to generate log file with GenerateLogFile() function. Implements accessors to indicate the number and occurrence of each exception type, and handlers to accommodate occurrence of specific derived exceptions. The following is a list of the abstract base and middle-tier abstract base classes that determine what occurs at run time.

o GenericException: Abstract base class for all derived custom exceptions. 

o Fatal: Exception that will break the application if stored in the ExceptionAggregator and triggered with HandleFatals(). Examples include MissingConfigFiles exception that indicates that a key application configuration file could not be found at run time.

o	SemiFatal: Exception that stops key application operation, but will not break it. Examples include FailedToGenerateReports, which indicates that a Data Manager report could not be completed due to one or more issues.

o	NonFatal: Minor exceptions. Examples include GenericValueErrors, meant to detail programming mistakes (such as type conversion errors, out of bounds, etc), and MissingCurves to detail Merlin Curves that could not be found in production or in a particular file. 

