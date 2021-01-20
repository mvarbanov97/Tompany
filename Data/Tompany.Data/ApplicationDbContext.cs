﻿namespace Tompany.Data
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Tompany.Data.Common.Models;
    using Tompany.Data.Models;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Destination> Destinations { get; set; }

        public DbSet<State> States { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Trip> Trips { get; set; }

        public DbSet<UserTrip> UserTrips { get; set; }

        public DbSet<TripRequest> TripRequests { get; set; }

        public DbSet<ContactFormEntry> ContactFormEntries { get; set; }

        public DbSet<View> Views { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<UserGroup> UsersGroups { get; set; }

        public DbSet<UserRating> UserRatings { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<WatchListTrip> WatchListTrips { get; set; }

        public DbSet<UserNotification> UserNotifications { get; set; }

        public override int SaveChanges() => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            this.SaveChangesAsync(true, cancellationToken);

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<WatchListTrip>()
                .HasKey(x => new { x.UserId, x.TripId });

            builder.Entity<Message>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.UserId);

            builder.Entity<UserTrip>()
                .HasKey(x => new { x.UserId, x.TripId });

            builder.Entity<UserTrip>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserTrips)
                .HasForeignKey(x => x.UserId);

            builder.Entity<UserTrip>()
                .HasOne(x => x.Trip)
                .WithMany(x => x.UserTrips)
                .HasForeignKey(x => x.TripId);

            builder.Entity<ApplicationUser>()
                .HasMany(x => x.UserTrips)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.Entity<UserRating>().HasKey(x => new
            {
                x.Username,
                x.RaterUsername,
            });

            builder.Entity<UserGroup>().HasKey(k => new
            {
                k.GroupId,
                k.ApplicationUserId,
            });

            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            this.ConfigureUserIdentityRelations(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            var entityTypes = builder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { builder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        // Applies configurations
        private void ConfigureUserIdentityRelations(ModelBuilder builder)
             => builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}