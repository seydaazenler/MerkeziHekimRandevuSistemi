﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MHRSLite_BLL.Contracts
{
    public interface IRepositoryBase<T> where T : class, new()
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);
        T GetById(int id);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter = null,
            string includeProperties = null);

        bool Add(T entity);
        bool Update(T entity);
        bool Delete(T entity);
    }

}
