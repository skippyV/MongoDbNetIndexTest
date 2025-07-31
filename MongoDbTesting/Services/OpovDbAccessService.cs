using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbTesting.Data;
using System.Collections;

// Original design: have a collection of OpovEvents. For example, one document
// of the OpovEvents would be the CannaCup. Which would be completely unrelated to 
// another OpovEvent, say the TomatoCup, so should both documents reside in the same
// collection? For now I think the answer is yes.
//
// Therefore, there will be ONE Collection with the OpovDb called OpovEvents.
// And within that single Collection we will add OpovEvent documents.
// E.g. CannaCup document, TomatoCup document, etc.
//
// Each OpovEvent Document will reference the Contest objects added to it.
// 
// Reference: https://www.mongodb.com/community/forums/t/nested-collections/202832
//
// However, when one adds an Index to a TYPE of DOCUMENT that will be referenced by
// the collection, then those Documents within the collection will be indexed. 
//
// What confuses me about this code:
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

            CreateOpovEventsCollection(OpovDbAccessServiceConstants.OpovEventsCollectionName);
            AddUniqueIndexOnEventName(OpovDbAccessServiceConstants.OpovEventsCollectionName);
        }

        private void UpdateOpovEvent(OpovEvent opovEvent)
        {
            IMongoCollection<OpovEvent> opovEventsCollection = iMongoDatabase!.GetCollection<OpovEvent>(OpovDbAccessServiceConstants.OpovEventsCollectionName);
            FilterDefinition<OpovEvent> filter = Builders<OpovEvent>.Filter.Eq(nameof(opovEvent.EventName), opovEvent.EventName);

            UpdateDefinition<OpovEvent> updateDefinition = Builders<OpovEvent>.Update
            .Set(upRec => upRec.Contests, opovEvent.Contests);

            opovEventsCollection.UpdateOne(filter, updateDefinition);
        }

        public string AddContest(Contest contest, string opovEventName)
        {
            string message = "";
            try
            {
                // KEEP these comments for reference.
                // To add a Text Index on the Name field of Contest
                // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/indexes/
                // var indexModel = new CreateIndexModel<Contest>(Builders<Contest>.IndexKeys.Text(m => m.Name));
                // Contests.Indexes.CreateOne(indexModel);

                
                OpovEvent? opovEvent = GetOpovEvent(opovEventName);

                if (opovEvent is null)
                {
                    message = $"Error! Could NOT find OpovEvent {opovEventName}";
                    return message ;
                }

                if (opovEvent.Contests is not null)
                {
                    Contest? contestSearch = opovEvent.Contests.Find(c => c.Name.Equals(contest.Name));
                    if (contestSearch is null)
                    {
                        opovEvent.AddContest(contest);
                        UpdateOpovEvent(opovEvent);
                        message = $"Contest {contest.Name} was ADDED";
                    }
                    else
                    {
                        message = $"Contest {contest.Name} Already EXISTS!";
                    }
                }
                else
                {
                    opovEvent.AddContest(contest);
                    UpdateOpovEvent(opovEvent);
                    message = $"Contest {contest.Name} was ADDED";
                }

                // Task<IAsyncCursor<Contest>> doesContestExist = opovEvent.Contests.FindAsync(c => c.Name.Equals(contest.Name));
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
                return $"Failed to add contest error: {ex.Message}";
            }

        }

        // Not currently using.
        // I can't figure out how to use the propertyInfo in the lambda expression of IndexKeys.Ascending(xx)
        // So using AddUniqueIndexOnNameMemberOfContests() for now.
        public void AddIndexOnField(string collectionName, string fieldName)
        {
            IMongoCollection<Contest> Contests = iMongoDatabase!.GetCollection<Contest>(collectionName);

            System.Reflection.PropertyInfo? propertyInfo = typeof(Contest).GetProperty(fieldName);            

            var indexModel = new CreateIndexModel<Contest>(Builders<Contest>.IndexKeys.Ascending(c => c.Name));

            Contests.Indexes.CreateOne(indexModel);

        }

        private void AddUniqueIndexOnEventName(string collectionName)
        {
            IMongoCollection<OpovEvent> OpovEvents = iMongoDatabase!.GetCollection<OpovEvent>(collectionName);
            var options = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<OpovEvent>(Builders<OpovEvent>.IndexKeys.Ascending(c => c.EventName), options);

            OpovEvents.Indexes.CreateOne(indexModel);
        }        

        private string CreateOpovEventsCollection(string collectionName)
        {
            IMongoCollection<OpovEvent> checkIfCollectionAlreadyExists = iMongoDatabase!.GetCollection<OpovEvent>(collectionName);

            long numDocs = checkIfCollectionAlreadyExists.CountDocuments(Builders<OpovEvent>.Filter.Empty);

            if (numDocs == 0)
            {
                iMongoDatabase!.CreateCollection(collectionName); // if collectionName/collection already exists then it is not created
                return $"Collection {collectionName} was created";
            }
            return $"Collection {collectionName} already exists";
        }

        public Contest GetContest(string eventName, string contestName)
        {
            throw new NotImplementedException();
        }

        public string AddOpovEvent(OpovEvent opovEvent, string collectionName)
        {
            IMongoCollection<OpovEvent> currentOpovEvents = iMongoDatabase!.GetCollection<OpovEvent>(collectionName);

            var filter = Builders<OpovEvent>.Filter.Eq(g => g.EventName, opovEvent.EventName);
            var results = currentOpovEvents.Find(filter).ToList();

            if (results.Count == 0)
            {
                currentOpovEvents.InsertOne(opovEvent);
                return $"OpovEvent {opovEvent.EventName} was added!";
            }

            return $"OpovEvent {opovEvent.EventName} already exists! No changes made.";
        }
        /// <summary>
        /// Gets the OpovEvent with the input EventName
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns>OpovEvent or null</returns>
        public OpovEvent? GetOpovEvent(string eventName)
        {
            IMongoCollection<OpovEvent> OpovEventsCollection = iMongoDatabase!.GetCollection<OpovEvent>(OpovDbAccessServiceConstants.OpovEventsCollectionName);

            // Task<List<OpovEvent>> documents = OpovEventsCollection.Find(new BsonDocument()).ToListAsync(); // to get all documents from collection
            // List<OpovEvent> docs = documents.Result;

            long numDocs = OpovEventsCollection.CountDocuments(Builders<OpovEvent>.Filter.Empty);

            FilterDefinition<OpovEvent> filter = Builders<OpovEvent>.Filter.Eq(e => e.EventName, eventName);
            List<OpovEvent> findResults = OpovEventsCollection.Find(filter).ToList();

            if (findResults.Count > 0)
            {
                return findResults.First();
            }

            return null;
        }



        public List<OpovEvent> GetOpovEvents(string collectionName)
        {
            return [];
        }

        public List<string> GetDocumentNames(string collectionName)
        {
            List<string> eventNames = new List<string>();

            IMongoCollection<OpovEvent> currentOpovEvents = iMongoDatabase!.GetCollection<OpovEvent>(collectionName);

            List<OpovEvent> documents = currentOpovEvents.Find(new BsonDocument()).ToList();

            foreach (var doc in documents)
            {
                eventNames.Add(doc.EventName);
            }

            return eventNames;
        }

        public async Task<List<string>> ListCollectionNames()
        {
            int count = 0;
            List<string> names = new List<string>();

            IAsyncCursor<string> collectionNames = iMongoDatabase!.ListCollectionNames();
            List<string> stringList = await collectionNames.ToListAsync();

            // https://stackoverflow.com/questions/40007258/mongodb-c-sharp-driver-iasynccursorbsondocument-behavior
            // collectionNames.ForEachAsync(collectionName => { count++; a});

            return stringList;
        }


    }

    public static class OpovDbAccessServiceConstants
    {
        public const string OpovEventsCollectionName = "OpovEvents";

    }
}
