using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Model
{
    [Table("asp_meet")]
    public class Meet
    {
        [Key]
        public int MeetId { get; set; }
        public String Title { get; set; }
        //0申请未通过 1正在申请 2申请通过
        public String Type { get; set; }
        public int Status { get; set; }
        public String HostPeople { get; set; }
        public Double Budget { get; set; }
        public String Address { get; set; }
        [Column("start_time")]
        public Nullable<DateTime> StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String Content { get; set; }

        public String Participants { get; set; }
        public String Roles { get; set; }
        [MaxLength(256)]
        public String Other { get; set; }
        public String Picture { get; set; }
        public String File { get; set; }
        public bool IsEnd { get; set; }
        public String ApplyPeople { get; set; }

        [JsonIgnore]
        public virtual Examine Examine { get; set; }

        [JsonIgnore]
        public virtual MeetMsg MeetMsg { get; set; }

        public virtual MeetRoom MeetRoom { get; set; }
    }
}
