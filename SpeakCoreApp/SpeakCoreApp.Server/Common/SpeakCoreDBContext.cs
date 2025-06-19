using Microsoft.EntityFrameworkCore;
using speakcore.API.Model;

namespace speakcore.API.Common
{
    public class SpeakCoreDBContext : DbContext
    {
        public SpeakCoreDBContext(DbContextOptions<SpeakCoreDBContext> options)
       : base(options) { }

        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Registration>()
                .HasKey(r => r.Key);

            modelBuilder.Entity<Registration>()
                .HasIndex(r => r.Email)
                .IsUnique();

            modelBuilder.Entity<Registration>()
                .HasIndex(r => r.RegistrationId)
                .IsUnique();
        }
    }
}
