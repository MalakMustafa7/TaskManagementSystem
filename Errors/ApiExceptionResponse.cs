namespace TeamTaskManagement.Api.Errors
{
    public class ApiExceptionResponse : ApiResponse
    {
        public string? Details { get; set; }
        public ApiExceptionResponse(int statuscode, string? messege = null, string? details = null) : base(statuscode, messege)
        {
            Details = details;
        }
    }
}
