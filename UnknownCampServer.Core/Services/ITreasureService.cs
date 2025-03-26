using UnknownCampServer.Core.Entities;

namespace UnknownCampServer.Core.Services
{
    public interface ITreasureService
    {
        Task<TreasureResult> UnlockTreasureAsync(string id, TreasureOpening treasureOpening);
        Task AddMockTreasureAsync();
    }
}
