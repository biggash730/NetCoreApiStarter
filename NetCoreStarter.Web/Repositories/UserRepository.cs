
using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreStarter.Data.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public User Get(string username)
        {
            return DbSet.SingleOrDefault(x => x.UserName == username);
        }

        public override void Update(User entity)
        {
            var theUser = DbSet.Find(entity.Id);
            if (theUser == null) throw new Exception("User not found to update.");
            theUser.Surname = entity.Surname;
            theUser.OtherNames = entity.OtherNames;
            theUser.Email = entity.Email;
            theUser.PhoneNumber = entity.PhoneNumber;
            theUser.Email = entity.Email;

            SaveChanges();
        }

        public void Delete(string id)
        {
            var user = DbSet.Find(id);
            if (user.Locked) throw new Exception(ExceptionMessage.RecordLocked);
            DbSet.Remove(user);
            SaveChanges();
        }
    }
}
