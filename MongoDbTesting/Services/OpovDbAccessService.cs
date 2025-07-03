using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using MongoDbTesting.Data;

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

                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "ERROR";
            }

        }

        public void CreateCollection(string collectionName)
        {
            iMongoDatabase!.CreateCollection(collectionName); // if collection exists then it is not created
        }
    }
}
