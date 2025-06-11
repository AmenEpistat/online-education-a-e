using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MathTrainer.Models;

namespace MathTrainer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<CourseProgress> CourseProgresses { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Value Converter для преобразования int в string
           

            

            modelBuilder.Entity<CourseProgress>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.CourseProgresses)
                .HasForeignKey(cp => cp.UserId)
                .HasConstraintName("FK_CourseProgresses_Users_UserId")
                .OnDelete(DeleteBehavior.Cascade);

          

            modelBuilder.Entity<ExamAttempt>()
                .HasOne(ea => ea.User)
                .WithMany()
                .HasForeignKey(ea => ea.UserId)
                .HasConstraintName("FK_ExamAttempts_Users_UserId")
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка Question
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CourseId)
                .HasConstraintName("FK_Questions_Courses_CourseId")
                .OnDelete(DeleteBehavior.SetNull);

            // Настройка Result
            modelBuilder.Entity<Result>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .HasConstraintName("FK_Results_Users_UserId")
                .OnDelete(DeleteBehavior.Cascade);

            // Начальные данные для Questions
            modelBuilder.Entity<Question>().HasData(
                new Question { Id = 1, Text = "11x = 36", Options = "5;4;3", CorrectAnswer = "3", Level = "easy" },
                new Question { Id = 2, Text = "9x + 4 = 48 - 2x", Options = "4;3;-2", CorrectAnswer = "4", Level = "easy" },
                new Question { Id = 3, Text = "6x + 2 = 2 - 4x", Options = "1;0;5", CorrectAnswer = "0", Level = "easy" },
                new Question { Id = 4, Text = "2(x - 3) = 4x + 6", Options = "-6;-3;-12", CorrectAnswer = "-6", Level = "medium" },
                new Question { Id = 5, Text = "x² - 4x + 4 = 0", Options = "2;-2;0", CorrectAnswer = "2", Level = "medium" },
                new Question { Id = 6, Text = "x² + x - 6 = 0", Options = "-3 и 2;-2 и 3;-1 и 6", CorrectAnswer = "-3 и 2", Level = "medium" },
                new Question { Id = 7, Text = "Решите систему: x + y = 5, x - y = 1", Options = "x=3, y=2;x=2, y=3;x=1, y=4", CorrectAnswer = "x=3, y=2", Level = "hard" },
                new Question { Id = 8, Text = "Найдите корень: √(x + 9) = 5", Options = "16;25;5", CorrectAnswer = "16", Level = "hard" },
                new Question { Id = 9, Text = "Решите: 2^(x) = 16", Options = "2;4;8", CorrectAnswer = "4", Level = "hard" },
                new Question { Id = 10, Text = "x + 2 = 7", CorrectAnswer = "5", Level = "exam", Options = "5;3;7" },
                new Question { Id = 11, Text = "x² - 1 = 0", CorrectAnswer = "1;-1", Level = "exam", Options = "1;-1;0" }
            );
        }
    }
}