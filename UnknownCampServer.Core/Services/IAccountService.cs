using UnknownCampServer.Core.Entities;
using UnknownCampServer.Core.DTOs;

namespace UnknownCampServer.Core.Services
{
    public interface IAccountService
    {
        Task<Account> GetAccount(string id);
        Task<List<string>> GetAllEmailsAsync();
        Task<bool> CreateAccountAsync(AccountRegDTO accountDTO);
        Task<bool> VerifyEmailAsync(string token);
        Task<Account> LoginAsync(AccountLoginDTO loginDto);

        Task<bool> AddUnlockableAsync(string accountId, string unlockable);
        Task<bool> BuyMatchBoxAsync(string accountId);
        Task<TreasureResult> OpenTreasureAsync(string accountId, string treasureId);
        Task<bool> AddMatchesAsync(string accountId, int matches);
    }
}
