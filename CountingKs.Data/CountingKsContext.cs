using CountingKs.Data.Entities;
using System.Data.Entity;

namespace CountingKs.Data
{
    public class CountingKsContext : DbContext
    {
        public CountingKsContext()
          : base("DefaultConnection")
        {
            base.Configuration.LazyLoadingEnabled = false;
            base.Configuration.ProxyCreationEnabled = false;
        }

        static CountingKsContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<CountingKsContext, CountingKsMigrationConfiguration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            CountingKsMapping.Configure(modelBuilder);
        }

        public DbSet<ApiUser> ApiUsers { get; set; }
        public DbSet<AuthToken> AuthTokens { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Measure> Measures { get; set; }
        public DbSet<Diary> Diaries { get; set; }
        public DbSet<DiaryEntry> DiaryEntries { get; set; }
    }
}
