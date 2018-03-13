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
    [Table("asp_examine")]
    public class Examine
    {
        [Key,ForeignKey("Meet")]
        public int ExamineId { get; set; }
        public String ExaminePeople { get; set; }
        public int Status { get; set; }
        public String Content { get; set; }

        public String Other { get; set; }
        public String Roles { get; set; }
        public Nullable<DateTime> Time { get; set; }
        [JsonIgnore]
        public virtual Meet Meet { get; set; }
    }
}
