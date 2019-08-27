

using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using NetCoreStarter.Web.Models;
using System;
using System.Linq;

namespace NetCoreStarter.Web.Repositories
{
    public class RoleRepository : BaseRepository<Role>
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override void Update(Role entity)
        {
            var theRole = DbSet.Find(entity.Id);
            theRole.Name = entity.Name;
            theRole.NormalizedName = entity.Name;

            //remove existing claims
            var existing = _context.RoleClaims.Where(x => x.RoleId == entity.Id);
            _context.RoleClaims.RemoveRange(existing);
            
            //add claims
            foreach(var c in entity.Claims)
            {
                _context.RoleClaims.Add(new RoleClaim
                {
                    RoleId = entity.Id,
                    ClaimType = GenericProperties.Privilege,
                    ClaimValue  = c
                });
            }
            SaveChanges();
        }

        public override void Insert(Role entity)
        {
            var existing = DbSet.Where(x=> x.Name == entity.Name).FirstOrDefault();
            if (existing != null)
            {
                this.Update(entity);
                return;
            }
            _context.Roles.Add(new Role
                {
                Name = entity.Name,
                NormalizedName = entity.Name
            });

            //add claims
            foreach (var c in entity.Claims)
            {
                _context.RoleClaims.Add(new RoleClaim
                {
                    RoleId = entity.Id,
                    ClaimType = GenericProperties.Privilege,
                    ClaimValue = c
                });
            }
            SaveChanges();
        }

        public Role GetByName(string name) 
        {
            return DbSet.Where(x => x.Name == name).FirstOrDefault(); 
        }

        public Role GetById(string id)
        {
            return DbSet.Where(x => x.Id == id).FirstOrDefault();
        }

        public void RemoveFromAllRoles(string userId)
        {
            var recs = _context.UserRoles.Where(x => x.UserId == userId);
            _context.UserRoles.RemoveRange(recs);
            SaveChanges();
        }

        public void AddToRole(string userId, string roleName)
        {
            var role = DbSet.Where(x => x.Name == roleName).FirstOrDefault();
            if(role == null) throw new Exception($"Could not fine the {role} role");
            var rec = _context.UserRoles.FirstOrDefault(x => x.UserId == userId && x.Role.Name == roleName);
            if (rec != null) throw new Exception("User already has the selected role");
            _context.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = role.Id
            });            
            SaveChanges();
        }
    }
}
