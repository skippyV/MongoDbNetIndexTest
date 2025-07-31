using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbTesting.Utils;

namespace MongoDbTesting.Data
{
    public class OpovEvent
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string[] AllowedUsers { get; set; } = [];

        public List<Contest> Contests { get; set; } = [];

        private readonly string eventName;

        public OpovEvent()
        {
            Console.WriteLine("Is the Contests List null at this point?");
        }
        public required string EventName
        {
            get => eventName;
            init
            {
                eventName = StringStuff.RemoveWhitespacesUsingStringBuilder(value);
            }
        }

        public void AddContest(Contest contest)
        {
            if (Contests is null)
            {
                Contests = new List<Contest>();
            }
            Contests.Add(contest);
        }
        public Contest? GetContest(string id)
        {
            return Contests.FirstOrDefault(p => p.Id.Equals(id, StringComparison.Ordinal));
        }
    }
}
