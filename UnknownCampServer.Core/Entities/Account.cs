using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UnknownCampServer.Core.Entities
{
    public class Account
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public string Token { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? VerifiedAt { get; set; }

        public string DeviceCreation { get; set; }

        public List<AccountUnlockable> Unlockables { get; set; }

        public Currency Currency { get; set; }
        public List<TreasureOpening> TreasureOpenings { get; set; }
    }

    public class AccountUnlockable
    {
        public string UnlockableId { get; set; }
        public DateTime DateObtained { get; set; }
    }

    public class Currency
    {
        public int Matches { get; set; }
        public int Ashes { get; set; }
        public int Boxes { get; set; }
    }

    public class TreasureOpening
    {
        public string TreasureID { get; set; }
        public int Opened { get; set; }

        public List<LastOpening> LastOpenings { get; set; }


        public class LastOpening
        {
            [BsonRepresentation(BsonType.String)]
            public UnlockableType Type { get; set; }
            public int LastAt { get; set; }
        }
    }
}
