using MongoDB.Driver;
using UnknownCampServer.Core.Entities;
using UnknownCampServer.Core.Repositories;
using UnknownCampServer.Infrastructure.Data;

namespace UnknownCampServer.Infrastructure.Repositories
{
    public class TreasureRepository : ITreasureRepository
    {
        private readonly IMongoCollection<Treasure> _treasures;

        public TreasureRepository(MongoDbService mongoDbService)
        {
            _treasures = mongoDbService.Database.GetCollection<Treasure>("treasures");
        }

        public async Task<Treasure> GetTreasureByIdAsync(string id)
        {
            return await _treasures.Find(t => t.Id == id).FirstOrDefaultAsync();
        }
        public async Task CreateTreasureAsync(Treasure treasure)
        {
            await _treasures.InsertOneAsync(treasure);
        }
    }
}
