using CachingFramework.Redis.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PradeepTech.DataAccess.Helpers;
using PradeepTech.DataAccess.Models;
using PradeepTech.Domain.Context;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace PradeepTech.DataAccess.Base
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        protected readonly DataContext Context;

        internal DbSet<T> dbSet;

        DataContext IDataRepository<T>.Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DataRepository(DataContext context)
        {
            Context = context;
            this.dbSet = context.Set<T>();
        }

        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>()
                .Where<T>(predicate)
                .FirstOrDefault();
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>()
                .Where<T>(predicate)
                .FirstOrDefaultAsync();
        }

        public virtual bool AlreadyExists(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>()
                .Any<T>(predicate);
        }

        public int GetLast(Expression<Func<T, int>> predicate)
        {
            return Context.Set<T>().Count() == 0 ? 0 : Context.Set<T>().Max(predicate);
        }

        public async Task<int> GetLastAsync(Expression<Func<T, int>> predicate)
        {
            return await Context.Set<T>().CountAsync() == 0 ? 0 : await Context.Set<T>().MaxAsync(predicate);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>()
                .AsNoTracking()
                .ToList();
        }

        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>()
                .AsNoTracking()
                .Where<T>(predicate)
                .ToList();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Context.Set<T>()
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>()
                .AsNoTracking()
                .Where<T>(predicate)
                .ToListAsync();
        }

        public virtual void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("entity is null");
            }

            _ = Context.Set<T>().Add(entity);
            _ = Context.SaveChanges();
        }

        public virtual async Task InsertAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("entity is null");
            }

            _ = Context.Set<T>().Add(entity);
            _ = await Context.SaveChangesAsync();
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("entity is null");
            }

            _ = Context.Set<T>().Update(entity);
            _ = Context.SaveChanges();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("entity is null");
            }

            _ = Context.Set<T>().Update(entity);
            _ = await Context.SaveChangesAsync();
        }

        public virtual void Delete(T entity)
        {
            _ = Context.Set<T>().Remove(entity);
            _ = Context.SaveChanges();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _ = Context.Set<T>().Remove(entity);
            _ = await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}