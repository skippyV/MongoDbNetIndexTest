using MongoDbTesting.Data;

namespace MongoDbTesting.Services
{
    public interface IOpovDbAccessService
    {

        string AddContest(Contest contest, string opovEventName);
        string AddOpovEvent(OpovEvent opovEvent, string collectionName);

        List<string> GetDocumentNames(string collectionName);
        List<OpovEvent> GetOpovEvents(string collectionName);

        OpovEvent GetOpovEvent(string eventName);

        Contest GetContest(string eventName, string contestName);

        Task<List<string>> ListCollectionNames();

    }
}
