using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Utils.Generator
{
    /// <summary>
    /// Класс вопроса для генератора тестов
    /// </summary>
    public class QuestionGenData
    {
        public SelectedIndexes ChaptersIds { get; set; } = new();
        /// <summary>
        /// Сложность вопроса
        /// </summary>
        public int Difficulty { get; set; }
    }
}
