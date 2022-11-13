using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using ClosedXML;
using ClosedXML.Excel;
using QDB.Utils.Logging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO.Packaging;
using System.Windows;
using QDB.Controllers;
using QDB.Utils.enums;
using System.Diagnostics;
using QDB.Models.Answers;
using QDB.Database;
using QDB.Models;
using System.Collections.Immutable;
using System.Linq;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using QDB.Database.Configurations;

namespace QDB.Utils.Readers
{
    /*
     * Для импорта из Excel, сам файл должен иметь определенный формат и набор данных.
     * Должно быть как минимум 3 вкладки (Sheets):
     *      1. Questions
     *      2. Chapters
     *      3. Sections
     * Вкладка "Questions" должна содержать список вопросов и ряд колонок:
     *      - ID вопроса (int ID > 0)
     *      - Номер раздела (int CID > 0)
     *      - Номер подраздела (если отсутствует, то присваивается к подразделу "Все подкатегории" (SID > 0)
     *      - Сложность (int D = 1..5)
     *      - Тип вопроса (int QT)
     */
    public class ExcelReader : IDatabaseReader
    {
        private List<int> _misingChaptersIds = new();
        private List<int> _missingSectionsIds = new();
        private List<(int, int)> _chaptersToReplace = new();
        private List<(int, int)> _sectionsToReplace = new();

        private int _chaptersCount = 1;
        private int _sectionsCount = 1;
        private int _questionsCount = 0;
        public string Filepath { get; set; } = String.Empty;
        public List<QDbQuestion>? Questions;
        public List<QDbAnswer>? Answers;
        public List<QDbChapter>? Chapters;
        public List<QDbSection>? Sections;
        public void LoadQuestions(string filename)
        {
            Filepath = filename;
            //Обращаемся к книге
            var workbook = GetWorkbook(filename);
            var qSheet = GetQuestionsWorksheet(workbook);
            var cptSheet = GetChaptersWorksheet(workbook);
            var sectSheet = GetSectionsWorksheet(workbook);

            //Смещаем Id, ChapterId, SectionId на нужную позицию
            _questionsCount = QuestionExtensions.Count();
            _chaptersCount = ChaptersExtensions.Count();
            _sectionsCount = SectionsExtensions.Count();

            TryGetChapters(cptSheet, out Chapters);
            TryGetSections(sectSheet, out Sections);
            TryGetQuestions(qSheet, out Questions, out Answers);
           
            //Записываем полученные данные в БД
            ChaptersExtensions.Add(Chapters);
            SectionsExtensions.Add(Sections);
            QuestionExtensions.Add(Questions);
            AnswersExtensions.Add(Answers);
        }

        public XLWorkbook GetWorkbook(string path)
        {
            if (!File.Exists(path))
            {
                Logger.Log($"Некорректный путь или файл не существует:\n{path}","I/O");
                return new XLWorkbook(path);
            }
            return new XLWorkbook(path);
        }

        public IXLWorksheet GetQuestionsWorksheet(XLWorkbook book)
        {
            IXLWorksheet sheet;
            if (!book.TryGetWorksheet("Questions", out sheet))
            {
                return book.AddWorksheet("Questions");
            }
            return sheet;
        }
        public IXLWorksheet GetChaptersWorksheet(XLWorkbook book)
        {
            IXLWorksheet sheet;
            if (!book.TryGetWorksheet("Chapters", out sheet))
            {
                return book.AddWorksheet("Chapters");
            }
            return sheet;
        }
        public IXLWorksheet GetSectionsWorksheet(XLWorkbook book)
        {
            IXLWorksheet sheet;
            if (!book.TryGetWorksheet("Sections", out sheet))
            {
                return book.AddWorksheet("Sections");
            }
            return sheet;
        }

