using System.Collections.Generic;
using UnknownCampServer.Core.Entities;
using UnknownCampServer.Core.Repositories;
using UnknownCampServer.Core.Services;
using UnknownCampServer.Infrastructure.Repositories;

namespace UnknownCampServer.Infrastructure.Services
{
    public class TreasureService : ITreasureService
    {
        private readonly ITreasureRepository _treasureRepository;

        public TreasureService(ITreasureRepository treasureRepository)
        {
            _treasureRepository = treasureRepository;
        }

        public async Task<TreasureResult> UnlockTreasureAsync(string id, TreasureOpening treasureOpening)
        {
            Treasure treasure = await _treasureRepository.GetTreasureByIdAsync(id);

            return GetRandomUnlockable(treasure, treasureOpening);
        }

        public TreasureResult GetRandomUnlockable(Treasure treasure, TreasureOpening treasureOpening)
        {
            DistributeChances(treasure, treasureOpening);

            var selectedGroup = GetRandomUnlockablesGroup(treasure);

            var selectedUnlockable = GetRandomUnlockableFromGroup(selectedGroup);

            return new TreasureResult()
            {
                UnlockableId = selectedUnlockable.Id,
                Type = selectedGroup.Type,
            };
        }

        private void DistributeChances(Treasure treasure, TreasureOpening treasureOpening)
        {
            var totalChance = treasure.UnlockablesGroups
                .Where(group => group.Chance.HasValue)
                .Sum(group => group.Chance.Value);

            var nullCount = treasure.UnlockablesGroups.Count(group => !group.Chance.HasValue);

            if (nullCount > 0)
            {
                float remainingChance = 100 - totalChance;
                float nullChance = remainingChance / nullCount;

                foreach (var group in treasure.UnlockablesGroups.Where(group => !group.Chance.HasValue))
                {
                    group.Chance = nullChance;
                }
            }

            if (treasureOpening == null || treasureOpening.TreasureID != treasure.Id)
                return;

            foreach (var unlockablesGroup in treasure.UnlockablesGroups)
            {
                var lastOpening = treasureOpening.LastOpenings
                    .FirstOrDefault(lo => lo.Type == unlockablesGroup.Type);

                int lastAt = lastOpening?.LastAt ?? 0; 
                int guaranteed = unlockablesGroup.Guaranteed ?? 0; 

                if (guaranteed > 0 && treasureOpening.Opened - lastAt >= guaranteed)
                {
                    unlockablesGroup.Chance = 100;
                }
            }
        }

        private UnlockablesGroup GetRandomUnlockablesGroup(Treasure treasure)
        {
            float randomChance = (float)new Random().NextDouble() * 100;

            float cumulativeChance = 0;
            foreach (var group in treasure.UnlockablesGroups)
            {
                cumulativeChance += group.Chance ?? 0;

                if (randomChance <= cumulativeChance)
                {
                    return group;
                }
            }

            return treasure.UnlockablesGroups.Last();
        }

        private Unlockable GetRandomUnlockableFromGroup(UnlockablesGroup group)
        {
            float totalWeight = group.Unlockables.Sum(unlockable => unlockable.Weight);

            float randomWeight = (float)new Random().NextDouble() * totalWeight;

            float cumulativeWeight = 0;
            foreach (var unlockable in group.Unlockables)
            {
                cumulativeWeight += unlockable.Weight;

                if (randomWeight <= cumulativeWeight)
                {
                    return unlockable;
                }
            }

            return group.Unlockables.Last();
        }

        public async Task AddMockTreasureAsync()
        {
            var treasure = new Treasure
            {
                Name = "Flower Treasure",
                UnlockablesGroups = new List<UnlockablesGroup>
                {
                    new UnlockablesGroup
                    {
                        Type = UnlockableType.S,
                        Chance = 5.5f,
                        Guaranteed = 50,
                        Unlockables = new List<Unlockable>
                        {
                            new Unlockable { Id = "SkinS", Weight = 1f },
                        }
                    },
                    new UnlockablesGroup
                    {
                        Type = UnlockableType.A,
                        Chance = 7.25f,
                        Guaranteed = 20,
                        Unlockables = new List<Unlockable>
                        {
                            new Unlockable { Id = "SkinA_1", Weight = 1f },
                            new Unlockable { Id = "SkinA_2", Weight = 1f }
                        }
                    },
                    new UnlockablesGroup
                    {
                        Type = UnlockableType.B,
                        Chance = 30f,
                        Unlockables = new List<Unlockable>
                        {
                            new Unlockable { Id = "Skin6", Weight = 1f },
                            new Unlockable { Id = "Skin7", Weight = 1f }
                        }
                    },
                    new UnlockablesGroup
                    {
                        Type = UnlockableType.C,
                        Chance = 40f,
                        Unlockables = new List<Unlockable>
                        {
                            new Unlockable { Id = "Skin8", Weight = 1f },
                            new Unlockable { Id = "Skin9", Weight = 1f }
                        }
                    }
                }
            };

            await _treasureRepository.CreateTreasureAsync(treasure);

            Console.WriteLine("Mock treasure added successfully.");
        }
    }
}
