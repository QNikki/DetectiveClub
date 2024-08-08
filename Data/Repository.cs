using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DetectiveClub.Data;

internal class Repository<TEntity>(DbContext context) : IRepository<TEntity> where TEntity : class
{
    public int Add(TEntity entity)
    {
        context.Set<TEntity>().Add(entity);
        return context.SaveChanges();
    }

    public void Delete(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity);
        context.SaveChanges();
    }

    public void Edit(TEntity entity)
    {
        context.Set<TEntity>().Update(entity);
        context.SaveChanges();
    }

    public TEntity GetById(object id) => context.Set<TEntity>().Find(id);

    public IQueryable<TEntity> GetAll() => context.Set<TEntity>();

    public IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate) =>
        context.Set<TEntity>().Where(predicate);
}