using FinanceManager.Requests;
using FinanceManager.Requests.Transactions;
using FinanceManager.Services;

namespace FinanceManager.Extensions.Endpoints
{
    public class TransactionEndPoints : IEndPoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/transactions").WithTags("Transações");

            group.MapGet("/", async (
                ITransactionService service,
                int pageNumber = 1,
                int pageSize = PagedRequest.DefaultPageSize) =>
            {
                var result = await service.GetAllAsync(new TransactionGetAllRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                });
                return result.ToHttpResult();
            })
            .WithDescription("Lista as transações do usuário (paginado).");

            group.MapGet("/{id:int}", async (ITransactionService service, int id) =>
            {
                var result = await service.GetByIdAsync(new TransactionGetByIdRequest { Id = id });
                return result.ToHttpResult();
            })
            .WithDescription("Busca uma transação pelo ID.");

            group.MapPost("/", async (ITransactionService service, TransactionCreateRequest request) =>
            {
                var result = await service.AddAsync(request);
                return result.ToCreatedResult(transaction => $"/transactions/{transaction.Id}");
            })
            .WithDescription("Cria uma nova transação.");

            group.MapPut("/{id:int}", async (ITransactionService service, int id, TransactionEditorRequest request) =>
            {
                request.Id = id;
                var result = await service.UpdateAsync(request);
                return result.ToHttpResult();
            })
            .WithDescription("Atualiza uma transação existente.");

            group.MapDelete("/{id:int}", async (ITransactionService service, int id) =>
            {
                var result = await service.DeleteAsync(new TransactionDeleteRequest { Id = id });
                return result.ToHttpResult();
            })
            .WithDescription("Deleta uma transação existente.");
        }
    }
}
