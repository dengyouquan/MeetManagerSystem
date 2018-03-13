using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using System.Linq.Expressions;

namespace DAL
{
    public class BaseDAL<T> where T:class,new()
    {
        private MyDbContext dbContext = DbContextFactory.Create() as MyDbContext;
        public void Add(T t)
        {
            dbContext.Set<T>().Add(t);
        }

        public void Delete(T t)
        {
            dbContext.Set<T>().Remove(t);
        }

        public void Update(T t)
        {
            //加上using System.Data.Entity.Migrations;
            dbContext.Set<T>().AddOrUpdate(t);
        }

        public IQueryable<T> GetModels(Expression<Func<T, bool>> whereLambda)
        {
            return dbContext.Set<T>().Where(whereLambda);
        }

        public IQueryable<T> GetModelsByPage<type>(int pageSize, int pageIndex, bool isAsc, Expression<Func<T, type>> OrderByLambda, Expression<Func<T, bool>> whereLambda)
        {
            if (isAsc)
            {
                return dbContext.Set<T>().Where(whereLambda).OrderBy(OrderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                return dbContext.Set<T>().Where(whereLambda).OrderByDescending(OrderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
        }

        public bool SaveChanges()
        {
            return dbContext.SaveChanges() > 0;
        }
    }
}
