using management.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace management
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
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
            modelBuilder.Entity<Transaction>().HasKey(t => t.code);
            modelBuilder.Entity<Transaction>()
            .Property(t => t.transaction_date)
            .HasColumnType("datetime");
            // Explicitly define primary keys for Identity tables
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(e => new { e.UserId, e.RoleId });
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(e => new { e.LoginProvider, e.ProviderKey }); // Fix for IdentityUserLogin
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
   
        }

    }
}
