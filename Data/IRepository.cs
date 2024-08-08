﻿using System.Linq;
using System.Linq.Expressions;

namespace DetectiveClub.Data;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();

    T? GetById(object id);

    IQueryable<T> GetList(Expression<System.Func<T, bool>> predicate);

    int Add(T entity);

    void Edit(T entity);

    void Delete(T entity);
}