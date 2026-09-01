using FinanceManager.Models;
using FinanceManager.Requests.Categories;
using FinanceManager.Requests.Transactions;
using FinanceManager.Responses;

namespace FinanceManager.Services
{
    public interface ITransactionService
    {
        Task<PagedResponse<List<Transaction>>> GetAllAsync(TransactionGetAllRequest request);
        Task<Response<Transaction>> GetByIdAsync(TransactionGetByIdRequest request);
        Task<Response<Transaction>> AddAsync(TransactionCreateRequest request);
        Task<Response<Transaction>> UpdateAsync(TransactionEditorRequest request);
        Task<Response<Transaction>> DeleteAsync(TransactionDeleteRequest request);
    }
}
