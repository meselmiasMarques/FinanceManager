namespace FinanceManager.Requests
{
    public class PagedRequest : RequestBase
    {
        public const int DefaultPageSize = 25;
        public const int MaxPageSize = 100;

        private int _pageNumber = 1;
        private int _pageSize = DefaultPageSize;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Clamp(value < 1 ? DefaultPageSize : value, 1, MaxPageSize);
        }
    }
}
