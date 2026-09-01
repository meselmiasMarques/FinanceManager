namespace FinanceManager.Requests
{
    public class PagedRequest : RequestBase
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
    }
}
