using InMemoryDB.Database.IdGenerators;

namespace InMemoryDB.Database
{
    public class Configuration
    {
        public string UniqueIdentifierName { get; set; }
        public IIdentityGenerator IdentityGenerator { get; set; }
        public IObjectCloner ObjectCloner { get; set; }

        public Configuration()
        {
            UniqueIdentifierName = "Id";
            IdentityGenerator = new IntIdGenerator();
            ObjectCloner = new CloneUsingServiceStackJson();
        }
    }
}