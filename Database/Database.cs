using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace InMemoryDB.Database
{
    public static class Database
    {
        private static DatabaseEngine _engine;

        public static void Init()
        {
            Init(new Configuration());
        }

        public static void Init(Configuration config)
        {
            _engine = new DatabaseEngine(config.IdentityGenerator, config.ObjectCloner, config.UniqueIdentifierName);
        }

        public static void Store<T>(T entity) where T : class
        {
            GuardInit();
            _engine.Store(entity);
        }

        public static void DeleteAll<T>() where T : class
        {
            GuardInit();
            _engine.DeleteAll<T>();
        }

        public static void Delete<T>(Expression<Func<T, bool>> filter) where T : class
        {
            GuardInit();
            var itemsToDelete = Query(filter).ToList();
            _engine.DeleteRange(itemsToDelete);
        }

        public static IEnumerable<T> Query<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            GuardInit();
            var subset = _engine.Query(filter);
            return subset.AsEnumerable();
        }

        public static int Count<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            GuardInit();
            var items = _engine.GetCollection<T>();
            if (filter == null) return items.Count();

            return items.Count(filter);
        }

        private static void GuardInit()
        {
            if (_engine == null)
            {
                throw new Exception("Please initialize database by calling Database.Init()");
            }
        }
    }
}