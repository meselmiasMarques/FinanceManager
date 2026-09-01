using FinanceManager.Models;
using FinanceManager.Repositories;
using FinanceManager.Requests.Transactions;
using FinanceManager.Responses;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Services
{
    public class TransactionService(ITransactionRepository repository) : ITransactionService
    {
        public async Task<Response<Transaction>> AddAsync(TransactionCreateRequest request)
        {
            var transaction = new Transaction
            {
                Title = request.Title,
                Type = request.Type,
                Amount = request.Amount,
                CreatedAt = DateTime.UtcNow,
                UserId = 1,
                CategoryId = request.CategoryId
            };

            try
            {
                await repository.AddAsync(transaction);
                await repository.CommitAsync();
                return new Response<Transaction>
                {
                    Code = 200,
                    Data = transaction,
                    Message = "Transaction created successfully",
                };
            }
            //catch
            //{
            //    return new Response<Transaction>(null, 500, "An error occurred while creating the transaction");
            //}

            catch (DbUpdateException ex)
            {
                return new Response<Transaction>(null, 500, $"EXDB01: {ex.Message}");
            }
        }

        public async Task<Response<Transaction>> DeleteAsync(TransactionDeleteRequest request)
        {
            try
            {
                var transaction = await repository.GetByIdAsync(request.Id);
                if (transaction == null)
                {
                    return new Response<Transaction>(null, 404, "Transaction not found");
                }

                await repository.DeleteAsync(transaction);
                await repository.CommitAsync();
                return new Response<Transaction>
                {
                    Code = 200,
                    Data = transaction,
                    Message = "Transaction deleted successfully",
                };
            }
            catch
            {
                return new Response<Transaction>(null, 500, "An error occurred while deleting the transaction");
            }
        }

        public async Task<PagedResponse<List<Transaction>>> GetAllAsync(TransactionGetAllRequest request)
        {
            var query = await repository.GetAllAsync(request);

            var totalCount = await query.CountAsync();

            var result = await query.ToListAsync();

            return new PagedResponse<List<Transaction>>(result, request.PageNumber, request.PageSize, totalCount);
        }

        public async Task<Response<Transaction>> GetByIdAsync(TransactionGetByIdRequest request)
        {
            var result = await repository.GetByIdAsync(request.Id);
            if (result == null)
            {
                return new Response<Transaction>(null, 404, "Transação não encontrada");
            }

            return new Response<Transaction>(result, 200, "Transaction retrieved successfully");
        }

        public async Task<Response<Transaction>> UpdateAsync(TransactionEditorRequest request)
        {
            var transaction = await repository.GetByIdAsync(request.Id);

            if (transaction == null)
            {
                return new Response<Transaction>(null, 404, "Transaction not found");
            }

            transaction.Title = request.Title;
            transaction.Type = request.Type;
            transaction.Amount = request.Amount;
            transaction.CategoryId = request.CategoryId;

            try
            {
                await repository.UpdateAsync(transaction);
                await repository.CommitAsync();
                return new Response<Transaction>(transaction, 200, "Transaction updated successfully");
            }
            catch
            {
                return new Response<Transaction>(null, 500, "An error occurred while updating the transaction");
            }
        }
    }
}