        public bool TryGetQuestions(IXLWorksheet questionsSheet, out List<QDbQuestion> QList, out List<QDbAnswer> AList)
        {
            QList = new List<QDbQuestion>();
            AList = new List<QDbAnswer>();

            var firstRow = questionsSheet.FirstRowUsed();
            if (firstRow == null)
            {
                Logger.Log($"No questions found in Excel file {Filepath}", "Excel reader");
                return false;
            }
            var headersRow = firstRow.RowUsed();
            //Если не выполнено требование о минимальном количестве колонок, то...
            if (headersRow.CellCount() < 3)
            {
                string msg = $"The 3-cols minimal requirement is't satisfied in Excel file {Filepath}. Sheet name: {questionsSheet.Name}";
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log(
                    msg, 
                    "Excel reader");
                return false;
            }
            //Получаем список названий колонок
            string[] names = new string[headersRow.CellCount()]; 
            for(int i = 0; i < names.Length; i++)
            {
                names[i] = headersRow.Cell(i + 1).GetString();
            }
            //Предлагаем пользователю сопоставить названия колонок и полей класса QDbQuestion
            //TODO
            //...
            //Временно используется сопоставление по-порядку
            // 1 - ID, 2 - ChapterID, 3 - SectionID, 4 - Difficult, 5 - Type, 6 - Question, 7 - Answer 1, 8 - Answer 2, ...
            // 1 - 7 - постоянные поля, присутствие которых обязательно
            // Максимальное количество ответов - 10
            var excelCols = MatchExcelColumnID();
            int qCount = 0;
            int answSum = 0;
            var lastRowInSheet = questionsSheet.LastRowUsed();
            var currentRow = headersRow.RowBelow();
            while (currentRow.WorksheetRow() != lastRowInSheet)
            {
                if (currentRow.Cell(excelCols[DbColNames.ID]).IsEmpty() ||
                    currentRow.Cell(excelCols[DbColNames.Question]).IsEmpty())
                {
                    currentRow = currentRow.RowBelow();
                    continue;
                }
                int colsCount = currentRow.RowUsed().CellCount();
                //Считаем, что разность между общим числом колонок и количеством служебных (6) - это количество ответов
                int answersCount = colsCount - 6;
                //Готовим значения по-умолчанию
                int questionID = _questionsCount + qCount + 1;
                int chapterId, sectionId = 1;
                int difficult = 1;
                QDbQuestion.QType questionType = QDbQuestion.QType.Choise;

                //Пытаемся прочитать данные
                currentRow.Cell(excelCols[DbColNames.ChapterID]).TryGetValue<int>(out chapterId);
                currentRow.Cell(excelCols[DbColNames.SectionID]).TryGetValue<int>(out sectionId);
                if (chapterId < 1)
                    chapterId = 1;
                else
                    chapterId += _chaptersCount;
                if (sectionId < 1)
                    sectionId = 1;
                else
                    sectionId += _sectionsCount;
                if (!ContainsChapterId(chapterId) && !_misingChaptersIds.Contains(chapterId))
                {
                    _misingChaptersIds.Add(chapterId);
                    chapterId = QDatabaseConfig.UncategorizedId;
                }
                if (!ContainsSectionId(sectionId) && !_missingSectionsIds.Contains(sectionId))
                {
                    _missingSectionsIds.Add(sectionId);
                    sectionId = QDatabaseConfig.UncategorizedId;
                }

                currentRow.Cell(excelCols[DbColNames.Difficult]).TryGetValue<int>(out difficult);
                //автоматически присваиваем низкую сложность при отсутствии указания в файле
                if (difficult < 1)
                    difficult = 1;
                currentRow.Cell(excelCols[DbColNames.QuestionType]).TryGetValue<QDbQuestion.QType>(out questionType);
                string questionText = currentRow.Cell(excelCols[DbColNames.Question]).GetString();
                if (string.IsNullOrEmpty(questionText))
                {
                    currentRow = currentRow.RowBelow();
                    continue;
                }
#if DEBUG
                //---------------------------------------------------
                //Проверка считанных данных. Выводим в консоль
                Trace.WriteLine($"{qCount + 1}. {questionText}");
                Trace.WriteLine($"(CID: {chapterId} / SID: {sectionId} / Diff: {difficult} / Type: {questionType})");
                //---------------------------------------------------
#endif
                for (int i = 0; i < answersCount; i++)
                {
                    var cell = currentRow.Cell(excelCols[DbColNames.Answer_1] + i);
                    string answerText = cell.GetString();
                    if (string.IsNullOrEmpty(answerText))
                        continue;
                    //Проверяем, является ли ответ верным
                    bool isTrueAnswer = false;
                    if (cell.Style.Font.Bold == true)
                        isTrueAnswer = true;
                    //Создаем объект ответа
                    QDbAnswer answer = new QDbAnswer()
                    {
                        QuestionId = questionID,
                        Content = answerText,
                        Type = QDbAnswer.AType.text,
                        IsCorrect = isTrueAnswer
                    };
                    AList.Add(answer);
#if DEBUG
                    //-----------------------------------------------
                    answSum++;
                    if (isTrueAnswer)
                        Console.ForegroundColor = ConsoleColor.Green;
                    Trace.WriteLine($"\t{i + 1}. {answerText}");
                    if (isTrueAnswer)
                        Console.ResetColor();
                    //-----------------------------------------------
#endif
                }

                //Создаем объект вопроса
                QDbQuestion question = new QDbQuestion()
                {
                    Id = questionID,
                    ChapterId = chapterId,
                    SectionId = sectionId,
                    Difficulty = difficult,
                    Type = questionType,
                    Text = questionText,
                    PictureURI = ""
                };
                //Добавляем в список вопросов
                QList.Add(question);

                qCount++;
                //Спускаемся на строку вниз
                currentRow = currentRow.RowBelow();
            }
#if DEBUG
            //Небольшая статистика по результатам чтения
            Trace.WriteLine($"Количество вопросов - {qCount}. Среднее кол-во ответов - {Math.Round((double)answSum / qCount, 1)}");
#endif
            return true;
        }

