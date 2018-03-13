using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using BLL;
using Common;
using System.Collections;

namespace WebApp.Controllers
{
    [Authorize(Roles="admin")]
    public class MeetRoomController : Controller
    {
        private IMeetService meetService = BLLContainer.Resolve<IMeetService>();
        private IExamineService examineService = BLLContainer.Resolve<IExamineService>();
        private IMeetMsgService meetMsgService = BLLContainer.Resolve<IMeetMsgService>();
        private IMeetRoomService meetRoomService = BLLContainer.Resolve<IMeetRoomService>();

        //
        // GET: /MeetRoom/
        public ActionResult Index(string searchString)
        {
            int pageSize = 5;
            int itemCount;
            if (!String.IsNullOrEmpty(searchString))
            {
                itemCount = meetRoomService.GetModels(p => p.Name.Contains(searchString) || p.Content.Contains(searchString)).Count();
            }
            else
            {
                itemCount = meetRoomService.GetModels(p => true).Count();
            }
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<MeetRoom> meetRooms = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    meetRooms = meetRoomService.GetModelsByPage(pageSize, pageIndex, true, a => a.Name, p => p.Name.Contains(searchString) || p.Content.Contains(searchString)).ToList();
                }
                else
                {
                    meetRooms = meetRoomService.GetModelsByPage(pageSize, pageIndex, true, a => a.Name, p => true).ToList();
                }
            }
            ViewData["meetRooms"] = meetRooms;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            ViewData["searchString"] = searchString;
            return View();
        }

        #region 会议室管理

        [Authorize(Roles="admin,apply")]
        public ActionResult TimeIsUsed()
        {
            string data = null;
            int id = int.Parse(Request.Params["id"]);
            string dateStr = Request.Params["time"].Split(' ')[0];
            DateTime dateTime = DateTime.Parse(Request.Params["time"]);
            if (dateTime == null)
            {
                return Content("输入时间格式有误");
            }
            MeetRoom meetRoom = meetRoomService.GetModels(p => p.MeetRoomId == id).FirstOrDefault();
            String[] dates = new String[0];
            if (meetRoom != null && !String.IsNullOrEmpty(meetRoom.UseTime))
            {
                dates = meetRoom.UseTime.Split('|');
            }
            foreach (String date in dates)
            {
                String[] startend = date.Split(',');
                String[] startStrArr = startend[1].Split(' ');
                String[] endStrArr = startend[2].Split(' ');
                String startStr = startStrArr[0].Replace("/", "-");
                String endStr = endStrArr[0].Replace("/", "-");
                if (DateTime.Parse(startStr).CompareTo(DateTime.Parse(dateStr))==0 || DateTime.Parse(endStr).CompareTo(DateTime.Parse(dateStr))==0)
                {
                    if (data != null)
                    {
                        data += "\n"+startStrArr[1] + "到" + endStrArr[1];
                    }
                    else
                    {
                        data += "今天有会议：" + startStrArr[1] + "到" + endStrArr[1];
                    }
                }
            }
            return Content(data);
        }

        public ActionResult GetUseTime()
        {
            string data = null;
            List<String> list = new List<String>();
            int id = int.Parse(Request.Params["id"]);
            MeetRoom meetRoom = meetRoomService.GetModels(p => p.MeetRoomId == id).FirstOrDefault();
            String[] dates = new String[0];
            if (meetRoom != null && !String.IsNullOrEmpty(meetRoom.UseTime))
            {
                dates = meetRoom.UseTime.Split('|');
            }
            foreach (String date in dates)
            {
                String[] startend = date.Split(',');
                String[] startStrArr = startend[1].Split(' ');
                String[] endStrArr = startend[2].Split(' ');
                String startStr = startStrArr[0].Replace("/", "-");
                String endStr = endStrArr[0].Replace("/", "-");
                String startDay = startStr.Split('-')[2];
                String endDay = startStr.Split('-')[2];
                //int startDay = int.Parse(startStr.Split('-')[2]);
                //int endDay = int.Parse(startStr.Split('-')[2]);
                if (!list.Contains(startDay))
                {
                    list.Add(startDay);
                }
                if (list.Contains(endDay))
                {
                    list.Add(endDay);
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);;
        }
        public ActionResult AddMeetRoom()
        {
            if (Request.Params["id"] != null)
            {
                int id = int.Parse(Request.Params["id"]);
                MeetRoom meetRoom = meetRoomService.GetModels(p => p.MeetRoomId == id).FirstOrDefault();
                ViewData["meetRoom"] = meetRoom;
                ViewData["Name"] = meetRoom.Name;
                ViewData["HoldPerson"] = meetRoom.HoldPerson;
                ViewData["Address"] = meetRoom.Address;
                ViewData["Describe"] = meetRoom.Describe;
                ViewData["Picture"] = meetRoom.Picture;
                ViewData["Status"] = meetRoom.Status;
                ViewData["MeetRoomId"] = meetRoom.MeetRoomId;
            }
            return View();
        }
       
        [ValidateInput(false)]
        public ActionResult Add(MeetRoom meetRoom)
        {
            if (meetRoom.MeetRoomId != 0)
            {

                //显示加载
                MeetRoom updateMeetRoom = meetRoomService.GetModels(p => p.MeetRoomId == meetRoom.MeetRoomId).FirstOrDefault();


                updateMeetRoom.Name = meetRoom.Name;
                updateMeetRoom.HoldPerson = meetRoom.HoldPerson;
                updateMeetRoom.Address = meetRoom.Address;
                updateMeetRoom.Describe = meetRoom.Describe;
                //updateMeetRoom.Picture = meetRoom.Picture;
                if (updateMeetRoom.Name != null && meetRoomService.Update(updateMeetRoom))
                {
                    return Content("ok");
                }
                else
                {
                    return Content("no");
                }
            }
            else
            {
                if (meetRoom.Name != null && meetRoomService.Add(meetRoom))
                {
                    return Content("ok");
                }
                else
                {
                    return Content("no");
                }
            }
        }

        public ActionResult Delete()
        {
            int id = int.Parse(Request["id"]);
            MeetRoom meetRoom = meetRoomService.GetModels(p => p.MeetRoomId == id).FirstOrDefault();
            if (meetRoomService.Delete(meetRoom))
            {
                return Content("ok");
            }
            else
            {
                return Content("no");
            }
        }
        public ActionResult MeetRoomDetail()
        {
            int id = int.Parse(Request["id"]);
            MeetRoom meetRoom = meetRoomService.GetModels(p => p.MeetRoomId == id).FirstOrDefault();
            return Json(meetRoom, JsonRequestBehavior.AllowGet);
        }
        #endregion
	}
}