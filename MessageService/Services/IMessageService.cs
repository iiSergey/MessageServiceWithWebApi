using MessageService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Services
{
    public interface IMessageService
    {
        void DeleteMessage(Guid messageId);

        IEnumerable<Message> GetAllMessages();

        Message GetMessage(Guid messageId);

        Message InsertMessage(string message);

        void UpdateMessage(Guid messageId, string message);
    }


    public interface IEntityRepository<TKey, TEntity> where TEntity : IEntity
    {
        string keySpace { get; set; }

        Task<TEntity> findByKey(TKey key);

        //Task<TEntity> findOneByParams(search: object, options?: object);

        //Task<IEnumerable<TEntity>> findByParams(search: object, options?: object);

        //Task<long> count(search?: object, options?: object) : Promise<number>;

        //Task<IEntityList<TEntity>> page(search?: object, sortQuery?: ISortQuery, options?: object);

        //Task<TEntity> create(TEntity entity, options?: object);

        //Task update(TEntity entity, options?: object);

        //Task updateByKey(TKey key, data: object, options?: object);

        //Task remove(TEntity entity, options?: object);

        //Task removeByKey(TKey key, options?: object);
    }

    public interface IKey
    {
        IKey ExtractKey();
        TEntity NormalizeKey<TEntity>() where TEntity : IKey;
        TEntity NormalizeExternalKey<TEntity>(IKey externalKey) where TEntity : IKey;
        Expression<Func<TEntity, bool>> FindBy<TEntity>() where TEntity : IKey;
    }
    public interface IKey3: IKey
    {
        IKey ExtractKey();
        TEntity NormalizeKey<TEntity>() where TEntity : IKey3;
        TEntity NormalizeExternalKey<TEntity>(IKey externalKey) where TEntity : IKey3;
        Expression<Func<TEntity, bool>> FindBy<TEntity>() where TEntity : IKey3;
    }
    public interface IEntity:IKey
    {
        //created_at
        DateTime CreatedAt { get; set; }

        //updated_at
        DateTime UpdatedAt { get; set; }
    }

    public interface IEntityList<TEntity> where TEntity : IEntity
    {
        long count { get; set; }
        string pageState { get; set; }
        IEnumerable<TEntity> list { get; set; }
    }
    //public interface IQueryFindByKey<TEntity, TKey> where TEntity : IEntity where TKey: IKey
    //{
    //    Expression<Func<TEntity, bool>> FindByKey(TKey key);
    //}
    public interface IIdentityKey
    {
        Guid id { get; set; }
    }

    interface IIdentity : IEntity
    {
        Guid id { get; set; }
    }

    interface IIdentityRepository<TIdentity> : IEntityRepository<IIdentityKey, TIdentity> where TIdentity : IIdentity
    {
    }

    //interface ISortQuery
    //{
    //    string? pageState { get; set; }
    //    string? top { get; set; }
    //    string? orderby { get; set; }
    //    string? orderdir { get; set; }
    //}

    interface IDBContext
    {
        ICluster Cluster { get; }
        ISession GetSession(string keySpace);
    }

    class DBContext : IDBContext
    {
        public DBContext(ICluster cluster)
        {
            Debug.Assert(cluster != null, nameof(cluster) + " != null");
            this.Cluster = cluster;
        }

        public ICluster Cluster { get; }

        public ISession GetSession(string keySpace) => 
            Cluster.Connect(keySpace);
    }

    public class EntityRepository<K, T> : IEntityRepository<K, T> where T: IEntity where K: IKey
    {
        protected IDBContext dbContext { get; }
        public string keySpace { get; set; }

        EntityRepository( IDBContext dbContext,
               string keySpace)
    {
        this.dbContext = dbContext;
        this.keySpace = keySpace;
    }


        public async Task<T> findByKey(K key)
        {
            using (var session = dbContext.GetSession(keySpace))
            {
                return await session.GetTable<T>()
                    .First(key.FindBy<T>())
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<T> findOneByParams(Expression<Func<T,bool>> exception)
        {
            using (var session = dbContext.GetSession(keySpace))
            {
                return await session
                    .GetTable<T>()
                    .First(exception)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<TResult>> findOneByParams<TResult>(Expression<Func<T, bool>> exceptionWhere, Expression<Func<T, TResult>> exceptionSelect)
        {
            using (var session = dbContext.GetSession(keySpace))
            {
                return await session
                    .GetTable<T>()
                    .Where(exceptionWhere)
                    .Select(exceptionSelect)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<long> count(Expression<Func<T, bool>> exceptionWhere)
        {
            using (var session = dbContext.GetSession(keySpace))
            {
                return await session
                    .GetTable<T>()
                    .Where(exceptionWhere)
                    .Count()
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task create(T model)
        {
            using (var session = dbContext.GetSession(keySpace))
            {
                await session
                    .GetTable<T>()
                    .Insert(model)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
        }
        public async Task updateByKey(K key,T model)
        {
           T normalizeModel = model.NormalizeExternalKey<T>(key);
            using (var session = dbContext.GetSession(keySpace))
            {
                await session
                    .GetTable<T>()
                    .Select(t => normalizeModel)
                    .UpdateIf(key.FindBy<T>())
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
        }
        public async Task update(T model, Expression<Func<T, bool>> exceptionWhere)
        {
            T normalizeModel = model.NormalizeKey<T>();
            var key = model.ExtractKey();
            using (var session = dbContext.GetSession(keySpace))
            {
                await session
                    .GetTable<T>()
                    .Select(u => normalizeModel)
                    .UpdateIf(key.FindBy<T>())
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task removeByKey(K key)
        {
            using (var session = dbContext.GetSession(keySpace))
            {
                await session
                    .GetTable<T>()
                    .DeleteIf(key.FindBy<T>())
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
        }
        public async Task remove(T model, Expression<Func<T, bool>> exceptionWhere)
        {
            var key = model.ExtractKey();
            using (var session = dbContext.GetSession(keySpace))
            {
                await session
                    .GetTable<T>()
                    .UpdateIf(key.FindBy<T>())
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<IPage<TResult>> page<TResult>(
            Expression<Func<T, bool>> exceptionWhere,
            Expression<Func<T, TResult>> exceptionSelect,
            Expression<Func<T, K>> exceptionOrderBy)
        {
            using (var session = dbContext.GetSession(keySpace))
            {
                return await session
                    .GetTable<T>()
                    .Where(exceptionWhere)
                    .OrderBy(exceptionOrderBy)
                    .Select(exceptionSelect)
                    .ExecutePagedAsync()
                    .ConfigureAwait(false);
            }
        }
    }

}