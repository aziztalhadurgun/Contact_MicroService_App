using Microsoft.EntityFrameworkCore;

namespace Contact.Report.DataAccess
{
    public class ReportDbContext: DbContext
    {
        public ReportDbContext()
        {
        }

        public ReportDbContext(DbContextOptions opt) : base(opt)
        {
        }

        public DbSet<Report> Reports{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }

        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>(b => { b.ToTable("report"); });
        }

    }
}
