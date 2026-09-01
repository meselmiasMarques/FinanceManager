using FinanceManager.Requests.Transactions;
using FinanceManager.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FinanceManager.Extensions.Endpoints
{
    public class TransactionEndPoints : IEndPoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {

            app.MapGet("/transactions", async (
                ITransactionService Service,
                int skip = 1,
                int take = 10) =>
            {
                var request = new TransactionGetAllRequest
                {
                    PageNumber = skip,
                    PageSize = take,
                    UserId = 1
                };
                var result = await Service.GetAllAsync(request);
                return result.Code == 200 ?
                Results.Ok(result)
                : Results.BadRequest(result);
            })
                 .WithDescription("Lista as transações")
                 .WithTags("Transações")
                 .WithOrder(1);

            app.MapGet("/transactions/{id}", async (
                ITransactionService Service,
                int id) =>
            {
                var request = new TransactionGetByIdRequest
                {
                    Id = id,
                    UserId = 1 //Identity.User
                };
                var result = await Service.GetByIdAsync(request);
                return result.Code == 200 ?
                Results.Ok(result)
                : Results.BadRequest(result);
            })
                 .WithDescription("Busca uma transação pelo ID")
                 .WithTags("Transações")
                 .WithOrder(2);

            app.MapPost("/transactions", async (
                ITransactionService Service,
                TransactionCreateRequest request) =>
            {



                var result = await Service.AddAsync(request);
                return result.Code == 200 ?
                Results.Created($"/transactions/{result.Data.Id}", result)
                : Results.BadRequest(result);
            })
                 .WithDescription("Cria uma nova transação")
                 .WithTags("Transações")
                 .WithOrder(3);

            app.MapPut("/transactions", async (
                ITransactionService Service,
                TransactionEditorRequest request) =>
            {
                var result = await Service.UpdateAsync(request);
                return result.Code == 200 ?
                Results.Ok(result)
                : Results.BadRequest(result);
            })
                 .WithDescription("Atualiza uma transação existente")
                 .WithTags("Transações")
                 .WithOrder(4);

            app.MapDelete("/transactions/{id}", async (
                ITransactionService Service,
                int id) =>
            {
                var request = new TransactionDeleteRequest
                {
                    Id = id,
                    UserId = 1 //Identity.User
                };
                var result = await Service.DeleteAsync(request);
                return result.Code == 200 ?
                Results.Ok(result)
                : Results.BadRequest(result);
            })
                 .WithDescription("Deleta uma transação existente")
                 .WithTags("Transações")
                 .WithOrder(5);
        }
    }
}

