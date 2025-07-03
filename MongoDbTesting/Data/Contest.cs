using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbTesting.Utils;

namespace MongoDbTesting.Data
{
    public class Contest
    {
        // I can probably use init here somehow
        // https://www.youtube.com/watch?v=Z8urV5AullQ

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        private readonly string name;
        public required string  Name 
        { 
            get => name; 
            init
            {
                name = StringStuff.RemoveWhitespacesUsingStringBuilder(value);
            }  
        }
        public string Title { get; set; } = "";
        public List<string>? ContestAdmins { get; set; }




    }
    
}
