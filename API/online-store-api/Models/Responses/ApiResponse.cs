namespace online_store_api.Models.Responses
{
    public class ApiResponse<T>
    {
        public string ResponseStatus { get; set; } = "success"; // success, failure, warning
        public string ResponseMessage { get; set; } = string.Empty;
        public T ResponseObject { get; set; }

        public ApiResponse() { }

        public ApiResponse(string status, string message, T responseObject)
        {
            ResponseStatus = status;
            ResponseMessage = message;
            ResponseObject = responseObject;
        }

        public static ApiResponse<T> Success(string message, T responseObject)
        {
            return new ApiResponse<T>
            {
                ResponseStatus = "success",
                ResponseMessage = message,
                ResponseObject = responseObject
            };
        }

        public static ApiResponse<T> Failure(string message)
        {
            return new ApiResponse<T>
            {
                ResponseStatus = "failure",
                ResponseMessage = message,
                ResponseObject = default(T)
            };
        }

        public static ApiResponse<T> Warning(string message, T responseObject)
        {
            return new ApiResponse<T>
            {
                ResponseStatus = "warning",
                ResponseMessage = message,
                ResponseObject = responseObject
            };
        }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse() { }

        public ApiResponse(string status, string message, object responseObject = null)
            : base(status, message, responseObject) { }
    }
}
