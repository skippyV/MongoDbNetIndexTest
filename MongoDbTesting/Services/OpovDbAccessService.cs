using MongoDB.Driver;
using MongoDbTesting.Data;

// OpovEvent is a collection. It's type is 'collection'.
//
// Each OpovEvent Collection will reference the Contest Documents added to it.
// The Contest Documents are not maintained as a "Collection" within a Collection. 
// At least from the MongoDB perspective. 
// Reference: https://www.mongodb.com/community/forums/t/nested-collections/202832
//
// However, when one adds an Index to a TYPE of DOCUMENT that will be referenced by
// the collection, then those Documents within the collection will be indexed. 
//
// What confuses me about this model is this code:
//	 IMongoCollection<Contest> Contests = iMongoDatabase!.GetCollection<Contest>(collectionName);
//
// The GetCollection call returns "An implementation of a collection."  
// So the GetCollection() call kind of creates a pseudo-collection? Within the collection? 

namespace MongoDbTesting.Services
{
    public class OpovDbAccessService : IOpovDbAccessService
    {
        private MongoClient? mongoClient = null;
        private IMongoDatabase? iMongoDatabase = null;

        public OpovDbAccessService(MongoDbConfig config)
        {
            //mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");  // TODO - move this into config appsettings.json
            //iMongoDatabase = mongoClient.GetDatabase("AccessControl");
            mongoClient = new MongoClient(config.OpovDbConnectionString);
            iMongoDatabase = mongoClient.GetDatabase(config.OPOVDatabaseName);
        }

        public void AddIndexOnField(string collectionName, string fieldName)
        {
            IMongoCollection<Contest> Contests = iMongoDatabase!.GetCollection<Contest>(collectionName);

            System.Reflection.PropertyInfo? propertyInfo = typeof(Contest).GetProperty(fieldName);

            // I can't figure out how to use the propertyInfo in the lambda expression of IndexKeys.Ascending(xx)
            // So using AddIndexOnNameField() for now.

            var indexModel = new CreateIndexModel<Contest>(Builders<Contest>.IndexKeys.Ascending(c => c.Name));

            Contests.Indexes.CreateOne(indexModel);

        }

        public void AddIndexOnNameField(string collectionName)
        {
            IMongoCollection<Contest> Contests = iMongoDatabase!.GetCollection<Contest>(collectionName);
            var options = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<Contest>(Builders<Contest>.IndexKeys.Ascending(c => c.Name), options);

            Contests.Indexes.CreateOne(indexModel);
        }

        public string AddContest(Contest contest, string collectionName)
        {
            string message = "";
            try
            {
                // trying to add a Text Index on the Name field of Contest
                // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/indexes/

                IMongoCollection<Contest> Contests = iMongoDatabase!.GetCollection<Contest>(collectionName);

               // var indexModel = new CreateIndexModel<Contest>(Builders<Contest>.IndexKeys.Text(m => m.Name));
              //  Contests.Indexes.CreateOne(indexModel);


                Contests.InsertOne(contest);

                //Task<IAsyncCursor<Contest>> doesContestExist = Contests.FindAsync(c => c.Name.Equals(contest.Name));
                //doesContestExist.Wait();

                //IAsyncCursor<Contest> result = doesContestExist.Result;
                //IEnumerable<Contest> docsWithName = result.Current;
                //Contest firstDocWithName = result.FirstOrDefault();

                //if (firstDocWithName == null)
                //{
                //    Contests.InsertOne(contest);
                //    message = $"{contest.Name} was added";
                //}
                //else
                //{
                //    message = $"{contest.Name} already exits!";
                //}

                return $"Contest {contest.Name} was inserted";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return $"Failed to add contest error: {ex.Message}";
            }

        }

        public string CreateOpovEventCollection(string collectionName)
        {
            IMongoCollection<Contest> checkIfCollectionAlreadyExists = iMongoDatabase.GetCollection<Contest>(collectionName);

            long numDocs = checkIfCollectionAlreadyExists.CountDocuments(Builders<Contest>.Filter.Empty);

            if (numDocs == 0)
            {
                iMongoDatabase!.CreateCollection(collectionName); // if collection already exists then it is not created
                return $"Collection {collectionName} was created";
            }
            return $"Collection {collectionName} already exists";
        }
    }
}
