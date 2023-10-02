using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NexusServer.Data 
{
    public class DataMethods<TEntity> : IDataMethods<TEntity> where TEntity : class
    {
        private readonly DataContext _dbContext;

        public DataMethods(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Generic method to get all entities
        public List<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>().ToList();
        }

        // Generic method to get entities with filtering and sorting
        public List<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            // Apply filtering
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include related entities
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            // Apply ordering
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        // Generic method to get the first entity that matches the filter or default value
        public TEntity GetFirstOrDefault(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            // Apply filtering
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include related entities
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query.FirstOrDefault();
        }

        // Add more generic methods as needed

        // Example method to add a new entity
        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        // Example method to update an entity
        public void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        // Example method to delete an entity
        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        // Save changes to the database
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }

}
