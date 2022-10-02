using QDB.Models;
using QDB.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QDB.Utils.Logging;
using System.Collections.ObjectModel;

namespace QDB.Database
{
    public static class ChaptersExtensions
    {
        public static void Add(QDbChapter chapter)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Chapters.Add(chapter);
                context.SaveChanges();
            }
        }
        public static void Update(QDbChapter chapter)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Chapters.Update(chapter);
                context.SaveChanges();
            }
        }
        public static void Remove(QDbChapter chapter, bool UpdateChildren = false, int DefaultCategoryId = 1, int DefaultSectionId = 1)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Chapters.Remove(chapter);
                //Изменить все относящиеся к удаленному разделу вопросы
                if (UpdateChildren)
                {
                    var questionsForUpdate = context.Questions.Where(q => q.ChapterId == chapter.Id).ToList();
                    for (int i = 0; i < questionsForUpdate.Count; i++)
                    {
                        questionsForUpdate[i].ChapterId = DefaultCategoryId;
                        questionsForUpdate[i].SectionId = DefaultSectionId;
                        context.Questions.Update(questionsForUpdate[i]);
                    }
                }
                context.SaveChanges();
            }
        }

        public static void Remove(int id, bool UpdateChildren = false, int DefaultCategoryId = 1, int DefaultSectionId = 1)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                var chapter = context.Chapters.Where(c => c.Id == id).FirstOrDefault();
                if (chapter != null)
                {
                    context.Chapters.Remove(chapter);
                    //Изменить все относящиеся к удаленному разделу вопросы
                    if (UpdateChildren)
                    {
                        var questionsForUpdate = context.Questions.Where(q => q.ChapterId == chapter.Id).ToList();
                        for (int i = 0; i < questionsForUpdate.Count; i++)
                        {
                            questionsForUpdate[i].ChapterId = DefaultCategoryId;
                            questionsForUpdate[i].SectionId = DefaultSectionId;
                            context.Questions.Update(questionsForUpdate[i]);
                        }
                    }
                }
                else
                    Logger.Log($"No such chapter with Id = {id}", "Error");
                context.SaveChanges();
            }
        }

        public static List<QDbChapter> GetAll()
        {
            List<QDbChapter> chapters;
            using (QDbContext context = QDbContext.GetInstance())
            {
                chapters = context.Chapters.ToList();
            }
            return chapters;
        }
        public static void AddRange(this ObservableCollection<QDbChapter> chaptersList, IEnumerable<QDbChapter> chapters)
        {
            foreach (var chapter in chapters)
            {
                chaptersList.Add(chapter);
            }
        }
    }
}
