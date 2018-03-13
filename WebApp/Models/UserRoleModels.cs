using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace WebApp.Models
{
    //public class UserRoleModels
    //{

    //}
    public class ApplicationUserRole : IdentityUserRole
    {
        public ApplicationUserRole() : base() { }
    }
}