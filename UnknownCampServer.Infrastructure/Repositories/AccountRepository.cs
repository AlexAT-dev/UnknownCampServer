using MongoDB.Driver;
using UnknownCampServer.Core.Entities;
using UnknownCampServer.Core.Repositories;
using System.Threading.Tasks;
using UnknownCampServer.Infrastructure.Data;

namespace UnknownCampServer.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMongoCollection<Account> _accounts;

        public AccountRepository(MongoDbService mongoDbService)
        {
            _accounts = mongoDbService.Database.GetCollection<Account>("accounts");
        }

        public async Task CreateAccountAsync(Account account)
        {
            await _accounts.InsertOneAsync(account);
        }

        public async Task<List<Account>> GetAllVerifiedAccountsAsync()
        {
            return await _accounts.Find(a => a.VerifiedAt != null).ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(string id)
        {
            return await _accounts.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            return await _accounts.Find(a => a.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Account> GetAccountByEmailOrNameAsync(string email, string name)
        {
            return await _accounts.Find(a => a.Email == email || a.Name == name).FirstOrDefaultAsync();
        }

        public async Task<Account> GetAccountByTokenAsync(string token)
        {
            return await _accounts.Find(a => a.Token == token && a.VerifiedAt == null).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            var updateResult = await _accounts.ReplaceOneAsync(a => a.Id == account.Id, account);
            return updateResult.MatchedCount > 0;
        }
    }
}
