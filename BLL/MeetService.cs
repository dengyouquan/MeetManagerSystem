using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DAL;

namespace BLL
{
    public class MeetService:BaseService<Meet>,IMeetService
    {
        private IMeetDAL meetDAL = DALContainer.Resolve<IMeetDAL>();
        public override void SetDal()
        {
            Dal = meetDAL;
        }
    }
}
