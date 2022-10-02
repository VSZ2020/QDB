using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Utils.Readers.QuestionReader
{
    public interface IQuestionReader
    {
        public QDbQuestion ImportQuestion();
        public void ExportQuestion(QDbQuestion question);
    }
}
