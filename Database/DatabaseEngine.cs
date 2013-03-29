using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace InMemoryDB.Database
{
    class DatabaseEngine
    {
        private readonly ThreadLocal<Dictionary<string, Dictionary<string, dynamic>>> _collections;
        private readonly IObjectCloner _objectObjectCloner;
        private readonly IIdentityGenerator _idGenerator;
        private readonly string _uniqueIdPropName;

        public DatabaseEngine(IIdentityGenerator identityGenerator, IObjectCloner objectCloner, string uniqueIdPropName)
        {
            _objectObjectCloner = objectCloner;
            _idGenerator = identityGenerator;
            _uniqueIdPropName = uniqueIdPropName;
            _collections = new ThreadLocal<Dictionary<string, Dictionary<string, dynamic>>>();
            _collections.Value = new Dictionary<string, Dictionary<string, dynamic>>();
        }

        public void Store<T>(T entity)
        {
            if (!IsCollectionExists(typeof (T)))
            {
                CreateCollectionFor(typeof (T));
            }
            bool isNew = IsNew(entity);
            if (isNew)
            {
                AssignNewIdentityTo(entity);
            }
            object id = GetUniqueId(entity);

            Save(entity, GetCollectionName(typeof (T)), isNew, id.ToString());
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            var subset = GetCollection<T>();
            if (filter != null)
            {
                subset = subset.Where(filter);
            }
            var clonedSubset = subset.Select(r => _objectObjectCloner.MakeClone(r));
            return clonedSubset;
        }

        public void DeleteAll<T>() where T : class
        {
            string collName = GetCollectionName(typeof (T));
            _collections.Value[collName] = new Dictionary<string, dynamic>();
        }

        public void DeleteRange<T>(IEnumerable<T> items) where T : class
        {
            string collName = GetCollectionName(typeof (T));
            foreach (T item in items)
            {
                object id = GetUniqueId(item);
                _collections.Value[collName].Remove(id.ToString());
            }
        }

        internal IQueryable<T> GetCollection<T>()
        {
            string collName = GetCollectionName(typeof (T));
            if (!_collections.Value.ContainsKey(collName))
            {
                throw new Exception("Collection does not exist");
            }
            return _collections.Value[collName].Values.Cast<T>().AsQueryable();
        }

        private void Save(object entity, string collName, bool isNew, string id)
        {
            object copiedEntity = _objectObjectCloner.MakeClone(entity); //cut reference to entity
            if (isNew)
            {
                _collections.Value[collName].Add(id, copiedEntity);
            }
            else
            {
                //update
                _collections.Value[collName][id] = copiedEntity;
            }
        }

        private object GetUniqueId<T>(T entity)
        {
            PropertyInfo idProp = FindUniqueIdProperty<T>();
            //found prop
            object id = idProp.GetValue(entity, null);
            return id;
        }

        private void AssignNewIdentityTo<T>(T entity)
        {
            PropertyInfo idProp = FindUniqueIdProperty<T>();
            //found prop
            try
            {
                object newId = _idGenerator.New();
                idProp.SetValue(entity, newId, null);
            }
            catch (Exception ex)
            {
                string msg = String.Format("Error setting identity property. Type mismatch: {0}", ex.Message);
                throw new Exception(msg);
            }
        }

        private bool IsNew<T>(T entity)
        {
            PropertyInfo idProp = FindUniqueIdProperty<T>();
            object val = idProp.GetValue(entity, null);
            if (_idGenerator.IsValid(val))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private PropertyInfo FindUniqueIdProperty<T>()
        {
            var props = typeof (T).GetProperties();
            foreach (PropertyInfo info in props)
            {
                if (String.Equals(info.Name, _uniqueIdPropName))
                {
                    //found prop
                    return info;
                }
            }
            throw new Exception("Unique property not found. Please define in database config.");
        }

        private string GetCollectionName(Type entityType)
        {
            return entityType.FullName;
        }

        private bool IsCollectionExists(Type entityType)
        {
            string collName = GetCollectionName(entityType);
            return _collections.Value.ContainsKey(collName);
        }

        private void CreateCollectionFor(Type entityType)
        {
            string collName = GetCollectionName(entityType);
            _collections.Value[collName] = new Dictionary<string, dynamic>();
        }
    }
}