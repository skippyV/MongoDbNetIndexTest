using MongoDbTesting.Data;

namespace MongoDbTesting.Services
{
    public interface IOpovDbAccessService
    {
        void CreateCollection(string collectionName);
        string AddContest(Contest contest, string collectionName);
        void AddIndexOnNameField(string collectionName);
    }
}
