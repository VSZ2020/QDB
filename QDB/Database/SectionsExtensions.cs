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
    public static class SectionsExtensions
    {
        public static void Add(QDbSection section)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Sections.Add(section);
                context.SaveChanges();
            }
        }
        public static void Update(QDbSection section)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Sections.Update(section);
                context.SaveChanges();
            }
        }
        public static void Remove(QDbSection section, bool UpdateChildren = false, int DefaultSectionId = 1)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Sections.Remove(section);
                if (UpdateChildren)
                {
                    var questionsForUpdate = context.Questions.Where(q => q.ChapterId == section.Id).ToList();
                    for (int i = 0; i < questionsForUpdate.Count; i++)
                    {
                        questionsForUpdate[i].SectionId = DefaultSectionId;
                        context.Questions.Update(questionsForUpdate[i]);
                    }
                }
                context.SaveChanges();
            }
        }

        public static void Remove(int id, bool UpdateChildren = false, int DefaultSectionId = 1)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                var section = context.Sections.Where(c => c.Id == id).FirstOrDefault();
                if (section != null)
                {
                    context.Sections.Remove(section);
                    if (UpdateChildren)
                    {
                        var questionsForUpdate = context.Questions.Where(q => q.ChapterId == section.Id).ToList();
                        for (int i = 0; i < questionsForUpdate.Count; i++)
                        {
                            questionsForUpdate[i].SectionId = DefaultSectionId;
                            context.Questions.Update(questionsForUpdate[i]);
                        }
                    }
                }
                else
                    Logger.Log($"No such section with Id = {id}", "Error");
                context.SaveChanges();
            }
        }

        public static List<QDbSection> GetAll(int chapterId = 0)
        {
            List<QDbSection> sections;
            using (QDbContext context = QDbContext.GetInstance())
            {
                if (chapterId > 0)
                {
                    sections = context.Sections.Where(s => s.ChapterId == chapterId).ToList();
                }
                else
                    sections = context.Sections.ToList();
            }
            return sections;
        }
        public static void AddRange(this ObservableCollection<QDbSection> list, IEnumerable<QDbSection> sections)
        {
            foreach (var section in sections)
            {
                list.Add(section);
            }
        }
    }
}
