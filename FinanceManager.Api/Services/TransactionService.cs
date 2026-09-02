using FinanceManager.Extensions;
using FinanceManager.Models;
using FinanceManager.Repositories;
using FinanceManager.Requests.Transactions;
using FinanceManager.Responses;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Services
{
    public class TransactionService(
        ITransactionRepository repository,
        ICategoryRepository categoryRepository,
        IUserContext user,
        ILogger<TransactionService> logger) : ITransactionService
    {
        private async Task<bool> UserOwnsCategoryAsync(int categoryId)
            => await categoryRepository.GetByIdAsync(categoryId, user.UserId) is not null;

        public async Task<PagedResponse<List<Transaction>>> GetAllAsync(TransactionGetAllRequest request)
        {
            try
            {
                request.UserId = user.UserId;

                var query = repository.GetAll(request);
                var totalCount = await query.CountAsync();

                var data = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                return new PagedResponse<List<Transaction>>(data, request.PageNumber, request.PageSize, totalCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar transações");
                return new PagedResponse<List<Transaction>>(null, 500, "Ocorreu um erro ao buscar as transações.");
            }
        }

        public async Task<Response<Transaction>> GetByIdAsync(TransactionGetByIdRequest request)
        {
            try
            {
                var transaction = await repository.GetByIdAsync(request.Id, user.UserId);
                return transaction is null
                    ? new Response<Transaction>(null, 404, "Transação não encontrada.")
                    : new Response<Transaction>(transaction, 200, "Transação encontrada.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao buscar transação {TransactionId}", request.Id);
                return new Response<Transaction>(null, 500, "Ocorreu um erro ao buscar a transação.");
            }
        }

        public async Task<Response<Transaction>> AddAsync(TransactionCreateRequest request)
        {
            if (request.Amount <= 0)
                return new Response<Transaction>(null, 400, "O valor da transação deve ser maior que zero.");

            if (!await UserOwnsCategoryAsync(request.CategoryId))
                return new Response<Transaction>(null, 400, "Categoria inválida ou não pertence ao usuário.");

            try
            {
                var transaction = new Transaction
                {
                    Title = request.Title,
                    Type = request.Type,
                    Amount = request.Amount,
                    CategoryId = request.CategoryId,
                    UserId = user.UserId,
                    CreatedAt = DateTime.UtcNow,
                };

                await repository.AddAsync(transaction);
                await repository.SaveChangesAsync();

                return new Response<Transaction>(transaction, 201, "Transação criada com sucesso.");
            }
            catch (DbUpdateException ex)
            {
                logger.LogWarning(ex, "Falha ao criar transação (provável categoria inexistente)");
                return new Response<Transaction>(null, 400, "Não foi possível criar a transação. Verifique a categoria informada.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao criar transação");
                return new Response<Transaction>(null, 500, "Ocorreu um erro ao criar a transação.");
            }
        }

        public async Task<Response<Transaction>> UpdateAsync(TransactionEditorRequest request)
        {
            if (request.Amount <= 0)
                return new Response<Transaction>(null, 400, "O valor da transação deve ser maior que zero.");

            if (!await UserOwnsCategoryAsync(request.CategoryId))
                return new Response<Transaction>(null, 400, "Categoria inválida ou não pertence ao usuário.");

            try
            {
                var transaction = await repository.GetByIdAsync(request.Id, user.UserId);
                if (transaction is null)
                    return new Response<Transaction>(null, 404, "Transação não encontrada.");

                transaction.Title = request.Title;
                transaction.Type = request.Type;
                transaction.Amount = request.Amount;
                transaction.CategoryId = request.CategoryId;
                transaction.UpdatedAt = DateTime.UtcNow;

                repository.Update(transaction);
                await repository.SaveChangesAsync();

                return new Response<Transaction>(transaction, 200, "Transação atualizada com sucesso.");
            }
            catch (DbUpdateException ex)
            {
                logger.LogWarning(ex, "Falha ao atualizar transação {TransactionId}", request.Id);
                return new Response<Transaction>(null, 400, "Não foi possível atualizar a transação. Verifique a categoria informada.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao atualizar transação {TransactionId}", request.Id);
                return new Response<Transaction>(null, 500, "Ocorreu um erro ao atualizar a transação.");
            }
        }

        public async Task<Response<Transaction>> DeleteAsync(TransactionDeleteRequest request)
        {
            try
            {
                var transaction = await repository.GetByIdAsync(request.Id, user.UserId);
                if (transaction is null)
                    return new Response<Transaction>(null, 404, "Transação não encontrada.");

                repository.Delete(transaction);
                await repository.SaveChangesAsync();

                return new Response<Transaction>(transaction, 200, "Transação deletada com sucesso.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao deletar transação {TransactionId}", request.Id);
                return new Response<Transaction>(null, 500, "Ocorreu um erro ao deletar a transação.");
            }
        }
    }
}
