using System.Text.Json.Serialization;

namespace FinanceManager.Responses
{
    public class PagedResponse<T> : Response<T> where T : class
    {
        [JsonConstructor]
        public PagedResponse(T data, 
            int currentPage, 
            int pageSize, 
            int totalCount) 
                : base(data)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public PagedResponse(T data, int code = 200, string message = null) 
            : base(data, code, message)
        {
            
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
