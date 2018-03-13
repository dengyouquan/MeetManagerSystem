using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DAL;

namespace BLL
{
    public class MeetRoomService:BaseService<MeetRoom>,IMeetRoomService
    {
        private IMeetRoomDAL meetRoomDAL = DALContainer.Resolve<IMeetRoomDAL>();
        public override void SetDal()
        {
            Dal = meetRoomDAL;
        }
    }
}
