namespace Avanade.Repository.Services.Hibernate.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using NHibernate.Linq;

    public abstract class NHibernateRepository<T> : IRepository<T>
        where T : class
    {
        #region Properties

        protected INHibernateUnitOfWork NHibernateUnitOfWork
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public virtual void Add(T entity)
        {
            NHibernateUnitOfWork.CurrentSession.Save(entity);
        }

        public virtual IQueryable<T> AsQueryable()
        {
            return NHibernateUnitOfWork.CurrentSession.Linq<T>();
        }

        public virtual void Attach(T entity)
        {
            NHibernateUnitOfWork.CurrentSession.Update(entity);
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return NHibernateUnitOfWork.CurrentSession.Linq<T>().Count(predicate);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return NHibernateUnitOfWork.CurrentSession.Linq<T>().Where(predicate).ToArray();
        }

        public virtual T First(Expression<Func<T, bool>> predicate)
        {
            return NHibernateUnitOfWork.CurrentSession.Linq<T>().First(predicate);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return NHibernateUnitOfWork.CurrentSession.Linq<T>();
        }

        public virtual void Remove(T entity)
        {
            NHibernateUnitOfWork.CurrentSession.Delete(entity);
        }

        public virtual void Remove(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> records = Find(predicate);
            foreach (T record in records)
            {
                NHibernateUnitOfWork.CurrentSession.Delete(record);
            }
        }

        public virtual T Single(Expression<Func<T, bool>> query)
        {
            return NHibernateUnitOfWork.CurrentSession.Linq<T>().SingleOrDefault(query);
        }

        public int TotalCount()
        {
            return NHibernateUnitOfWork.CurrentSession.Linq<T>().Count();
        }

        public virtual void Update(T entity)
        {
            NHibernateUnitOfWork.CurrentSession.SaveOrUpdate(entity);
        }

        #endregion Methods
    }
}