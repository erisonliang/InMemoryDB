using System.Threading;

namespace InMemoryDB.Database.IdGenerators
{
    public sealed class LongIdGenerator : IIdentityGenerator
    {
        private long _currentWatermarkOfId;

        public LongIdGenerator() : this(0)
        {
        }

        public LongIdGenerator(long startingIdValue)
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