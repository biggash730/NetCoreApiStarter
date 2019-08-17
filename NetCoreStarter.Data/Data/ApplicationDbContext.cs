using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCoreStarter.Data.Classes;

namespace NetCoreStarter.Data.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<LookUp> LookUps { get; set; }
        public DbSet<Profile> Profiles { get; set; }
    }
}
