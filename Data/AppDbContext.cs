using EventsAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventsAPI.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<EvHeader> Events { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<Music> Musics { get; set; }
        public DbSet<Outlet> Outlets { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BoughtTicket> BoughtTickets { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admission>()
                .HasOne(a => a.EvHeader)
                .WithMany(e => e.Admissions)
                .HasForeignKey(a => a.EventListingId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

    }
}
