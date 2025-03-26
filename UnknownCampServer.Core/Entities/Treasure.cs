using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UnknownCampServer.Core.Entities
{
    public class Treasure
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }

        public List<UnlockablesGroup> UnlockablesGroups { get; set; }
    }

    public class UnlockablesGroup
    {
        [BsonRepresentation(BsonType.String)]
        public UnlockableType Type { get; set; }
        public float? Chance { get; set; }
        public int? Guaranteed { get; set; }
        public List<Unlockable> Unlockables { get; set; }
    }

    public class Unlockable
    {
        public string Id { get; set; }
        public float Weight { get; set; } = 1f;
    }

    public enum UnlockableType
    {
        S,
        A,
        B,
        C,
        D
    }

    public class TreasureResult
    {
        public string UnlockableId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public UnlockableType Type { get; set; }
        public int Ashes { get; set; }
    }
}
