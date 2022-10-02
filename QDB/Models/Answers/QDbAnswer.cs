using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Models.Answers
{
    public class QDbAnswer
    {
        /// <summary>
        /// Перечень видов ответов
        /// </summary>
        public enum AType
        {
            text = 0,
            input = 1,
            image = 2
        }
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public AType Type { get; set; } = AType.text;
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;

        public static QDbAnswer GetDefaultAnswer(int questionId)
        {
            return new QDbAnswer() {  
                QuestionId = questionId, 
                Type = AType.text, 
                IsCorrect = false, 
                Content = "Answer" };
        }
    }
}
