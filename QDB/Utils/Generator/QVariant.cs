using QDB.Models.Answers;
using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Utils.Generator
{
    public class QVariant
    {
        public int Id { get; set; }
        public List<QDbQuestion> Questions { get; set; } = new();
        /// <summary>
        /// Ответы на отобранные вопросы. 
        /// Индекс списка ответов в главном родительском списке соответствует индексу вопроса
        /// в перечне отобранных вопросов
        /// </summary>
        public List<List<QDbAnswer>> Answers { get; set; } = new();
        public int QuestionsCount { get => Questions.Count; }
    }
}
