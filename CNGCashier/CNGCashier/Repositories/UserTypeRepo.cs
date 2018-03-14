using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNGCashier.Models
{
    public class UserTypeRepo
    {
        private CNGCashierDBContext context = new CNGCashierDBContext();

        public IQueryable<UserType> List()
        {
            return context.UserTypes;
        }
        public UserType GetById(int id)
        {
            UserType userType = context.UserTypes.FirstOrDefault(p => p.Id == id);

            return userType;
        }
    }
}