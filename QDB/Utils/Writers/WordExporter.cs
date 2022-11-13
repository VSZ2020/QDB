using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.VariantTypes;
using QDB.Utils.Generator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xceed.Document.NET;
using Xceed.Words.NET;

namespace QDB.Utils.Writers
{
    public class WordExporter
    {
        public void Export(string filePath, List<QVariant> variants, string testTitle, bool MarkTrueAnswers = false)
        {
            var doc = DocX.Create(filePath, DocumentTypes.Document);
            doc.SetDefaultFont(new Xceed.Document.NET.Font("Times New Roman"), 10, Color.Black);
            //Настраиваем стили
            var heading1_style = DocX.GetParagraphStyleIdFromStyleName(doc, "Heading 1");
            var normal_style = DocX.GetParagraphStyleIdFromStyleName(doc, "Normal");

            doc.MarginLeft = 52f;
            doc.MarginRight = 52f;
            doc.MarginTop = 52f;
            doc.MarginBottom = 52f;
            for (int i = 0; i < variants.Count; i++)
            {
                var currVar = variants[i];
                var title = doc.InsertParagraph();
                title.StyleId = heading1_style;
                title.Append($"{testTitle}\nВариант {variants[i].Id}");
                title.SpacingLine(18);
                title.SpacingAfter(12);
                title.Alignment = Alignment.center;
                title.Color(Color.Black);
                title.Font("Times New Roman");
                title.FontSize(12);
                //Добавляем поле для ввода ФИО и номера группы
                var p = doc.InsertParagraph("ФИО _____________________________________________________________________________________________" +
                    "\nГруппа _________________________________________");
                p.Append("\nВнимание! В тесте может быть несколько вариантов ответа.").Italic(true);
                p.FontSize(10);
                p.SpacingAfter(3).SpacingBefore(3);
                p.SpacingLine(18);

                //Вставляем таблицу с вопросами
                int questionCount = currVar.QuestionsCount;
                var table = doc.AddTable(questionCount + 1, 2);
                table.Alignment = Alignment.left;
                //Заполняем первую строку таблицы (заголовок)
                var head_1 = table.Rows[0].Cells[0].Paragraphs[0].Append("Вопрос").SpacingAfter(6).SpacingBefore(6);
                var head_2 = table.Rows[0].Cells[1].Paragraphs[0].Append("Варианты ответа").SpacingAfter(6).SpacingBefore(6);
                head_1.Alignment = Alignment.center;
                head_2.Alignment = Alignment.center;
                head_1.Bold(true);
                head_2.Bold(true);

                for (int j = 0; j < questionCount; j++)
                {
                    var currQuestion = currVar.Questions[j];
                    var answersCount = currVar.Answers[j].Count;
                    table.Rows[j + 1].Cells[0].Paragraphs[0].Append($"{j + 1}. {currQuestion.Text}");
                    //Добавляем ответы
                    for (int k = 0; k < answersCount; k++)
                    {
                        var ap = table.Rows[j + 1].Cells[1].InsertParagraph(string.Format("{0}. {1}", k + 1, currVar.Answers[j][k].Content));
                        if (MarkTrueAnswers && currVar.Answers[j][k].IsCorrect)
                            ap.Bold(true);
                    }
                    table.Rows[j + 1].Cells[1].ReplaceText("\n", "");
                    table.Rows[j + 1].Cells[1].Paragraphs[0].Remove(false);
                }
                //Вставляем разрыв страницы
                var t = doc.InsertTable(table);
                if (i < variants.Count - 1)
                    t.InsertPageBreakAfterSelf();
            }
            doc.Save();
        }

        public void ExportTrueAnswers(string filePath, List<QVariant> variants, string testTitle)
        {
            //Модифицируем имя файла, чтобы добавить приписку
            string filename = System.IO.Path.GetFileNameWithoutExtension(filePath);
            filePath = filePath.Replace(filename, filename + "_answers");
            //Создаем документ
            var doc = DocX.Create(filePath, DocumentTypes.Document);
            doc.SetDefaultFont(new Xceed.Document.NET.Font("Times New Roman"), 10, Color.Black);
            //Настраиваем стили
            var heading1_style = DocX.GetParagraphStyleIdFromStyleName(doc, "Heading 1");

            doc.MarginLeft = 52f;
            doc.MarginRight = 52f;
            doc.MarginTop = 52f;
            doc.MarginBottom = 52f;
            for (int i = 0; i < variants.Count; i++)
            {
                var currVar = variants[i];
                var title = doc.InsertParagraph();
                title.StyleId = heading1_style;
                title.Append($"{testTitle}\nВариант {variants[i].Id}");
                title.SpacingLine(18);
                title.SpacingAfter(12);
                title.Alignment = Alignment.center;
                title.Color(Color.Black);
                title.Font("Times New Roman");
                title.FontSize(12);

                //Вставляем таблицу с вопросами и ответами
                int questionCount = currVar.QuestionsCount;
                var table = doc.AddTable(questionCount + 1, 2);
                table.Alignment = Alignment.left;
                //Заполняем первую строку таблицы (заголовок)
                var head_1 = table.Rows[0].Cells[0].Paragraphs[0].Append("Вопрос").SpacingAfter(6).SpacingBefore(6);
                var head_2 = table.Rows[0].Cells[1].Paragraphs[0].Append("Правильные тветы").SpacingAfter(6).SpacingBefore(6);
                head_1.Alignment = Alignment.center;
                head_2.Alignment = Alignment.center;
                head_1.Bold(true);
                head_2.Bold(true);

                for (int j = 0; j < questionCount; j++)
                {
                    var currQuestion = currVar.Questions[j];
                    var answersCount = currVar.Answers[j].Count;
                    table.Rows[j + 1].Cells[0].Paragraphs[0].Append($"{j + 1}. {currQuestion.Text}");
                    //Добавляем правильные ответы
                    for (int k = 0; k < answersCount; k++)
                    {
                        if (currVar.Answers[j][k].IsCorrect)
                            table.Rows[j + 1].Cells[1].InsertParagraph(string.Format("{0}. {1}",k + 1,currVar.Answers[j][k].Content));
                    }
                    //table.Rows[j + 1].Cells[1].ReplaceText("\n", "");
                    table.Rows[j + 1].Cells[1].Paragraphs[0].Remove(false);
                }
                //Вставляем разрыв страницы
                var t = doc.InsertTable(table);
                if (i < variants.Count - 1)
                    t.InsertPageBreakAfterSelf();
            }
            doc.Save();
        }
    }
}
