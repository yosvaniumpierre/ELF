using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Avanade.Repository.Services 
{
    public interface ICanAdd<in T>
        where T : class
    {
        #region Methods 

        void Add(T entity);

        /// <summary>
        /// Reattaches an entity to the session.
        /// If the ORM implementation uses NHibernate, then this method is semantically equivalent to 'Update'.
        /// A call to Update is used to tell NHibernate to start tracking an object that it is not being tracked already (aka detached object).
        /// </summary>
        /// <param name="entity"></param>
        void Attach(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        #endregion Methods
    }

    public interface ICanRemove<T>
        where T : class
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Remove(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        void Remove(Expression<Func<T, bool>> predicate);

        #endregion Methods
    }

    public interface IRepository<T> : ICanRemove<T>, ICanAdd<T>
        where T : class
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IQueryable<T> AsQueryable();

        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        T First(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetAll();

        T Single(Expression<Func<T, bool>> predicate);

        int TotalCount();

        int Count(Expression<Func<T, bool>> predicate);

        #endregion Methods
    }
}