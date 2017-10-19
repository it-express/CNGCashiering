using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public User GetByCredentials(string username, string password) {
            User user = List().FirstOrDefault(p => p.Username == username && p.Password == password);

            //SqlParameter parameter1 = new SqlParameter("@CompanyID", Sessions.CompanyId);
            //var affectedRows = context.Database.ExecuteSqlCommand("sp_Update_Item_UnitCost @CompanyID", parameter1);
            var affectedRows1 = context.Database.ExecuteSqlCommand("spUpdate_Items_QuantityOnHand");

            return user;
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
                    dbEntry.UserLevel = user.UserLevel;
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

        public int GetByUserLevel(int userid)
        {
            int Userid = context.Users.FirstOrDefault(p => p.Id == userid).UserLevel;

            return Userid;
        }
    }
}