using QDB.Models.Answers;
using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Database
{
    public static class AnswersExtensions
    {
        public static void Add(QDbAnswer answer)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Answers.Add(answer);
                context.SaveChanges();
            }
        }
        public static void Add(IEnumerable<QDbAnswer> answers)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Answers.AddRange(answers);
                context.SaveChanges();
            }
        }
        public static void Update(QDbAnswer answer)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Answers.Update(answer);
                context.SaveChanges();
            }
        }
        public static void Remove(QDbAnswer answer)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                context.Answers.Remove(answer);
                context.SaveChanges();
            }
        }
        public static void Replace(int QuestionId, List<QDbAnswer> answersList)
        {
            using (QDbContext context = QDbContext.GetInstance())
            {
                var answers = context.Answers.Where(a => a.QuestionId == QuestionId).ToList();
                foreach (var answer in answers)
                {
                    context.Answers.Remove(answer);
                }
                foreach(var answer in answersList)
                {
                    context.Answers.Add(answer);
                }
                context.SaveChanges();
            }
        }
        public static List<QDbAnswer> GetAll(int questionId = 0)
        {
            List<QDbAnswer> answers = new List<QDbAnswer>();
            using (QDbContext ctx = QDbContext.GetInstance())
            {
                if (questionId > 0)
                    answers.AddRange(ctx.Answers.Where(a => a.QuestionId == questionId).ToList());
                else
                    answers.AddRange(ctx.Answers.ToList());
            }
            return answers;
        }
        public static void AddRange(this ObservableCollection<QDbAnswer> list, IEnumerable<QDbAnswer> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
        public static int Count()
        {
            int count = 0;
            using (QDbContext ctx = QDbContext.GetInstance())
            {
                //HACK: Slow method!
                count = ctx.Answers.Count();
            }
            return count;
        }
    }
}
