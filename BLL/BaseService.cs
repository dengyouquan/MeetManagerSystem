using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BLL
{
    public abstract class BaseService<T> where T : class,new()
    {
        public IBaseDAL<T> Dal { get; set; }
        public abstract void SetDal();
        public BaseService()
        {
            SetDal();
        }
        public bool Add(T t)
        {
            Dal.Add(t);
            return Dal.SaveChanges();
        }

        public bool Delete(T t)
        {
            Dal.Delete(t);
            return Dal.SaveChanges();
        }

        public bool Update(T t)
        {
            Dal.Update(t);
            return Dal.SaveChanges();
        }

        public IQueryable<T> GetModels(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda)
        {
            return Dal.GetModels(whereLambda);
        }

        public IQueryable<T> GetModelsByPage<type>(int pageSize, int pageIndex, bool isAsc, System.Linq.Expressions.Expression<Func<T, type>> orderByLamdba, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda)
        {
            return Dal.GetModelsByPage(pageSize, pageIndex, isAsc, orderByLamdba, whereLambda);
        }
    }
}
