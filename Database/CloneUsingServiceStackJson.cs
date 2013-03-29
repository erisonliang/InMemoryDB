using ServiceStack.Text;

namespace InMemoryDB.Database
{
    sealed class CloneUsingServiceStackJson : IObjectCloner
    {
        public T MakeClone<T>(T objToClone) where T : class
        {
            TypeSerializer<T> serializer = new TypeSerializer<T>();
            string json = serializer.SerializeToString(objToClone);

            return serializer.DeserializeFromString(json);
        }
    }
}