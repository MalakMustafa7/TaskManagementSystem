
namespace TeamTaskManagement.Api.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        public ApiResponse(int statuscode,string? message=null)
        {
            StatusCode = statuscode;
            ErrorMessage =message ??GetDefaultErrorMessage(StatusCode);
        }

        private string? GetDefaultErrorMessage(int? statuscode)
        {
            return statuscode switch
            {
                400 => "Bad Request",
                401 => "You Are Not Authorized",
                404 => "Resourse Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}
