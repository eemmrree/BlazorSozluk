using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DbContext dbContext;
        protected DbSet<TEntity> entitys =>dbContext.Set<TEntity>();
        public GenericRepository(DbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        #region Add
        public int Add(TEntity entity)
        {
            this.entitys.Add(entity);
            return dbContext.SaveChanges();
        }

        public int Add(IEnumerable<TEntity> entities)
        {
            if (entities == null && !entities.Any())
                return 0;
            entitys.AddRange(entities);
            return dbContext.SaveChanges();
        }

        public async Task<int> AddAsync(TEntity entity)
        {
            await this.entitys.AddAsync(entity);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> AddAsync(IEnumerable<TEntity> entity)
        {
            if (entity == null && !entity.Any())
                return 0;
            await entitys.AddRangeAsync(entity);
            return await dbContext.SaveChangesAsync();
        }
        #endregion
        #region AddOrUpdate
        public int AddOrUpdate(TEntity entity)
        {
            if(!this.entitys.Local.Any(i=>EqualityComparer<Guid>.Default.Equals(i.Id,entity.Id)))
                dbContext.Update(entity);
            return dbContext.SaveChanges();
        }

        public Task<int> AddOrUpdateAsync(TEntity entity)
        {
            if (!this.entitys.Local.Any(i => EqualityComparer<Guid>.Default.Equals(i.Id, entity.Id)))
                dbContext.Update(entity);
            return dbContext.SaveChangesAsync();
        }
        #endregion
        public IQueryable<TEntity> AsQueryable() => entitys.AsQueryable();

        public virtual async Task BulkAdd(IEnumerable<TEntity> entities)
        {
            if(entities !=null && !entities.Any())
                await Task.CompletedTask;

            await entitys.AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();

        }

        public virtual Task BulkDelete(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public virtual Task BulkDelete(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public virtual Task BulkDeleteById(IEnumerable<Guid> ids)
        {
            if (ids != null && !ids.Any())
                return Task.CompletedTask;
            dbContext.RemoveRange(entitys.Where(i => ids.Contains(i.Id)));
            return dbContext.SaveChangesAsync();
        }

        public virtual Task BulkUpdate(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
        #region Delete
        public virtual int Delete(TEntity entity)
        {
            if(dbContext.Entry(entity).State == EntityState.Detached)
                this.entitys.Attach(entity);
            this.entitys.Remove(entity);
            return dbContext.SaveChanges();
        }

        public virtual int Delete(Guid id)
        {
            var ent = this.entitys.Find(id);
            return Delete(ent);
        }

        public virtual Task<int> DeleteAsync(TEntity entity)
        {
            if(dbContext.Entry(entity).State == EntityState.Detached)
                this.entitys.Attach(entity);
            this.entitys.Remove(entity);
            return dbContext.SaveChangesAsync();

        }

        public virtual Task<int> DeleteAsync(Guid id)
        {
            var ent = this.entitys.Find(id);
            return DeleteAsync(ent);
        }

        public virtual bool DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            dbContext.RemoveRange(entitys.Where(predicate));
            return dbContext.SaveChanges()>0;
        }

        public virtual async Task<bool> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            dbContext.RemoveRange(entitys.Where(predicate));
            return await dbContext.SaveChangesAsync() > 0;
        }
        #endregion
        public virtual Task<TEntity> FirstAllDefaultAsync(Expression<Func<TEntity, bool>> expression, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = entitys.AsQueryable();
            if(expression !=null)
                query = query.Where(expression);
            query = ApplyIncludes(query, includes);
            if(noTracking)
                query = query.AsNoTracking();
            return query;
        }

        public virtual Task<List<TEntity>> GetAll(bool noTracking = true)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            TEntity found = await entitys.FindAsync(id);
            if(found == null)
                return null;
            if(noTracking)
                dbContext.Entry(found).State = EntityState.Detached;
            foreach(Expression<Func<TEntity, object>> include in includes)
            {
                dbContext.Entry(found).Reference(include).Load();
            }
            return found;
        }

        public virtual async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> expression, bool noTracking = true, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = entitys.AsQueryable();
            if (expression != null)
                query = query.Where(expression);
            foreach(Expression<Func<TEntity,object>> include in includes)
            {
                query = query.Include(include);
            }
            if (orderBy != null)
                query = orderBy(query);
            if (noTracking)
                query = query.AsNoTracking();
            return await query.ToListAsync();
            
        }

        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> expression, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = entitys.AsQueryable();
            if (expression != null)
                query = query.Where(expression);
            query = ApplyIncludes(query, includes);
            if (noTracking)
                query = query.AsNoTracking();
            return await query.SingleOrDefaultAsync();
        }
        #region Update
        public virtual int Update(TEntity entity)
        {
            this.entitys.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            return dbContext.SaveChanges();
        }

        public virtual Task<int> UpdateAsync(TEntity entity)
        {
            this.entitys.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            return dbContext.SaveChangesAsync();
        }
        #endregion
        #region SaveChanges
        public virtual Task<int> SaveChangesAsync()
        {
            return dbContext.SaveChangesAsync();
        }
        public virtual int SaveChanges()
        {
            return dbContext.SaveChanges();
        }
        #endregion
        private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query , params Expression<Func<TEntity,object>>[] includes)
        {
            if (includes != null)
            {
                foreach(var includeItem in includes)
                {
                    query = query.Include(includeItem);
                }
            }
            return query;
        }
    }
}
