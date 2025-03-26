using UnknownCampServer.Core.Entities;

namespace UnknownCampServer.Core.Repositories
{
    public interface ITreasureRepository
    {
        Task<Treasure> GetTreasureByIdAsync(string id);
        Task CreateTreasureAsync(Treasure treasure);
    }
}
