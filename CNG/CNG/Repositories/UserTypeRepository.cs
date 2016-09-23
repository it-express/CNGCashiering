using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class UserTypeRepository
    {
        private CNGDBContext context = new CNGDBContext();

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