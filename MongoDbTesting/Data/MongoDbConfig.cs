namespace MongoDbTesting.Data
{
    public class MongoDbConfig
    {
        public required string IdentityDatabaseName { get; init; }
        public required string OPOVDatabaseName { get; init; }
        public required string Host { get; init; }
        public int Port { get; init; }
        public string IdentityDbConnectionString => $"mongodb://{Host}:{Port}/{IdentityDatabaseName}";
        public string OpovDbConnectionString => $"mongodb://{Host}:{Port}/{OPOVDatabaseName}";
    }
}
