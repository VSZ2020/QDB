// See https://aka.ms/new-console-template for more information
using QDB.Database;
using QDB.Models.Answers;
using QDB.Utils.Generator;
using QDB.Utils.Writers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

Console.WriteLine("---QDB Tester---");
ConsoleTraceListener listener = new ConsoleTraceListener();
Trace.Listeners.Add(listener);
//Проверка чтения базы вопросов из Excel
//QDB.Utils.Readers.ExcelReader reader = new QDB.Utils.Readers.ExcelReader();
//reader.Filepath = @"P:\База тестов.xlsx";
//var wb = reader.GetWorkbook(reader.Filepath);
//var sheet = reader.GetQuestionsWorksheet(wb);
//List<QDB.Models.Questions.QDbQuestion> questions;
//List<QDbAnswer> answers;
//reader.TryGetQuestions(sheet, out questions, out answers);

//Генерация вариантов
Console.WriteLine("Создание вариантов теста");
int variantsCount = 3;
int questionsCount = 5;
var chapters = ChaptersExtensions.GetAll();
List<QuestionGenData> genData = new();
for (int i = 0; i < questionsCount; i++)
{
    SelectedIndexes chaptersIds = new();
    for (int j = 0; j < chapters.Count; j++)
    {
        List<int> sectionsIds = new();
        var sections = SectionsExtensions.GetAll(chapters[j].Id);
        for (int k = 0; k < sections.Count; k++)
            sectionsIds.Add(sections[k].Id);
        chaptersIds.Indexes.TryAdd(chapters[j].Id, sectionsIds);
    }
    QuestionGenData data = new QuestionGenData() { 
        Difficulty = 0, 
        ChaptersIds = chaptersIds
    };
    genData.Add(data);
}
//Засекаем время генерации вариантов
Stopwatch timer = new Stopwatch();
timer.Start();
QTestGenerator generator = new QTestGenerator();
generator.MixAnswers = true;
var testVariants = generator.Generate(genData, variantsCount);
timer.Stop();
//Выводим варианты на экран
void PrintVariants()
{
    for (int i = 0; i < variantsCount; i++)
    {
        Console.WriteLine($"Вариант {testVariants[i].Id}");
        var variant = testVariants[i];
        for (int j = 0; j < questionsCount; j++)
        {
            Console.WriteLine($"\t{j + 1}. {variant.Questions[j].Text}");
            int answersCount = variant.Answers[j].Count;
            for (int k = 0; k < answersCount; k++)
            {
                var answer = variant.Answers[j][k];
                if (answer.IsCorrect)
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\t\t{k + 1}. {answer.Content}");
                if (answer.IsCorrect)
                    Console.ResetColor();
            }
        }
    }
}
PrintVariants();
Console.WriteLine($"Затраченное на генерацию {variantsCount} вариантов по {questionsCount} вопросов время равно {timer.ElapsedMilliseconds/1000.0} с");
Console.Write("Экспорт вариантов в DOCX...");
string exportPath = @"P:\Test Variants.docx";
WordExporter exp = new WordExporter();
exp.Export(exportPath, testVariants, "Вводный тест", true);
exp.ExportTrueAnswers(exportPath, testVariants, "Вводный тест");