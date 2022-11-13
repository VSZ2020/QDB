using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using QDB.Models;
using QDB.Models.Answers;
using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Database
{
    public static class QuestionExtensions
    {
        public static void Add(QDbQuestion question)
        {
            using (QDbContext ctx = QDbContext.GetInstance())
            {
                ctx.Questions.Add(question);
                ctx.SaveChanges();
            }
        }

        public static void Add(IEnumerable<QDbQuestion> questions)
        {
            using (QDbContext ctx = QDbContext.GetInstance())
            {
                ctx.Questions.AddRange(questions);
                ctx.SaveChanges();
            }
        }

        public static void Update(QDbQuestion question)
        {
            using (QDbContext ctx = QDbContext.GetInstance())
            {
                ctx.Questions.Update(question);
                ctx.SaveChanges();
            }
        }
        public static void Remove(QDbQuestion question)
        {
            using (QDbContext ctx = QDbContext.GetInstance())
            {
                ctx.Questions.Remove(question);
                ctx.SaveChanges();
            }
        }

        public static List<QDbQuestion> GetRelatedQuestions(this QDbChapter chapter, int chapterId)
        {
            return new List<QDbQuestion>();
        }

        public static int Count()
        {
            int count = 0;
            using (QDbContext ctx = QDbContext.GetInstance())
            {
                //HACK: Slow method!
                count = ctx.Questions.Count();
            }
            return count;
        }
        public static void AddRange(this ObservableCollection<QDbQuestion> list, IEnumerable<QDbQuestion> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
        public static List<QDbQuestion> GetAll(int chapterId = 0, int sectionId = 0)
        {
            List<QDbQuestion> questions = new List<QDbQuestion>();
            using (QDbContext ctx = QDbContext.GetInstance())
            {
                if (chapterId > 0)
                {
                    if (sectionId > 0)
                    {
                        questions = ctx.Questions.Where(q => q.ChapterId == chapterId && q.SectionId == sectionId).ToList();
                    }
                    else
                        questions = ctx.Questions.Where(q => q.ChapterId == chapterId).ToList();
                }
                else
                    questions = ctx.Questions.ToList();
            }
            return questions;
        }
    }
}
