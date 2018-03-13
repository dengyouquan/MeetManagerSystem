using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DAL;

namespace BLL
{
    public class ExamineService:BaseService<Examine>,IExamineService
    {
        private IExamineDAL examineDAL = DALContainer.Resolve<IExamineDAL>();
        public override void SetDal()
        {
            Dal = examineDAL;
        }
    }
}
