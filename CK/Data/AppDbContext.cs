using CK.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CK.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<StandardBooking> StandardBookings { get; set; }
    public DbSet<SuiteBooking> SuiteBookings { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Cấu hình TPT (Table-Per-Type)
        modelBuilder.Entity<Booking>().ToTable("Bookings");
        modelBuilder.Entity<StandardBooking>().ToTable("StandardBookings");
        modelBuilder.Entity<SuiteBooking>().ToTable("SuiteBookings");

        // 2. Global Query Filter: Mặc định ẩn các Booking đã bị xóa mềm
        modelBuilder.Entity<Booking>().HasQueryFilter(b => b.Status != "SoftDeleted");
    }
}