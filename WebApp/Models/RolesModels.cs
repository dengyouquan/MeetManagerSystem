using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace WebApp.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }
        // 在此添加额外属性
        //public String r_id { get; set; }
    }

    public class ApplicationRoleDbContext : DbContext
    {
        public ApplicationRoleDbContext()
            : base("DefaultConnection")
        {
        }
    }
}