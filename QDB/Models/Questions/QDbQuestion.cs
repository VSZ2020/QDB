using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Models.Questions
{
    public class QDbQuestion
    {
        /// <summary>
        /// Тип вопроса: 0 - текстовый, 1 - вопрос-картинка
        /// </summary>
        public enum QType
        {
            Choise = 0,
            Input = 1,
            Match = 2
        }
        public int Id { get; set; }
        public QType Type { get; set; } = QType.Choise;
        public int Difficulty { get; set; } = 1;
        public int? ChapterId { get; set; } = -1;
        public int? SectionId { get; set; } = -1;
        
        public string? PictureURI { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public QDbQuestion() { }
    }
}
