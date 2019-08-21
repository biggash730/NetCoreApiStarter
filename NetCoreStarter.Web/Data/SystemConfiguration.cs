using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCoreStarter.Web.Data
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.Property(s => s.Name)
                .IsRequired(true);

            builder.HasData
            (
                new Role(GenericProperties.Administrator)
            );
        }
    }

    //public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    //{
    //    public void Configure(EntityTypeBuilder<Claim> builder)
    //    {
    //        builder.ToTable("Claims");
    //        builder.Property(s => s.Type)
    //            .IsRequired(true);

    //        builder.HasData
    //        (
    //            new Role(GenericProperties.Administrator)
    //        );
    //    }
    //}
    public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable("RoleClaims");
            builder.Property(s => s.RoleId)
                .IsRequired(true);            
            builder.Property(s => s.ClaimValue)
                .IsRequired(true);
            builder.Property(s => s.ClaimType)
                .HasDefaultValue(GenericProperties.Privilege);

            builder.HasData
            (
                new RoleClaim
                {
                    Id = 1,
                    RoleId = Guid.NewGuid().ToString(),
                    ClaimType = GenericProperties.Privilege,
                    ClaimValue = Privileges.CanManageRoles,
                }
            );
        }
    }
}
