using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public static class Common
    {
        public static User GetCurrentUser {
            get {
                UserRepository userRepo = new UserRepository();

                int userId = Convert.ToInt32(HttpContext.Current.Session["uid"]);
                User user = userRepo.GetById(userId);

                return user;
            }
        }
    }
}