using System.Threading;

namespace InMemoryDB.Database.IdGenerators
{
    public sealed class IntIdGenerator : IIdentityGenerator
    {
        private int _currentWatermarkOfId;

        public IntIdGenerator() : this(0)
        {
        }

        public IntIdGenerator(int startingIdValue)
        {
            _currentWatermarkOfId = startingIdValue;
        }

        public object New()
        {
            Interlocked.Increment(ref _currentWatermarkOfId);
            return _currentWatermarkOfId;
        }

        public bool IsValid(object possibleId)
        {
            if (possibleId == null) return false;

            string strId = possibleId.ToString();
            return strId != "0";
        }
    }
}