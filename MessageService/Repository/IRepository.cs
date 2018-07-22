using System;
using System.Collections.Generic;

namespace MessageService.Repository
{
    public interface IRepository<TEntity, TKey>
    {
        TEntity Delete(TKey messageId);

        TEntity Get(TKey messageId);

        IEnumerable<TEntity> GetAll();

        TEntity Insert(TEntity entity);

        void Update(TKey messageId, TEntity entity);
    }

    public interface IRepository<TEntity> : IRepository<TEntity, Guid>
    {
    }
}