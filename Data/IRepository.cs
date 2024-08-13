
using System.Linq.Expressions;

namespace DetectiveClub.Data;

public interface IRepository<T>
{
    IQueryable<T> GetAll();

    T? GetById(object id);

    IQueryable<T> GetList(Expression<System.Func<T, bool>> predicate);

    int Add(T entity);

    void Edit(T entity);

    void Delete(T entity);
}