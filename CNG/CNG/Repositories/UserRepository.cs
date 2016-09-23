using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class UserRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<User> List()
        {
            return context.Users;
        }

        public User GetById(int id)
        {
            User user = context.Users.FirstOrDefault(p => p.Id == id);

            return user;
        }

        public void Save(User user)
        {
            if (user.Id == 0)
            {
                context.Users.Add(user);
            }
            else
            {
                User dbEntry = context.Users.Find(user.Id);
                if (dbEntry != null)
                {
                    dbEntry.Username = user.Username;
                    dbEntry.Password = user.Password;
                    dbEntry.FirstName = user.FirstName;
                    dbEntry.LastName = user.LastName;
                    dbEntry.UserTypeId = user.UserTypeId;
                    dbEntry.GeneralManagerId = user.GeneralManagerId;
                }
            }

            context.SaveChanges();
        }

        public void Delete(int id)
        {
            User user = context.Users.Find(id);

            context.Users.Remove(user);

            context.SaveChanges();
        }

        public List<User> GetByUserTypeId(int userTypeId) {
            List<User> lstUser = context.Users.Where(p => p.UserTypeId == userTypeId).ToList();

            return lstUser;
        }
    }
}