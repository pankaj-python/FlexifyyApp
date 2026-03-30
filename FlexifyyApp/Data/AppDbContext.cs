using FlexifyyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlexifyyApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Exercises> Exercises { get; set; }
        public DbSet<Workouts> Workouts { get; set; }
        public DbSet<WorkoutDetails> WorkoutDetails { get; set; }
        public DbSet<Progress> Progress { get; set; }
        public DbSet<DietPlans> DietPlans { get; set; }
        public DbSet<Achievements> Achievements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Goal).HasMaxLength(20);
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("SYSDATETIME()");
            });

            modelBuilder.Entity<Exercises>(entity =>
            {
                entity.ToTable("Exercises");
                entity.HasKey(e => e.ExerciseId);
                entity.Property(e => e.ExerciseId).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MuscleGroup).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.VideoUrl).HasMaxLength(300);
                entity.Property(e => e.IsCustom).HasDefaultValue(false);
            });


            modelBuilder.Entity<Workouts>(entity =>
            {
                entity.ToTable("Workouts");
                entity.HasKey(e => e.WorkoutId);
                entity.Property(e => e.WorkoutId).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId)
                      .HasColumnName("UsersID");
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.WorkoutName).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .HasConstraintName("FK_Workouts_Users")
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<WorkoutDetails>(entity =>
            {
                entity.ToTable("WorkoutDetails");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Sets).IsRequired();
                entity.Property(e => e.Reps).IsRequired();
                entity.Property(e => e.WeightKG).HasColumnType("decimal(6,2)");
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("SYSDATETIME()");
                entity.HasOne(e => e.Workout)
                      .WithMany(w => w.Details)
                      .HasForeignKey(e => e.WorkoutId)
                      .HasConstraintName("FK_WorkoutDetails_Workouts")
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Exercise)
                      .WithMany()
                      .HasForeignKey(e => e.ExerciseId)
                      .HasConstraintName("FK_WorkoutDetails_Exercises")
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Progress>(entity =>
            {
                entity.ToTable("Progress");
                entity.HasKey(e => e.ProgressId);
                entity.Property(e => e.ProgressId).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId)
                      .HasColumnName("UsersID");         
                entity.Property(e => e.Weight)
                      .IsRequired()
                      .HasColumnType("decimal(5,2)");
                entity.Property(e => e.BodyFatPercent)
                      .HasColumnType("decimal(4,1)");
                entity.Property(e => e.ChestCM)
                      .HasColumnType("decimal(4,1)");
                entity.Property(e => e.WaistCM)
                      .HasColumnType("decimal(4,1)");
                entity.Property(e => e.PhotoUrl).HasMaxLength(500);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("SYSDATETIME()");
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .HasConstraintName("FK_Progress_Users")
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<DietPlans>(entity =>
            {
                entity.ToTable("DietPlans");
                entity.HasKey(e => e.DietId);
                entity.Property(e => e.DietId).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId)
                      .HasColumnName("UsersID");      
                entity.Property(e => e.MealType)
                      .IsRequired()
                      .HasMaxLength(30);
                entity.Property(e => e.FoodItems)
                      .IsRequired()
                      .HasMaxLength(1000);
                entity.Property(e => e.DietDate)
                      .HasColumnName("Dietdate");        
                entity.Property(e => e.IsVegetarian)
                      .HasColumnName("IsVegeterian")    
                      .HasDefaultValue(true);
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("SYSDATETIME()");
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .HasConstraintName("FK_DietPlans_Users")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Achievements>(entity =>
            {
                entity.ToTable("Achievements");
                entity.HasKey(e => e.AchievementId);
                entity.Property(e => e.AchievementId).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId)
                      .HasColumnName("UsersID");       
                entity.Property(e => e.BadgeName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.EarnedAt)
                      .HasDefaultValueSql("SYSDATETIME()");
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .HasConstraintName("FK_Achievements_Users")
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}