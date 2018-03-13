using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace BLL
{
    public interface IBaseService<T> where T:class,new()
    {
        bool Add(T t);
        bool Delete(T t);
        bool Update(T t);
        IQueryable<T> GetModels(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> GetModelsByPage<type>(int pageSize, int pageIndex, bool isAsc, Expression<Func<T, type>> orderByLamdba, Expression<Func<T, bool>> whereLambda);
    }
}
