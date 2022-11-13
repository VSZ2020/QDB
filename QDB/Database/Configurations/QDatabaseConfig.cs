using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QDB.Models;
using QDB.Models.Answers;
using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Database.Configurations
{
    
    public class QDatabaseConfig
    {
        public const int AllCategoriesId = 0;
        public const int UncategorizedId = 1;
        public class QuestionsConfiguration : IEntityTypeConfiguration<QDbQuestion>
        {
            public void Configure(EntityTypeBuilder<QDbQuestion> builder)
            {
                builder.ToTable("Questions");
                
            }
        }

        public class AnswersConfiguration : IEntityTypeConfiguration<QDbAnswer>
        {
            public void Configure(EntityTypeBuilder<QDbAnswer> builder)
            {
                builder.ToTable("Answers");
            }
        }

        public class ChapterConfiguration : IEntityTypeConfiguration<QDbChapter>
        {
            public void Configure(EntityTypeBuilder<QDbChapter> builder)
            {
                builder.ToTable("Chapters");
                //builder.HasMany(c => c.Sections).WithOne(s => s.Chapter);
                builder.HasData(new QDbChapter() { Id = UncategorizedId, Header = "Без категории" });
            }
        }
        public class SectionConfiguration : IEntityTypeConfiguration<QDbSection>
        {
            public void Configure(EntityTypeBuilder<QDbSection> builder)
            {
                builder.ToTable("Sections");
                builder.HasData(new QDbSection() { Id = UncategorizedId, Header = "Без подкатегории" });
            }
        }
        public class DifficultyConfiguration : IEntityTypeConfiguration<QDbDifficulty>
        {
            public void Configure(EntityTypeBuilder<QDbDifficulty> builder)
            {
                builder.ToTable("Difficulties");
                builder.HasData(new QDbDifficulty() { Id = 1, Name = "Очень легкий" });
                builder.HasData(new QDbDifficulty() { Id = 2, Name = "Легкий" });
                builder.HasData(new QDbDifficulty() { Id = 3, Name = "Средний" });
                builder.HasData(new QDbDifficulty() { Id = 4, Name = "Сложный" });
                builder.HasData(new QDbDifficulty() { Id = 5, Name = "Очень сложный" });
            }
        }
    }
}
