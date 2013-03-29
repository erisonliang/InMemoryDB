namespace InMemoryDB.Database
{
    public interface IIdentityGenerator
    {
        object New();
        bool IsValid(object possibleId);
    }
}