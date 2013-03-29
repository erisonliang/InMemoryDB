using System;

namespace InMemoryDB.Database.IdGenerators
{
    public sealed class GuidIdGenerator : IIdentityGenerator
    {
        public object New()
        {
            return Guid.NewGuid();
        }

        public bool IsValid(object possibleId)
        {
            if (possibleId == null) return false;

            string strId = possibleId.ToString();
            Guid result;
            if (Guid.TryParse(strId, out result))
            {
                if (result == Guid.Empty)
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}