using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using BLL;
using System.IO;

namespace WebApp.Controllers
{
    [Authorize]
    public class MeetController : Controller
    {
        private IMeetService meetService = BLLContainer.Resolve<IMeetService>();
        private IExamineService examineService = BLLContainer.Resolve<IExamineService>();
        private IMeetMsgService meetMsgService = BLLContainer.Resolve<IMeetMsgService>();
        private IMeetRoomService meetRoomService = BLLContainer.Resolve<IMeetRoomService>();
        //
        // GET: /Meet/

        //0:未审核 1：审核通过 -1：审核未通过 2：纪要 3：归档失败 4：归档成功

        //删除时更改Status
        //meetmemo 内容加载不起
        //meet Status审核和纪要可以同时共存

        //传pageIndex 和传ID 冲突 在meetApply meetExamine等，只有有pageBar全有冲突

        //申请者的申请审核后无法修改删除 纪要者的纪要归档后无法修改删除

        //各单位的更新问题？？？

        //查询等冗余代码封装问题？？

        //显示加载 不能使用延迟加载 meet 指明meetMsg  不然update后置空   有问题 要是本身没有Examine等怎么加载
        //Meet updateMeet = meetService.GetModels(p => p.MeetId == meet.MeetId && p.Examine.ExamineId == meet.MeetId && p.MeetMsg.MeetMsgId == meet.MeetId).FirstOrDefault();
        //显示加载
            //if (meet.MeetMsg != null)
            //{
            //    int mid = meet.MeetMsg.MeetMsgId;
            //}
            //if (meet.Examine != null)
            //{
            //    int eid = meet.Examine.ExamineId;
            //}      

        //靠Status支持的太脆弱，判断Examine是否存在

        //easyui子界面如会议通知，登录，发现不能刷新全界面，在login.cshtml中把Html.BeginForm改成Ajax.BeginForm(引入js)
        //然后写一个回调函数，回调父界面的刷新界面函数，不能直接用子界面的刷新，只会刷新子界面

        //实现搜索 没有实现类型搜索
        //搜索会搜索到HTML标签中的内容，如何避免？？？？搜t会搜到<strong>的t



        #region 会议申请
        [Authorize(Roles = "apply,admin")]
        public ActionResult MeetManager()
        {
            //string id = Request.Params["id"];
            //List<Meet> meets = meetService.GetModels(p => p.ApplyPeople==id).ToList();
            //ViewData["meets"] = meets;
            string id = Request.Params["id"];
            int pageSize = 5;
            int itemCount = meetService.GetModels(p => p.ApplyPeople == id).Count();
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<Meet> meets = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                meets = meetService.GetModelsByPage<Nullable<DateTime>>(pageSize, pageIndex, true, a => a.StartTime, p => p.ApplyPeople == id).ToList();

            }
            ViewData["meets"] = meets;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            if (Request.Params["id"] != null)
            {
                ViewData["id"] = id;
            }
            return View();
        }
        [Authorize(Roles = "apply,admin")]
        public ActionResult ApplyedMeet()
        {
            int id = int.Parse(Request.Params["id"]);
            MeetRoom meetRoom = meetRoomService.GetModels(p=>p.MeetRoomId==id).FirstOrDefault();
            List<Meet> meets = new List<Meet>();
            String[] dates = new String[0];
            if (meetRoom!=null && !String.IsNullOrEmpty(meetRoom.UseTime))
            {
               dates = meetRoom.UseTime.Split('|');
            }
            foreach (String date in dates)
            {
                String[] startend = date.Split(',');
                int mid = int.Parse(startend[0]);
                Meet meet = meetService.GetModels(p => p.MeetId == mid).FirstOrDefault();
                meets.Add(meet);
            }
            ViewData["meets"] = meets;
            return View();
        }
        [Authorize(Roles = "apply,admin")]

