namespace FinanceManager.Extensions.Endpoints
{
    public interface IEndPoint
    {
        static abstract void Map(IEndpointRouteBuilder app);
    }
}
