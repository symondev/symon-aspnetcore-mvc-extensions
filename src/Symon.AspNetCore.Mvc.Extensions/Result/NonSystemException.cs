using System;

namespace Symon.AspNetCore.Mvc.Extensions.Result
{
    public class NonSystemException : System.Exception
    {
        public NonSystemException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public NonSystemException(int errorCode, string message, System.Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; set; }
    }
}