        /// <summary>
        /// Сопоставляет тип считываемых данных и номером колонки в Excel документе
        /// </summary>
        /// <returns></returns>
        private Dictionary<DbColNames, int> MatchExcelColumnID()
        {
            Dictionary<DbColNames, int> columnIDs = new();
            columnIDs[DbColNames.ID] = 1;
            columnIDs[DbColNames.ChapterID] = 2;
            columnIDs[DbColNames.SectionID] = 3;
            columnIDs[DbColNames.Difficult] = 4;
            columnIDs[DbColNames.QuestionType] = 5;
            columnIDs[DbColNames.Question] = 6;
            columnIDs[DbColNames.Answer_1] = 7;
            return columnIDs;
        }

        public bool TryGetChapters(IXLWorksheet chaptersSheet, out List<QDbChapter> CList)
        {
            CList = new();

            var firstRow = chaptersSheet.FirstRowUsed();
            if (firstRow == null)
            {
                Logger.Log($"No chapters found in Excel file {Filepath}", "Excel reader");
                return false;
            }
            var headersRow = firstRow.RowUsed();
            //Если не выполнено требование о минимальном количестве колонок, то...
            if (headersRow.CellCount() < 2)
            {
                string msg = $"The 2-cols minimal requirement is't satisfied in Excel file {Filepath}. Sheet name: {chaptersSheet.Name}";
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log(
                    msg,
                    "Excel reader");
                return false;
            }
            //Сопоставляем номера колонок, содержащих ID и название раздела
            int[] excelCols = new int[] { 1, 2 };
            int cCount = 0;
            var currentRow = headersRow.RowBelow();
            while (!currentRow.Cell(excelCols[0]).IsEmpty() || !currentRow.Cell(excelCols[1]).IsEmpty())
            {
                int chapterID = -1;
                currentRow.Cell(excelCols[0]).TryGetValue<int>(out chapterID);
                string chapterName = currentRow.Cell(excelCols[1]).GetString();
                if (chapterID < 0)
                {
                    currentRow = currentRow.RowBelow();
                    continue;
                }
                if (string.IsNullOrEmpty(chapterName))
                {
                    chapterName = $"Без названия {cCount + 1}";
                }
                chapterID = _chaptersCount + cCount + 1;
                //Создаем объект раздела
                QDbChapter chpt = new() { Id = chapterID, Header = chapterName };
                CList.Add(chpt);

                currentRow = currentRow.RowBelow();
                cCount++;
            }
            return true;
        }
        public bool TryGetSections(IXLWorksheet sectionsSheet, out List<QDbSection> SList)
        {
#if DEBUG
            Trace.WriteLine(String.Format("Try sections read. Operated sheet: {0}",sectionsSheet.Name));
#endif
            SList = new();

            var firstRow = sectionsSheet.FirstRowUsed();
            if (firstRow == null)
            {
                Logger.Log($"No sections found in Excel file {Filepath}", "Excel reader");
                return false;
            }
            var headersRow = firstRow.RowUsed();
            //Если не выполнено требование о минимальном количестве колонок, то...
            if (headersRow.CellCount() < 3)
            {
                string msg = $"The 2-cols minimal requirement is't satisfied in Excel file {Filepath}. Sheet name: {sectionsSheet.Name}";
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log(
                    msg,
                    "Excel reader");
                return false;
            }
            //Сопоставляем номера колонок, содержащих ID и название раздела
            int[] excelCols = new int[] { 1, 2, 3 };
            int sCount = 0;
            var currentRow = headersRow.RowBelow();
            while (
                !currentRow.Cell(excelCols[0]).IsEmpty() || 
                !currentRow.Cell(excelCols[1]).IsEmpty() || 
                !currentRow.Cell(excelCols[2]).IsEmpty())
            {
                int sectionId = 0;
                currentRow.Cell(excelCols[0]).TryGetValue<int>(out sectionId);

                int chapterId = 0;
                currentRow.Cell(excelCols[1]).TryGetValue<int>(out chapterId);

                string sectionName = currentRow.Cell(excelCols[2]).GetString();
                
                if (sectionId < 0)
                {
                    currentRow = currentRow.RowBelow();
                    continue;
                }
                if (string.IsNullOrEmpty(sectionName))
                {
                    sectionName = $"Без названия {sCount + 1}";
                }
                sectionId = _sectionsCount + sCount + 1;
                if (chapterId < 0)
                    chapterId = QDatabaseConfig.UncategorizedId;
                else
                    chapterId += _chaptersCount;
                //Создаем объект раздела
                QDbSection sect = new() { 
                    Id = sectionId, 
                    Header = sectionName,
                    ChapterId = chapterId};
                SList.Add(sect);

                currentRow = currentRow.RowBelow();
                sCount++;
            }
#if DEBUG
            Trace.WriteLine(String.Format("Founded {0} sections", sCount));
#endif
            return true;
        }

        public bool ContainsChapterId(int id)
        {
            for (int i = 0; i < Chapters?.Count; i++)
            {
                if (Chapters[i]?.Id == id)
                    return true;
            }
            return false;
        }
        public bool ContainsSectionId(int id)
        {
            for (int i = 0; i < Sections?.Count; i++)
            {
                if (Sections[i]?.Id == id)
                    return true;
            }
            return false;
        }
    }
}
