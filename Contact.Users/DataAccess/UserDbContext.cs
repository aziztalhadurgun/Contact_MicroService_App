using Microsoft.EntityFrameworkCore;

namespace Contact.Users.DataAccess
{
    public class UserDbContext:DbContext
    {
        public UserDbContext()
        {
        }

        public UserDbContext(DbContextOptions opt) : base(opt)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ContactInformation> ContactInformations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }

        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b => { b.ToTable("user"); });
            modelBuilder.Entity<ContactInformation>(b => { b.ToTable("contactInformation"); });
        }

    }
}
