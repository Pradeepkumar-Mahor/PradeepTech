using PradeepTech.Domain.Context;
using System.Linq.Expressions;

namespace PradeepTech.DataAccess.Base
{
    public interface IDataRepository<T> : IDisposable
    {
        DataContext Context { get; set; }

        T Get(Expression<Func<T, bool>> predicate);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        bool AlreadyExists(Expression<Func<T, bool>> predicate);

        int GetLast(Expression<Func<T, int>> predicate);

        Task<int> GetLastAsync(Expression<Func<T, int>> predicate);

        IEnumerable<T> GetAll();

        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        void Insert(T entity);

        Task InsertAsync(T entity);

        void Update(T entity);

        Task UpdateAsync(T entity);

        void Delete(T entity);

        Task DeleteAsync(T entity);
    }
}