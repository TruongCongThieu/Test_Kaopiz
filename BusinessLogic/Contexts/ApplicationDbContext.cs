using BusinessLogic.Entities.Base;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Services.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Contexts
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
        : IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>, IdentityUserRole<string>,
            IdentityUserLogin<string>, AppRoleClaim, IdentityUserToken<string>>(options)
    {

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = dateTimeService.Now;
                        entry.Entity.CreatedBy = currentUserService.UserName ?? "SuperAdmin";
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = dateTimeService.Now;
                        entry.Entity.LastModifiedBy = currentUserService.UserName ?? "SuperAdmin";
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable(name: "Users", "Identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                // Add index to optimize queries
                entity.HasIndex(e => e.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                entity.HasIndex(e => e.NormalizedEmail).HasDatabaseName("EmailIndex");
            });

            builder.Entity<AppRole>(entity =>
            {
                entity.ToTable(name: "Roles", "Identity");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles", "Identity");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims", "Identity");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins", "Identity");
            });

            builder.Entity<AppRoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", "Identity");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", "Identity");
            });
        }
    }
}