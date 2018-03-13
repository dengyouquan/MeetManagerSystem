using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DAL;

namespace BLL
{
    class MeetMsgService:BaseService<MeetMsg>,IMeetMsgService
    {
        private IMeetMsgDAL meetMsgDAL = DALContainer.Resolve<IMeetMsgDAL>();
        public override void SetDal()
        {
            Dal = meetMsgDAL;
        }
    }
}
