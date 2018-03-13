using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Model;

namespace DAL
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyDbContext>());
        }
        public virtual DbSet<Meet> Meets { get; set; }
        public virtual DbSet<Examine> Examines { get; set; }
        public virtual DbSet<MeetMsg> MeetMsgs { get; set; }
        public virtual DbSet<MeetRoom> MeetRooms { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Fluent API配置
            base.OnModelCreating(modelBuilder);
        }
    }
}