        public ActionResult MeetApply(string searchString)
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
            return View();
        }
        [Authorize(Roles = "apply,admin")]
        public ActionResult ShowApply()
        {
            if (Request.Params["id"] != null)
            {
                int id = int.Parse(Request.Params["id"]);
                Meet meet = meetService.GetModels(p=>p.MeetId==id).FirstOrDefault();
                ViewData["meet"] = meet;
                ViewData["Title"] = meet.Title;
                ViewData["Type"] = meet.Type;
                ViewData["HostPeople"] = meet.HostPeople;
                ViewData["Budget"] = meet.Budget;
                ViewData["Address"] = meet.Address;
                ViewData["StartTime"] = meet.StartTime;
                ViewData["EndTime"] = meet.EndTime;
                ViewData["Content"] = meet.Content;
                ViewData["ApplyPeople"] = meet.ApplyPeople;
                ViewData["MeetId"] = meet.MeetId;
                ViewData["Other"] = meet.Other;
            }
            return View();
        }
        [Authorize(Roles = "apply,admin")]
        [ValidateInput(false)]
        public ActionResult Apply(Meet meet)
        {

           // DateTime d =  DateTime.Parse("1997-10-12 11:10:12");
            //String de = d.ToShortDateString();
            //try
            //{
            //    if(meet.StartTime!=null)
            //        DateTime.Parse(meet.StartTime.ToString());
            //    if (meet.EndTime != null)
            //        DateTime.Parse(meet.EndTime.ToString());
            //}catch(Exception e){
            //    return Content("no:时间格式不对");
            //}
            DateTime d = DateTime.Parse("0001-1-1 0:0:0");
            if (meet.StartTime==null || meet.StartTime == d)
            {
                return Content("no:开始时间不能为空或格式不对");
            }
            else
            {
                if (DateTime.Compare(DateTime.Parse(meet.StartTime.ToString()), DateTime.Now) < 0)
                {
                    return Content("no:开始时间不能为过去时间");
                }
            }
            if (meet.EndTime == null || meet.EndTime == d)
            {
                return Content("no:结束时间不能为空或格式不对");
            }
            else
            {
                if (DateTime.Compare(DateTime.Parse(meet.EndTime.ToString()), DateTime.Now) < 0)
                {
                    return Content("no:结束时间不能为过去时间");
                }
                if (meet.StartTime!=null && DateTime.Compare(DateTime.Parse(meet.EndTime.ToString()), DateTime.Parse(meet.StartTime.ToString())) < 0)
                {
                    return Content("no:结束时间不能小于开始时间");
                }
            }



            //0申请
            meet.Status = 0;
            int mid = int.Parse(meet.Other);
            MeetRoom meetRoom = meetRoomService.GetModels(p => p.MeetRoomId == mid).FirstOrDefault();

            //显示加载
            Meet updateMeet = meetService.GetModels(p => p.MeetId == meet.MeetId).FirstOrDefault();
            String[] dates = new String[0];
            if (!String.IsNullOrEmpty(meetRoom.UseTime))
            {
               dates = meetRoom.UseTime.Split('|');
            }
            String str = "";
            bool isOk = true;
            foreach (String date in dates)
            {
                String[] startend = date.Split(',');
                if (updateMeet != null && startend[0]==updateMeet.MeetId.ToString())
                {
                    //是更改时间,移去更改的时间
                }
                else{
                    if (DateTime.Compare(DateTime.Now, DateTime.Parse(startend[2])) > 0)
                    {
                        //移去 时间过了
                    }
                    else
                    {

                        if (String.IsNullOrEmpty(str))
                        {
                            str += date;
                        }
                        else
                        {
                            str += "|" + date;

                        }
                        bool b1 = DateTime.Compare(DateTime.Parse(meet.StartTime.ToString()), DateTime.Parse(startend[1])) >= 0;
                        bool b2 = DateTime.Compare(DateTime.Parse(meet.StartTime.ToString()), DateTime.Parse(startend[2])) <= 0;
                        bool b3 = DateTime.Compare(DateTime.Parse(meet.EndTime.ToString()), DateTime.Parse(startend[1])) >= 0;
                        bool b4 = DateTime.Compare(DateTime.Parse(meet.EndTime.ToString()), DateTime.Parse(startend[2])) <= 0;
                        if(b1 && b2 ||b3&&b4 || b2 && b3){
                            isOk = false;
                            break;
                        }
                        //if (DateTime.Compare(DateTime.Parse(meet.StartTime.ToString()), DateTime.Parse(startend[1])) >= 0 && DateTime.Compare(DateTime.Parse(meet.StartTime.ToString()), DateTime.Parse(startend[2])) <= 0
                        //|| DateTime.Compare(DateTime.Parse(meet.EndTime.ToString()), DateTime.Parse(startend[1])) >= 0 && DateTime.Compare(DateTime.Parse(meet.EndTime.ToString()), DateTime.Parse(startend[2])) <= 0
                        //|| DateTime.Compare(DateTime.Parse(meet.StartTime.ToString()), DateTime.Parse(startend[2])) <= 0 && DateTime.Compare(DateTime.Parse(meet.EndTime.ToString()), DateTime.Parse(startend[1])) >= 0 )
                        //{
                        //    isOk = false;
                        //    break;
                        //}
                    }
                }
                
            }
            if (isOk == false)
                return Content("no:会议室时间冲突");
            if (meet.MeetId != 0)
            {


                //显示加载
                if (meet.MeetMsg != null)
                {
                    int mmmid = meet.MeetMsg.MeetMsgId;
                }
                if (meet.Examine != null)
                {
                    int eid = meet.Examine.ExamineId;
                } 
                if (meet.MeetRoom != null)
                {
                    int mmid = meet.MeetRoom.MeetRoomId;
                }

                updateMeet.StartTime = meet.StartTime;
                updateMeet.EndTime = meet.EndTime;
                updateMeet.Title = meet.Title;
                updateMeet.Content = meet.Content;
                updateMeet.Type = meet.Type;
                updateMeet.Budget = meet.Budget;
                updateMeet.HostPeople = meet.HostPeople;
                updateMeet.Address = meet.Address;
                if (updateMeet.Title != null && meetService.Update(updateMeet))
                {
                    if (String.IsNullOrEmpty(str))
                    {
                        str += meet.MeetId + "," + meet.StartTime + "," + meet.EndTime;
                    }
                    else
                    {
                        str += "|" + meet.MeetId + "," + meet.StartTime + "," + meet.EndTime;
                    }
                    meetRoom.UseTime = str;
                    bool res = meetRoomService.Update(meetRoom);
                    return Content("ok:申请成功");
                }
                else
                {
                    return Content("no:申请失败");
                }
            }
            else
            {
                
                if (meet.Title != null && meetService.Add(meet))
                {
                    if (String.IsNullOrEmpty(str))
                    {
                        str += meet.MeetId + "," + meet.StartTime + "," + meet.EndTime;
                    }
                    else
                    {
                        str += "|" + meet.MeetId + "," + meet.StartTime + "," + meet.EndTime;
                    }
                    meetRoom.UseTime = str;
                    bool res = meetRoomService.Update(meetRoom);
                    return Content("ok");
                }
                else
                {
                    return Content("no");
                }
            }
            
        }
        public ActionResult FileUpload() {
            //获取不到文件
            HttpPostedFileBase postFile = Request.Files["fileUp"];
            String Title = Request["Title"];
            if (postFile == null)
            {
                return Content("no:请选择上传文件");
            }
            else
            {
                string fileName = Path.GetFileName(postFile.FileName);
                string fileExt = Path.GetExtension(fileName);
                if (fileExt == ".exe")
                {
                    return Content("no:上传文件格式不对");

                }
                else
                {
                    string dir = "/ImagePath/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";
                    Directory.CreateDirectory(Path.GetDirectoryName(Request.MapPath(dir)));
                    string newFileName = Guid.NewGuid().ToString();
                    string fullDir = dir + newFileName + fileExt;
                    postFile.SaveAs(Request.MapPath(fullDir));
                    return Content("ok:"+fullDir);
                }
            }
        }

        public ActionResult Delete()
        {
            int id = int.Parse(Request["id"]);
            Meet meet = meetService.GetModels(p => p.MeetId == id).FirstOrDefault();
            int mid = meet.MeetId;
            //meet没有meetRoom的引用
            //int rid = meet.MeetRoom.MeetRoomId==null?0:meet.MeetRoom.MeetRoomId;
            int rid = int.Parse(meet.Other);
            MeetRoom meetRoom = meetRoomService.GetModels(p => p.MeetRoomId == rid).FirstOrDefault();
            if (meetService.Delete(meet))
            {
                String[] dates = new String[0];
                if(meetRoom!=null && meetRoom.UseTime!=null){
                   dates = meetRoom.UseTime.Split('|');
                }
                String str = "";
                foreach (String date in dates)
                {
                    String[] startend = date.Split(',');
                    if (startend[0] == mid.ToString())
                    {
                        //移去 删除了
                    }
                    else
                    {

                        if (String.IsNullOrEmpty(str))
                        {
                            str += date;
                        }
                        else
                        {
                            str += "|" + date;
                        }
                    }
                }
                meetRoom.UseTime = str;
                bool res = meetRoomService.Update(meetRoom);
                return Content("ok");
            }
            else
            {
                return Content("no");
            }
            
        }

        public ActionResult Update(Meet meet)
        {

            DateTime d = DateTime.Parse("0001-1-1 0:0:0");
            if (meet.StartTime == d)
            {
                meet.StartTime = DateTime.Now;
            }
            if (meet.EndTime == d)
            {
                meet.EndTime = DateTime.Now;
            }
            meet.IsEnd = false;
            //0申请
            meet.Status = 0;
            if (meet.MeetId != 0)
            {

                //显示加载
                Meet updateMeet = meetService.GetModels(p => p.MeetId == meet.MeetId).FirstOrDefault();

                //显示加载
                if (meet.MeetMsg != null)
                {
                    int mid = meet.MeetMsg.MeetMsgId;
                }
                if (meet.Examine != null)
                {
                    int eid = meet.Examine.ExamineId;
                }
                updateMeet.StartTime = meet.StartTime;
                updateMeet.EndTime = meet.EndTime;
                updateMeet.Title = meet.Title;
                updateMeet.Content = meet.Content;
                updateMeet.Type = meet.Type;
                updateMeet.Budget = meet.Budget;
                updateMeet.HostPeople = meet.HostPeople;
                updateMeet.Address = meet.Address;
                if (updateMeet.Title != null && meetService.Update(updateMeet))
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
                return Content("no");
            }
        }
        
        #endregion




        //审核
        #region 会议审核
        [Authorize(Roles = "examine,admin")]
        public ActionResult ShowExamine()
        {
            string id = Request.Params["id"];
            int iid = int.Parse(id);
            ViewData["id"] = id;
            Meet meet = meetService.GetModels(p => p.MeetId == iid).FirstOrDefault();
            ViewData["meet"] = meet;
            ViewData["Title"] = meet.Title;
            ViewData["Type"] = meet.Type;
            ViewData["HostPeople"] = meet.HostPeople;
            ViewData["Budget"] = meet.Budget;
            ViewData["Address"] = meet.Address;
            ViewData["StartTime"] = meet.StartTime;
            ViewData["EndTime"] = meet.EndTime;
            ViewData["Content"] = meet.Content;
            return View();
        }
        [Authorize(Roles = "examine,admin")]
        [ValidateInput(false)]
        public ActionResult Examine(Examine examine)
        {
            int id = int.Parse(examine.Other);
            Meet meet = meetService.GetModels(p => p.MeetId == id).FirstOrDefault();
            //审核完毕 1:审核通过 -1：审核未通过
            meet.Status = int.Parse(examine.Roles);
            examine.Status = int.Parse(examine.Roles);
            examine.Meet = meet;
            examine.Time = DateTime.Now;
            if (examine.Content != null && examineService.Add(examine))
            {
                meet.Examine = examine;
                if (meet.MeetMsg != null)
                {
                    int mid = meet.MeetMsg.MeetMsgId;
                }
                meetService.Update(meet);
                return Content("ok");
            }
            else
            {
                return Content("no");
            }
        }
        [Authorize(Roles = "examine,admin")]
        public ActionResult MeetExamine()
        {
            ////未审核状态0
            //List<Meet> meets = meetService.GetModels(p => p.Status==0).ToList();
            ////未审核状态0
            //ViewData["meets"] = meets;
            ////有些复杂，需要改进
            //string id = Request.Params["id"];
            //List<Examine> examines = examineService.GetModels(p => p.ExaminePeople == id).ToList();
            //List<Meet> meetsok = new List<Meet>();
            //for(int i=0;i<examines.Count;i++){
            //    int t = examines[i].ExamineId;
            //    meetsok.Add(meetService.GetModels(p => p.MeetId==t).FirstOrDefault());
            //}
            //审核状态1
            //ViewData["meetsok"] = meetsok;

            string id = Request.Params["id"];
            int pageSize = 5;
            int itemCount = meetService.GetModels(p => p.Status == 0).Count();
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<Meet> meets = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                meets = meetService.GetModelsByPage(pageSize,pageIndex,true,p=>p.StartTime,p => p.Status == 0).ToList();

            }
            ViewData["meets"] = meets;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            if (Request.Params["id"] != null)
            {
                ViewData["id"] = id;
            }
            return View();
        }

        public ActionResult ExaminedMeet()
        {
            string id = Request.Params["id"];
            int pageSize = 5;
            int itemCount1 = meetService.GetModels(p => p.Status != 0 && p.Examine.ExaminePeople == id).Count();
            int pageCount1 = Convert.ToInt32(Math.Ceiling((double)itemCount1 / pageSize));
            //当前页码值
            int pageIndex1 = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex1 = pageIndex1 > pageCount1 ? pageCount1 : pageIndex1;
            List<Meet> meetsok = null;
            if (itemCount1 != 0 && pageIndex1 != 0)
            {
                meetsok = meetService.GetModelsByPage(pageSize, pageIndex1, true, p => p.StartTime, p => p.Status != 0 && p.Examine.ExaminePeople == id).ToList();

            }
            ViewData["meetsok"] = meetsok;
            ViewData["pageCount1"] = pageCount1;
            ViewData["pageIndex1"] = pageIndex1;
            if (Request.Params["id"] != null)
            {
                ViewData["id"] = id;
            }
            return View();
        }

        public ActionResult DeleteExamine()
        {
            int id = int.Parse(Request["id"]);
            Examine examine = examineService.GetModels(p => p.ExamineId == id).FirstOrDefault();
            if (examineService.Delete(examine))
            {
                //更改meetStatus
                Meet meet = meetService.GetModels(p => p.MeetId == id).FirstOrDefault();
                meet.Status = 0;
                bool r = meetService.Update(meet);
                return Content("ok");
            }
            else
            {
                return Content("no");
            }
        }

        public ActionResult MeetExamineDetail()
        {
            int id = int.Parse(Request["id"]);
            Examine examine = examineService.GetModels(p => p.ExamineId == id).FirstOrDefault();
            examine.Meet = null;
            return Json(examine, JsonRequestBehavior.AllowGet);
        }
        

        #endregion




        #region 会议通知
        public ActionResult MeetNotice()
        {
            //List<Meet> meets = meetService.GetModels(p => true).ToList();
            //List<Meet> meetsok = new List<Meet>();
            //int min = 10;
            //Meet meet = new Meet();
            //for (int i = 0; i < meets.Count; i++)
            //{
            //    TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            //    DateTime d = (DateTime)meets[i].StartTime;
            //    TimeSpan ts2 = new TimeSpan(d.Ticks);
            //    TimeSpan ts = ts1.Subtract(ts2).Duration();
            //    if (ts.Days < min && DateTime.Compare(d, DateTime.Now) > 0 && meets[i].Status >= 1)
            //    {
            //        min=ts.Days;
            //        meet = meets[i];
            //    }
            //    if (meets[i].Status >= 1 && ts.Days < 7 && DateTime.Compare(d, DateTime.Now) > 0)
            //    {
            //        meetsok.Add(meets[i]);
            //    }
            //}
            string id = Request.Params["id"];
            int pageSize = 5;
            //int itemCount = meetService.GetModels(p => DateTime.Compare((DateTime)p.StartTime, DateTime.Now) > 0 && p.Examine!=null).Count();
            int itemCount = meetService.GetModels(p => DateTime.Compare((DateTime)p.StartTime, DateTime.Now) > 0 && p.Status >= 1).Count();
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<Meet> meets = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                meets = meetService.GetModelsByPage(pageSize, pageIndex, true, p => p.StartTime,
                    p => DateTime.Compare((DateTime)p.StartTime, DateTime.Now) > 0 && p.Status >= 1).ToList();

            }
            Meet meet = meets == null ? new Meet() : meets[0];
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            ViewData["meets"] = meets;
            ViewData["meet"] = meet;
            ViewData["Title"] = meet.Title;
            ViewData["Type"] = meet.Type;
            ViewData["HostPeople"] = meet.HostPeople;
            ViewData["Budget"] = meet.Budget;
            int xid = Convert.ToInt32(meet.Other);
            MeetRoom meetRoom = meetRoomService.GetModels(p => p.MeetRoomId == xid).FirstOrDefault();
            ViewData["Address"] = meetRoom.Name + ":" + meetRoom.Address;
            ViewData["StartTime"] = meet.StartTime;
            ViewData["EndTime"] = meet.EndTime;
            ViewData["Content"] = meet.Content;
            if (Request.Params["id"] != null)
            {
                ViewData["id"] = id;
            }
            return View();
        }
        #endregion





        #region 会议纪要
        [Authorize(Roles = "memo,admin")]
        public ActionResult ShowMemo()
        {
            string id = Request.Params["id"];
            int iid = int.Parse(id);
            ViewData["id"] = id;
            Meet meet = meetService.GetModels(p => p.MeetId == iid).FirstOrDefault();
            MeetMsg meetMsg = meetMsgService.GetModels(p => p.MeetMsgId == iid).FirstOrDefault();
            ViewData["meet"] = meet;
            if (meetMsg != null)
            {
                ViewData["meetMsg"] = meetMsg;
                ViewData["Name"] = meetMsg.Name;
                ViewData["Content"] = meetMsg.Content;
                ViewData["MeetMsgId"] = meetMsg.MeetMsgId;
            }
           return View();
        }
        [Authorize(Roles = "memo,admin")]
        [ValidateInput(false)]
        public ActionResult Memo(MeetMsg meetMsg)
        {
            meetMsg.Status = 2;
            int id = int.Parse(meetMsg.Other);
            Meet meet = meetService.GetModels(p => p.MeetId == id).FirstOrDefault();
            //2:未归档 
            //显示加载
            if (meet.MeetMsg != null)
            {
                int mid = meet.MeetMsg.MeetMsgId;
            }
            if (meet.Examine != null)
            {
                int eid = meet.Examine.ExamineId;
            }
            meet.Status = 2;
            meetMsg.Meet = meet;
            meetMsg.Time = DateTime.Now;
            if (meetMsg.MeetMsgId == 0)
            {
                if (meetMsg != null && meetMsg.Content != null)
                {
                    if (meetMsgService.Add(meetMsg))
                    {
                        meet.MeetMsg = meetMsg;
                        meetService.Update(meet);
                        return Content("ok");
                    }
                }
                return Content("no");
            }
            else
            {
                //更新纪要
                MeetMsg updateMeetMsg = meetMsgService.GetModels(p => p.MeetMsgId == meetMsg.MeetMsgId).FirstOrDefault();
                Meet m = meetService.GetModels(p => p.MeetId == meetMsg.MeetMsgId).FirstOrDefault();
                updateMeetMsg.Meet = m;
                updateMeetMsg.Name = meetMsg.Name;
                updateMeetMsg.Content = meetMsg.Content;
                if (meetMsgService.Update(updateMeetMsg))
                {
                    return Content("ok");
                }
                else
                {
                    return Content("no");
                }
            }
           
        }
        [Authorize(Roles = "memo,admin")]
        public ActionResult MeetMemo()
        {
            //List<Meet> meets = meetService.GetModels(p => p.Status >= 1).ToList();
            //List<Meet> meetsok = new List<Meet>();
            ////ViewData["meets"] = meets;
            //for (int i = 0; i < meets.Count; i++)
            //{
            //    DateTime d = (DateTime)meets[i].StartTime;
            //    if (DateTime.Compare(DateTime.Now, d) > 0 && meets[i].Status>=1)
            //    {
            //        meetsok.Add(meets[i]);
            //    }
            //}
            //ViewData["meets"] = meetsok;
            //string id = Request.Params["id"];
            //List<MeetMsg> meetMsgs = meetMsgService.GetModels(p => p.Recorder == id).ToList();
            //List<Meet> meetsok1 = new List<Meet>();
            //for (int i = 0; i < meetMsgs.Count; i++)
            //{
            //    int t = meetMsgs[i].MeetMsgId;
            //    meetsok1.Add(meetService.GetModels(p => p.MeetId == t).FirstOrDefault());
            //}

            //ViewData["meetsok"] = meetsok1;
            string id = Request.Params["id"];
            int pageSize = 5;
            //需要纪要会议 时间 && Status=1
            int itemCount = meetService.GetModels(p => p.Status == 1 && DateTime.Compare(DateTime.Now, (DateTime)p.StartTime) > 0).Count();
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<Meet> meets = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                meets = meetService.GetModelsByPage(pageSize, pageIndex, true, p => p.StartTime,
                    p => p.Status == 1 && DateTime.Compare(DateTime.Now, (DateTime)p.StartTime) > 0).ToList();

            }
            ViewData["meets"] = meets;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            if (Request.Params["id"] != null)
            {
                ViewData["id"] = id;
            }
            return View();
        }

        public ActionResult MemoedMeet()
        {
            string id = Request.Params["id"];
            int pageSize = 5;
            //已纪要会议 时间 && Status>=2 && id
            int itemCount = meetService.GetModels(p => p.Status >= 2 && DateTime.Compare(DateTime.Now, (DateTime)p.StartTime) > 0 && p.MeetMsg.Recorder==id).Count();
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<Meet> meets = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                meets = meetService.GetModelsByPage(pageSize, pageIndex, true, p => p.StartTime,
                    p => p.Status >= 2 && DateTime.Compare(DateTime.Now, (DateTime)p.StartTime) > 0 && p.MeetMsg.Recorder == id).ToList();

            }
            ViewData["meetsok"] = meets;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            if (Request.Params["id"] != null)
            {
                ViewData["id"] = id;
            }
            return View();
        }
        public ActionResult MeetMemoDetail()
        {
            int id = int.Parse(Request["id"]);
            MeetMsg meetMsg = meetMsgService.GetModels(p => p.MeetMsgId == id).FirstOrDefault();
            meetMsg.Meet = null;
            return Json(meetMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMsg()
        {
            int id = int.Parse(Request["id"]);
            MeetMsg meetmsg = meetMsgService.GetModels(p => p.MeetMsgId == id).FirstOrDefault();
            if (meetMsgService.Delete(meetmsg))
            {
                //更改meetStatus
                //&& p.Examine.ExamineId==id meet延迟加载 使Examine加载出来
                Meet meet = meetService.GetModels(p => p.MeetId == id).FirstOrDefault();
                //显示加载
                if (meet.MeetMsg != null)
                {
                    int mid = meet.MeetMsg.MeetMsgId;
                    meet.MeetMsg.Meet = null;
                }
                if (meet.Examine != null)
                {
                    int eid = meet.Examine.ExamineId;
                    meet.Examine.Meet = null;
                }
                
                meet.Status = 1;
                Examine e = examineService.GetModels(p=>p.ExamineId==meet.MeetId).FirstOrDefault();
                meet.Examine = e;
                bool r = meetService.Update(meet);
                return Content("ok");
            }
            else
            {
                return Content("no");
            }
        }

        #endregion




        #region 会议归档
        [Authorize(Roles = "admin")]
        public ActionResult Save()
        {
            //归档成功
            int id = int.Parse(Request["id"]);
            //显示加载 meetMSg Examine
            Meet meet = meetService.GetModels(p => p.MeetId == id).FirstOrDefault();
            //显示加载
            if (meet.MeetMsg != null)
            {
                int mid = meet.MeetMsg.MeetMsgId;
            }
            if (meet.Examine != null)
            {
                int eid = meet.Examine.ExamineId;
            }
            
            meet.Status=4;
            MeetMsg meetMsg = meetMsgService.GetModels(p => p.MeetMsgId == id).FirstOrDefault();
            if (meetMsg != null)
            {
                meetMsg.Status = 4;
                //报不允许添加含有deleted状态的实体的关系
                //meetMsg.Meet = meet;
                //显示加载
                if (meetMsgService.Update(meetMsg))
                {
                    meet.MeetMsg = meetMsg;
                    meetService.Update(meet);
                }
                return Content("ok");
            }
            else
            {
                return Content("no");
            }
        }
        [Authorize(Roles = "admin")]
        public ActionResult Pass()
        {
            //丢弃
            int id = int.Parse(Request["id"]);
            //显示加载
            Meet meet = meetService.GetModels(p => p.MeetId == id).FirstOrDefault();
            //显示加载
            if (meet.MeetMsg != null)
            {
                int mid = meet.MeetMsg.MeetMsgId;
            }
            if (meet.Examine != null)
            {
                int eid = meet.Examine.ExamineId;
            }
            meet.Status = 3;
            MeetMsg meetMsg = meetMsgService.GetModels(p => p.MeetMsgId == id).FirstOrDefault();
            if (meetMsg != null)
            {
                meetMsg.Status = 3;
                //显示加载
                meetMsg.Meet = meet;
                if (meetMsgService.Update(meetMsg))
                {
                    meet.MeetMsg = meetMsg;
                    meetService.Update(meet);
                }
                return Content("ok");
            }
            else
            {
                return Content("ok");
            }
        }
        [Authorize(Roles = "admin")]
        public ActionResult MeetSave()
        {
            //未归档
            //List<Meet> meets = meetService.GetModels(p => p.Status == 2).ToList();
            //ViewData["meets"] = meets;
            ////归档
            //List<Meet> meetsok = meetService.GetModels(p => p.Status >= 3).ToList();
            //ViewData["meetsok"] = meetsok;
            //List<MeetMsg> meetMsgs = meetMsgService.GetModels(p=>p.Status==2).ToList();
            //ViewData["meetMsgs"] = meetMsgs;
            //List<MeetMsg> meetMsgsok = meetMsgService.GetModels(p => p.Status >= 3).ToList();
            //ViewData["meetMsgsok"] = meetMsgsok;
            string id = Request.Params["id"];
            int pageSize = 5;
            //需要归档会议 meet.Status == 2
            int itemCount = meetMsgService.GetModels(p => p.Meet.Status == 2).Count();
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<MeetMsg> meetMsgs = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                meetMsgs = meetMsgService.GetModelsByPage(pageSize, pageIndex, true, p => p.Name,
                    p => p.Meet.Status == 2).ToList();

            }
            ViewData["meetMsgs"] = meetMsgs;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            if (Request.Params["id"] != null)
            {
                ViewData["id"] = id;
            }
            return View();
        }

        public ActionResult SavedMeet()
        {
            int pageSize = 5;
            string id = Request.Params["id"];
            //已归档会议 meet.Status > 2 meetmsg.Recorder==id
            int itemCount = meetMsgService.GetModels(p => p.Meet.Status > 2).Count();
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<MeetMsg> meetMsgs = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                meetMsgs = meetMsgService.GetModelsByPage(pageSize, pageIndex, true, p => p.Name,
                    p => p.Meet.Status > 2).ToList();

            }
            ViewData["meetMsgsok"] = meetMsgs;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            if (Request.Params["id"] != null)
            {
                ViewData["id"] = id;
            }
            return View();
        }

        #endregion



        #region 会议查询
        public ActionResult Index(string meetType,string searchString)
        {
            //List<Meet> meets = meetService.GetModels(p => true).ToList();
            //ViewData["meets"] = meets;
            int pageSize = 5;
            int itemCount;
            //List<Meet> meetList = meetService.GetModels(p => true).ToList();
            //var meetTypeList = meetList.Select(p => new { p.Type }).Distinct();
            //ViewBag.meetType = new SelectList(meetTypeList.AsEnumerable().ToList());
            //if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(meetType))
            //{
            //    itemCount = meetService.GetModels(p => p.Type==meetType && p.Title.Contains(searchString) || p.Content.Contains(searchString)).Count();
            //}
            //else if (!String.IsNullOrEmpty(meetType))
            //{
            //    itemCount = meetService.GetModels(p => p.Type==meetType).Count();
            //}
            if (!String.IsNullOrEmpty(searchString))
            {
                itemCount = meetService.GetModels(p => p.Title.Contains(searchString) || p.Content.Contains(searchString)).Count();
            }
            else
            {
                itemCount = meetService.GetModels(p => true).Count();
            }
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<Meet> meets = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                //if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(meetType))
                //{
                //    meets = meetService.GetModelsByPage<Nullable<DateTime>>(pageSize, pageIndex, true, a => a.StartTime, p => p.Type == meetType && p.Title.Contains(searchString) || p.Content.Contains(searchString)).ToList();
                //}
                //else if (!String.IsNullOrEmpty(meetType))
                //{
                //    meets = meetService.GetModelsByPage<Nullable<DateTime>>(pageSize, pageIndex, true, a => a.StartTime, p => p.Type==meetType).ToList();
                //}
                if(!String.IsNullOrEmpty(searchString)){
                    meets = meetService.GetModelsByPage<Nullable<DateTime>>(pageSize, pageIndex, true, a => a.StartTime, p => p.Title.Contains(searchString) || p.Content.Contains(searchString)).ToList();
                }else{
                    meets = meetService.GetModelsByPage<Nullable<DateTime>>(pageSize, pageIndex, true, a => a.StartTime, p => true).ToList();
                }
            } 
            ViewData["meets"] = meets;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            ViewData["searchString"] = searchString;
            ViewData["meetType"] = meetType;
            return View();
        }

        

        public ActionResult OccurMeetQuery(string searchString)
        {
            //int itemCount = meetService.GetModels(p =>
            //{
            //    if (p.Status >= 1 && DateTime.Compare((DateTime)p.StartTime, DateTime.Now) < 0)
            //        return true;
            //    return false;
            //}).Count();
            int itemCount = 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                itemCount = meetService.GetModels(p => p.Status >= 1 && DateTime.Compare((DateTime)p.StartTime, DateTime.Now) < 0 && (p.Content.Contains(searchString) || p.Title.Contains(searchString))).Count();
            }
            else
            {
                itemCount = meetService.GetModels(p => p.Status >= 1 && DateTime.Compare((DateTime)p.StartTime, DateTime.Now) < 0).Count();
            }
            int pageSize = 5;
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<Meet> meets = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    meets = meetService.GetModelsByPage(pageSize, pageIndex, true, p => p.StartTime,
                     p => p.Status >= 1 && DateTime.Compare((DateTime)p.StartTime, DateTime.Now) < 0 && (p.Content.Contains(searchString) || p.Title.Contains(searchString))).ToList();
                }
                else
                {
                    meets = meetService.GetModelsByPage(pageSize, pageIndex, true, p => p.StartTime,
                     p => p.Status >= 1 && DateTime.Compare((DateTime)p.StartTime, DateTime.Now) < 0).ToList();
                }
                
            }
            ViewData["meets"] = meets;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            ViewData["meets"] = meets;
            ViewData["searchString"] = searchString;
            return View();
        }

        public ActionResult WaitMeetQuery(string searchString)
        {
            int itemCount = 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                itemCount = meetService.GetModels(p => p.Status >= 1 && DateTime.Compare((DateTime)p.StartTime, DateTime.Now) > 0 && (p.Content.Contains(searchString) || p.Title.Contains(searchString))).Count();
            }
            else
            {
                itemCount = meetService.GetModels(p => p.Status >= 1 && DateTime.Compare((DateTime)p.StartTime, DateTime.Now) > 0).Count();
            }
            
            
            int pageSize = 5;
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<Meet> meets = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    meets = meetService.GetModelsByPage(pageSize, pageIndex, true, p => p.StartTime,
                    p => p.Status >= 1 && DateTime.Compare((DateTime)p.StartTime, DateTime.Now) > 0 && (p.Content.Contains(searchString) || p.Title.Contains(searchString))).ToList();
                }
                else
                {
                    meets = meetService.GetModelsByPage(pageSize, pageIndex, true, p => p.StartTime,
                    p => p.Status >= 1 && DateTime.Compare((DateTime)p.StartTime, DateTime.Now) > 0).ToList();
                }
                
            }
            
            ViewData["meets"] = meets;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            ViewData["searchString"] = searchString;
            return View();
        }
        public ActionResult MeetMsg(string searchString)
        {
            int itemCount = 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                itemCount = meetMsgService.GetModels(p => p.Content.Contains(searchString) || p.Name.Contains(searchString)).Count();
            }
            else
            {
                itemCount = meetMsgService.GetModels(p => true).Count();
            }
            int pageSize = 5;
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<MeetMsg> meetMsgs = null;
            if (itemCount != 0 && pageIndex != 0)
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    meetMsgs = meetMsgService.GetModelsByPage(pageSize, pageIndex, true, p => p.Name, p => p.Content.Contains(searchString) || p.Name.Contains(searchString)).ToList();
                }
                else
                {
                    meetMsgs = meetMsgService.GetModelsByPage(pageSize, pageIndex, true, p => p.Name, p => true).ToList();
                }

            }
            ViewData["meetMsgs"] = meetMsgs;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            ViewData["searchString"] = searchString;
            //List<MeetMsg> meetMsgs = meetMsgService.GetModels(p => true).ToList();
            //ViewData["meetMsgs"] = meetMsgs;
            return View();
        }

        public ActionResult ExamineQuery(string searchString)
        {
            //List<Examine> examines = examineService.GetModels(p=>true).ToList();
            //ViewData["examines"] = examines;
            int itemCount = 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                itemCount = examineService.GetModels(p => p.Content.Contains(searchString)).Count();
            }
            else
            {
                itemCount = examineService.GetModels(p => true).Count();
            }
            int pageSize = 5;
            int pageCount = Convert.ToInt32(Math.Ceiling((double)itemCount / pageSize));
            //当前页码值
            int pageIndex = Request.Params["pageIndex"] == null ? 1 : int.Parse(Request.Params["pageIndex"]);
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            List<Examine> examines = null;
            if(itemCount != 0 && pageIndex != 0){
                if (!String.IsNullOrEmpty(searchString))
                {
                    examines = examineService.GetModelsByPage(pageSize, pageIndex, true, p => p.Time, p => p.Content.Contains(searchString)).ToList();
                }
                else
                {
                    examines = examineService.GetModelsByPage(pageSize, pageIndex, true, p => p.Time, p => true).ToList();
                }
                
            }
            ViewData["examines"] = examines;
            ViewData["pageCount"] = pageCount;
            ViewData["pageIndex"] = pageIndex;
            ViewData["searchString"] = searchString;
            return View();
        }
        
        public ActionResult MeetDetail()
        {
            int id = int.Parse(Request["id"]);
            Meet meet = meetService.GetModels(p => p.MeetId == id).FirstOrDefault();
            meet.Examine = null;
            meet.MeetMsg = null;
            meet.MeetRoom = null;

            int xid = Convert.ToInt32(meet.Other);
            MeetRoom meetRoom = meetRoomService.GetModels(p => p.MeetRoomId == xid).FirstOrDefault();
            ViewData["Address"] = meetRoom.Name + ":" + meetRoom.Address;
            meet.Address = meetRoom.Name +":"+ meetRoom.Address;
            return Json(meet, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}