using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Model
{
     [Table("asp_meetRoom")]
    public class MeetRoom
    {
         [Key]
         public int MeetRoomId { get; set; }
         public String Name { get; set; }
         public int HoldPerson { get; set; }
         public int Status { get; set; }
         public String Content { get; set; }
         public String People { get; set; }
         public String UseTime { get; set; }
         public String Address { get; set; }
         public String Describe { get; set; }
         public bool isPrepare { get; set; }
         public bool isUsed { get; set; }
         public int Memo { get; set; }
         public String Picture { get; set; }
         public String Other { get; set; }
         public virtual ICollection<Meet> Meets { get; set; }
    }
}
