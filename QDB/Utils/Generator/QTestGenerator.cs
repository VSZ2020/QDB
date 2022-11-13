using DocumentFormat.OpenXml.Packaging;
using QDB.Database;
using QDB.Models;
using QDB.Models.Answers;
using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QDB.Utils.Generator
{
    public class QTestGenerator
    {
        public bool MixAnswers { get; set; } = false;

        private List<int> _UsedQuestionIDs = new();
        public QTestGenerator()
        {
        }
        public List<QVariant> Generate(List<QuestionGenData> questionData, int variantsCount)
        {
            List<QVariant> variants = new();
            for (int i = 0; i < variantsCount; i++)
            {
                QVariant variant = GenerateVariant(i + 1, questionData);
                variants.Add(variant);
            }
            return variants;
        }

        public QVariant GenerateVariant(int variant_id, List<QuestionGenData> questionData)
        {
            List<QDbQuestion> variantQuestions = new();
            Random rnd = new Random();
            int questionsCount = questionData.Count;
            for (int i = 0; i < questionsCount; i++)
            {
                //Общее хранилище вопросов для указанных разделов и подразделов
                List<QDbQuestion> collectedQuestions = new();
                //Цикл по ID разделов
                foreach(var item in questionData[i].ChaptersIds.Indexes)
                {
                    if (item.Value.Count == 0)
                        continue;
                    //Цикл по ID подразделов
                    for (int k = 0; k < item.Value.Count; k++)
                    {
                        //Получаем список вопросов для данного ID раздела и ID подраздела
                        List<QDbQuestion> questions = QuestionExtensions.GetAll(
                            /*Cjapter ID*/
                            item.Key,
                            /*Section ID*/
                            item.Value[k]);
                        //Если задана НЕ любая сложность, то выбираем вопросы с указанным уровнем сложности
                        if (questionData[i].Difficulty > 0 && questionData[i].Difficulty <= 5)
                        {
                            questions = questions.Where(q => q.Difficulty == questionData[i].Difficulty).ToList();
                        }
                        collectedQuestions.AddRange(questions);
                    }
                }
                //Работаем с общей базой вопросов. Алгоритм отбора в лоб.
                //Убираем повторы из базы
                collectedQuestions = collectedQuestions.Where(q => !_UsedQuestionIDs.Contains(q.Id)).ToList();
                //Выбираем из этой базы случайный вопрос по его ID
                int collectionSize = collectedQuestions.Count;
                //Если количество вопросов 0, то уведомляем пользователя
                if (collectionSize == 0)
                {
                    MessageBox.Show(
                        $"Не удалось подобрать для варианта #{variant_id} и вопроса #{i + 1} вопроса из базы. Будет создан пустой вопрос",
                        "Внимание",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    variantQuestions.Add(GetEmptyQuestion());
                    continue;
                }
                //Если количество отобранных вопросов = 1 и он ранее не использовался, то выбираем только его и уведомляем пользователя 
                //об отсутствии альтернатив для выбора
                if (collectionSize == 1)
                {
                    variantQuestions.Add(collectedQuestions[0]);
                    _UsedQuestionIDs.Add(collectedQuestions[0].Id);
                }
                else
                {
                    int rndId = rnd.Next(0, collectionSize);
                    var choosedQuestion = collectedQuestions[rndId];
                    //Удаляем выбранный вопрос из коллекции и добавляем его в базу уже отобранных вопросов, чтобы не было повторов
                    variantQuestions.Add(choosedQuestion);
                    _UsedQuestionIDs.Add(choosedQuestion.Id);
                }
            }
            //Запрашиваем ответы для выбранных вопросов
            List<List<QDbAnswer>> allAnswers = new(questionsCount);
            for (int i = 0; i < questionsCount; i++)
            {
                int qId = variantQuestions[i].Id;
                if (qId == 0)
                {
                    allAnswers.Add(new List<QDbAnswer>() { QDbAnswer.GetDefaultAnswer(0) });
                    continue;
                }
                List<QDbAnswer> answers = AnswersExtensions.GetAll(qId);
                //Перемешиваем ответы, если задана опция
                if (MixAnswers)
                    MixQuestionAnswers(ref answers);
                allAnswers.Add(answers);
            }
            //Обнуляем список использованных вопросов для данного варианта
            _UsedQuestionIDs.Clear();
            return new QVariant()
            {
                Id = variant_id,
                Questions = variantQuestions,
                Answers = allAnswers
            };
        }

        private QDbQuestion GetEmptyQuestion()
        {
            return new QDbQuestion()
            {
                Id = 0,
                ChapterId = 0,
                SectionId = 0,
                Difficulty = 1,
                Text = "Пустой вопрос",
                Type = QDbQuestion.QType.Choise
            };
        }

        private void MixQuestionAnswers(ref List<QDbAnswer> answers)
        {
            if (answers == null)
                return;
            int count = answers.Count;
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                int insertPosition = rnd.Next(0, count - 1);
                var a1 = answers[i];
                answers[i] = answers[insertPosition];
                answers[insertPosition] = a1;
            }
        }
    }
}
