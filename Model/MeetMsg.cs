using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
     [Table("asp_meetMsg")]
    public class MeetMsg
    {
         
         [Key,ForeignKey("Meet")]
         public int MeetMsgId { get; set; }
         public String Name { get; set; }
         public int Status { get; set; }

         public DateTime Time { get; set; }
         public String Content { get; set; }
         public String Recorder { get; set; }
         public String File { get; set; }
         public String Other { get; set; }
         public virtual Meet Meet { get; set; }
    }
}
