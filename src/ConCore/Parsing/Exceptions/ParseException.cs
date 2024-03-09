using System;
using System.Collections.Generic;
using System.Text;

namespace ConCore.Parsing.Exceptions
{
    public enum ParseExceptionType
    {
        ExpectedNext,
    }

    public class ParseException: Exception
    {
        public ParseExceptionType ExceptionType;

        public ParseException(string? message, ParseException? innerException, ParseExceptionType exceptionType)
            : base(message, innerException)
        {
            ExceptionType = exceptionType;
        }
    }
}
