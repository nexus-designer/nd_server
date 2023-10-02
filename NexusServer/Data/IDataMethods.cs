using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NexusServer.Data;
public interface IDataMethods<TEntity> where TEntity : class
{
    public List<TEntity> GetAll();
    public List<TEntity> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");
    public TEntity GetFirstOrDefault(
        Expression<Func<TEntity, bool>> filter = null,
        string includeProperties = "");
    public void Add(TEntity entity);

    public void Update(TEntity entity);

    // Example method to delete an entity
    public void Delete(TEntity entity);
    public void Save();
}
