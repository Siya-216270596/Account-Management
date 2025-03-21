using management.Models;
using management.Models.Response;
using Microsoft.EntityFrameworkCore;


namespace management
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transaction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure unique constraints
            modelBuilder.Entity<Person>().HasIndex(p => p.id_number).IsUnique();
            modelBuilder.Entity<Account>().HasKey(a => a.code);
            modelBuilder.Entity<Person>().HasKey(p => p.Code);
            modelBuilder.Entity<Transaction>().HasKey(t => t.Code);
        }

    }
}
