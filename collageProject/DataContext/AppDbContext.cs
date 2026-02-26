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

        public DbSet<User> Users { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<EmailOtp> EmailOtps { get; set; }
        public DbSet<Requests> LeaveRequests { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


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


            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(p => p.ProfileId);

                entity.Property(p => p.Employee_Id);
                entity.Property(p => p.Emergency_Phone);
                entity.Property(p => p.DateOfBirth);
                entity.Property(p => p.Designation);
                entity.Property(p => p.Work_Email);
                entity.Property(p => p.Employment_Type);
                entity.Property(p => p.Work_Location);
                entity.Property(p => p.Work_Time);
                entity.Property(p => p.Manager);
                entity.Property(p => p.Experience); 

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
 
            modelBuilder.Entity<EmailOtp>().ToTable("EmailOtp");

            modelBuilder.Entity<Requests>(entity =>
            {

                entity.HasKey(r => r.LeaveRequestId);

                entity.Property(r => r.RequestType)
                      .HasMaxLength(50);

                entity.Property(r => r.Session)
                      .HasMaxLength(20);

                entity.Property(r => r.Reason)
                      .HasMaxLength(250);

                entity.Property(r => r.HandoverTo)
                      .HasMaxLength(100);

                entity.Property(r => r.Status)
                      .HasMaxLength(20)
                      .HasDefaultValue("Pending");

                entity.Property(r => r.FromDate)
                      .HasColumnType("date");

                entity.Property(r => r.ToDate)
                      .HasColumnType("date");

                entity.Property(r => r.AppliedOn)
                      .HasDefaultValueSql("GETDATE()");


                // ==========================
                // RELATIONSHIP (FK)
                // User → Profile → Requests
                // ==========================

                entity.HasOne(r => r.UserProfile)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .HasPrincipalKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
         
            modelBuilder.Entity<Requests>()
                .HasOne(r => r.UserProfile)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .HasPrincipalKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);


          
            modelBuilder.Entity<UserProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();
        }
    }
}
