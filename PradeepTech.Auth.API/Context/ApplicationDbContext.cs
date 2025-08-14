using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PradeepTech.Auth.API.Models;

namespace PradeepTech.Auth.API.Context
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        string,
        IdentityUserClaim<string>,
        ApplicationUserRole,
        IdentityUserLogin<string>,
        ApplicationRoleClaim,
        IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.ForeignKeyPropertiesMappedToUnrelatedTables));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User-Role many-to-many relationship
            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            // Fix for UserClaim → ApplicationUser
            builder.Entity<UserClaim>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserClaims)
                .HasForeignKey(uc => uc.UserId)
                .HasPrincipalKey(u => u.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Fix for ApplicationRoleClaim → ApplicationRole
            builder.Entity<ApplicationRoleClaim>()
                .HasOne(rc => rc.Role)
                .WithMany(r => r.RoleClaims)
                .HasForeignKey(rc => rc.RoleId)
                .HasPrincipalKey(r => r.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Seed default roles
            SeedRoles(builder);

            // Configure table names (optional)
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<ApplicationUserRole>().ToTable("UserRoles");
            builder.Entity<UserClaim>().ToTable("UserClaims");
            builder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole
                {
                    Id = "1",
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    Description = "Full system access",
                    DateCreated = new DateTime(2023, 01, 01),
                    IsActive = true,
                },
                new ApplicationRole
                {
                    Id = "2",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Administrative access",
                    DateCreated = new DateTime(2023, 01, 01),
                    IsActive = true,
                },
                new ApplicationRole
                {
                    Id = "3",
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "Management level access",
                    DateCreated = new DateTime(2023, 01, 01),
                    IsActive = true,
                },
                new ApplicationRole
                {
                    Id = "4",
                    Name = "User",
                    NormalizedName = "USER",
                    Description = "Standard user access",
                    DateCreated = new DateTime(2023, 01, 01),
                    IsActive = true,
                }
            );
        }
    }
}