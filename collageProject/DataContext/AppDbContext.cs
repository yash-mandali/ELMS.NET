using Microsoft.EntityFrameworkCore;
using collageProject.Model;

namespace collageProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ================================
        // DB TABLES
        // ================================

        public DbSet<User> Users { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<EmailOtp> EmailOtps { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================
            // USER TABLE CONFIGURATION
            // =====================================

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.UserName)
                      .HasMaxLength(70);

                entity.Property(u => u.Email)
                      .HasMaxLength(150);

                entity.HasIndex(u => u.Email)
                      .IsUnique(); // prevent duplicate emails

                entity.Property(u => u.Role)
                      .HasMaxLength(50);
            });


            // =====================================
            // USER PROFILE CONFIGURATION
            // =====================================

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(p => p.ProfileId);

                entity.Property(p => p.FullName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(p => p.Email)
                      .HasMaxLength(150);

                entity.Property(p => p.PhoneNumber)
                      .HasMaxLength(15);

                entity.Property(p => p.Address)
                      .HasMaxLength(200);

                entity.Property(p => p.Gender)
                      .HasMaxLength(20);

                entity.Property(p => p.Department)
                      .HasMaxLength(100);

                entity.Property(p => p.Role)
                      .HasMaxLength(50);

                entity.Property(p => p.ProfileImage)
                      .HasMaxLength(250);

                // Default CreatedAt value
                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });


            // =====================================
            // EMAIL OTP CONFIGURATION
            // =====================================

            modelBuilder.Entity<EmailOtp>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.Email)
                      .IsRequired();

                entity.Property(o => o.OtpCode)
                      .IsRequired();
            });


            // =====================================
            // USER ↔ USERPROFILE RELATION (1-to-1)
            // =====================================

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Prevent duplicate profiles per user
            modelBuilder.Entity<UserProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();
        }
    }
}
