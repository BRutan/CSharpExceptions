# CSharpExceptions:

Library defines a (somewhat) exhaustive list of all runtime exceptions in sensible categories, with the opportunity for further additions.

# Classes:

•	ExceptionAggregator: Object stores derived Fatal, SemiFatal and NonFatal exceptions, using an derived Merge() function that works with all derived GenericExceptions (Fatal, SemiFatal and NonFatal), system exceptions as well as other ExceptionAggregators. Returns concise message fit for message boxes giving brief detail of all NonFatal and SemiFatal exceptions with the ConciseMessage() function. Also has ability to generate log file with GenerateLogFile() function. Implements accessors to indicate the number and occurrence of each exception type, and handlers to accommodate occurrence of specific derived exceptions. The following is a list of the abstract base and middle-tier abstract base classes that determine what occurs at run time.

# Exception Objects:

o Fatal: Exception that will stop the application. 

o	SemiFatal: Exception that stops a particular operation, but not the entire application.

o	NonFatal: Minor exceptions that will not stop program. Examples include GenericValueErrors, meant to detail programming mistakes (such as type conversion errors, out of bounds, etc).

All of the above are derived from GenericException. 

The relevant unit test is ExceptionTest.cs.
