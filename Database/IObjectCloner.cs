namespace InMemoryDB.Database
{
    public interface IObjectCloner
    {
        T MakeClone<T>(T objToClone) where T : class;
    }
}