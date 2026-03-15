namespace TechBlogApi.Helpers
{
    public class ApiResult<T>
    {
        public bool Status { get; set; }
        public T? Data { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public ApiResult(bool status, T data)
        {
            Status = status;
            Data = data;
        }

        public ApiResult(bool status, T data, int totalCount, int page = 1, int pageSize = 10)
        {
            Status = status;
            Data = data;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }
    public class ApiResult
    {
        public ApiResult(bool status, string message)
        {
            Status = status;
            Message = message;
        }

        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}