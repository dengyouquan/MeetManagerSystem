using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Runtime.Remoting.Messaging;

namespace DAL
{
    public class DbContextFactory
    {
        /// <summary>
        /// 创建EF上下文对象，已经存在就直接取，不存在就创建，保证线程内是唯一的
        /// </summary>
        /// <returns></returns>
        public static DbContext Create()
        {
            DbContext db = CallContext.GetData("db") as DbContext;
            if (db == null)
            {
                db = new MyDbContext();
                CallContext.SetData("db",db);
            }
            return db;
        }
    }
}
