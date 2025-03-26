using UnknownCampServer.Core.Entities;

namespace UnknownCampServer.Core.Repositories
{
    public interface IAccountRepository
    {
        Task CreateAccountAsync(Account account);
        Task<List<Account>> GetAllVerifiedAccountsAsync();
        Task<Account> GetAccountByIdAsync(string id);
        Task<Account> GetAccountByEmailAsync(string email);
        Task<Account> GetAccountByEmailOrNameAsync(string email, string name);
        Task<Account> GetAccountByTokenAsync(string token);
        Task<bool> UpdateAccountAsync(Account account);

    }
}
