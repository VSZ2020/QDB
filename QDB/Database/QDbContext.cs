using Microsoft.EntityFrameworkCore;
using QDB.Models.Answers;
using QDB.Database.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QDB.Models;
using QDB.Models.Questions;

namespace QDB.Database
{
    public class QDbContext: DbContext
    {
        public const string DbName = "QDB.db";

        public DbSet<QDbChapter> Chapters { get; set; } = null!;
        public DbSet<QDbSection> Sections { get; set; } = null!;
        public DbSet<QDbQuestion> Questions { get; set; } = null!;
        public DbSet<QDbAnswer> Answers { get; set; } = null!;
        public DbSet<QDbDifficulty> Difficulties { get; set; } = null!;

        public QDbContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optBuilder)
        {
            if (!optBuilder.IsConfigured)
                optBuilder.UseSqlite($"Data Source={DbName}");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new QDatabaseConfig.ChapterConfiguration());
            builder.ApplyConfiguration(new QDatabaseConfig.SectionConfiguration());
            builder.ApplyConfiguration(new QDatabaseConfig.DifficultyConfiguration());
            builder.ApplyConfiguration(new QDatabaseConfig.QuestionsConfiguration());
            builder.ApplyConfiguration(new QDatabaseConfig.AnswersConfiguration());
        }

        public static QDbContext GetInstance()
        {
            return new QDbContext();
        }

        public void RecreateDb()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
