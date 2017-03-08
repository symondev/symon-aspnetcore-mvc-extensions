namespace Symon.AspNetCore.Mvc.Extensions.Result
{
    public class ApiResult
    {
        public ApiResult()
        {
            Success = true;
        }

        public ApiResult(object value)
        {
            Success = true;
            Value = value;
        }

        public ApiResult(int errorCode, string errorMessage)
        {
            Success = false;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; set; }

        public object Value { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
