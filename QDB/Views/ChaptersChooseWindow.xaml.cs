using DocumentFormat.OpenXml.Drawing;
using QDB.Database;
using QDB.Models;
using QDB.UserControls;
using QDB.UserControls.Classes;
using QDB.Utils.Generator;
using QDB.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QDB.Views
{
    /// <summary>
    /// Окно для выбора разделов, включаемых при генерации в каждый номер вопроса
    /// </summary>
    public partial class ChaptersChooseWindow : Window
    {
        private List<QDbDifficulty> Difficulties = new();
        private List<ChapterSelectorElement> QElements = new();

        public bool SuccessfullEdit = false;
        public List<QuestionGenData> Questions = new();

        //Ссылки на элементы управления
        public ChaptersChooseWindow(int QuestionsCount = 1)
        {
            InitializeComponent();
            Init(QuestionsCount);
        }

        public void Init(int questionsCount = 1)
        {
            //Загружаем список уровней сложности вопросов
            Difficulties = DifficultiesExtensions.GetAll();
            //Добавляем служебное поле - "Любая"
            Difficulties.Insert(0, new QDbDifficulty() { Id = 0, Name = "Любая" });

            //Загружаем список подготовленных разделов
            for (int qIndex = 0; qIndex < questionsCount; qIndex++)
            {
                var chaptersElements = PrepareChaptersWithSections();
                ChapterSelectorElement qe = new ChapterSelectorElement();
                qe.Id = qIndex + 1;
                qe.GroupName = $"Вопрос #{qIndex + 1}";
                qe.Difficulties = Difficulties;
                if (qe.Chapters == null)
                    qe.Chapters = new();
                for (int i = 0; i < chaptersElements.Count; i++)
                {
                    qe.Chapters.Add(chaptersElements[i]);
                }
                QElements.Add(qe); 
            }
            //Присваиваем созданный массив к элементу отображения
            qList.ItemsSource = QElements;

        }

        public bool CheckChoosedChapters()
        {
            StringBuilder errorsMsg = new StringBuilder();
            //быстро пробегаем по массивам и проверяем, что в каждом вопросе выбран хотя бы 1 раздел
            for (int eIndex = 0; eIndex < QElements.Count; eIndex++)
            {
                bool hasChecked = false;
                for (int i = 0; i < QElements[eIndex].Chapters.Count; i++)
                {
                    var isChecked = QElements[eIndex].Chapters[i].IsChecked;
                    if (isChecked.HasValue && isChecked.Value)
                    {
                        hasChecked = true;
                        break;
                    }
                }
                if (!hasChecked)
                    errorsMsg.AppendLine($"Не выбрано ни одного раздела в вопросе #{eIndex + 1} с заголовком \"{QElements[eIndex].GroupName}\"");
            }
            //Если есть ошибки, то выводим их пользователю
            if (errorsMsg.Length > 0)
                MessageBox.Show(
                    errorsMsg.ToString(), 
                    "Внимание", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
            return errorsMsg.Length == 0;
        }

        public void ReadSelectedIndexes()
        {
            int questionsCount = QElements.Count;
            for (int qIndx = 0; qIndx < questionsCount; qIndx++)
            {
                int choosedDifficulty = QElements[qIndx].SelectedDifficultyIndex + 1;
                var indexes = GetSectionsIndexes(QElements[qIndx].Chapters.ToList());
                if (choosedDifficulty < 0 || choosedDifficulty > Difficulties[^1].Id)
                    choosedDifficulty = 0;
                QuestionGenData QTG = new QuestionGenData()
                {
                    ChaptersIds = indexes,
                    Difficulty = choosedDifficulty
                };
                Questions.Add(QTG);
            }
        }

        public SelectedIndexes GetSectionsIndexes(List<ChapterElement> chaptersList)
        {
            int chaptersCount = chaptersList.Count;
            //Создаем массив массивов. В родительском массиве индекс элемента соответствует Id раздела смещенного на 1 назад
            SelectedIndexes indexes = new(chaptersCount);
            for (int i = 0; i < chaptersCount; i++)
            {
                var chapterIsChecked = chaptersList[i].IsChecked;
                if (chaptersList[i].Chapter.Id == 0 || 
                    (chapterIsChecked.HasValue && chapterIsChecked.Value))
                {
                    List<int> sectionsIndx = new();
                    for (int j = 1; j < chaptersList[i].Sections.Count; j++)
                    {
                        if (chaptersList[i].Sections[j].IsChecked ||
                            chaptersList[i].Sections[j].Section.Id == 0)
                            sectionsIndx.Add(chaptersList[i].Sections[j].Section.Id);
                    }
                    indexes.Indexes.Add(chaptersList[i].Chapter.Id,sectionsIndx);
                }
            }
            return indexes;
        }
        /// <summary>
        /// Выбирает все разделы в списке разделов
        /// </summary>
        /// <param name="elementId">Порядковый номер вопроса, для которого производится выбор. Начинается с 1. Если 0 - выбирает для всех вопросов</param>
        private void SelectAllChapters(int elementId = 0)
        {
            SetChaptersStateTo(elementId, true);
        }
        /// <summary>
        /// Убирает выбор всех разделов в списке разделов
        /// </summary>
        /// <param name="elementId">Порядковый номер вопроса, для которого производится выбор. Начинается с 1. Если 0 - выбирает для всех вопросов</param>
        private void UnselectAllChapters(int elementId = 0)
        {
            SetChaptersStateTo(elementId, false);
        }
        /// <summary>
        /// Задает состояние newState (выбран/не выбран) для всех разделов вопроса с ID = elementId. 
        /// Если elementId = 0, то для всех вопросов задается указанное состояние
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="newState"></param>
        private void SetChaptersStateTo(int elementId, bool newState = false)
        {
            if (elementId >= 0 && elementId < QElements.Count)
            {
                for (int j = 0; j < QElements[elementId].Chapters.Count; j++)
                    QElements[elementId].Chapters[j].IsChecked = newState;
            }
            else
                Logger.Log($"Ошибка в индексе вопроса. Функция SetChaptersState. Попытка получить элемент по индексу {elementId - 1}");
        }
        private List<ChapterElement> PrepareChaptersWithSections()
        {
            List<ChapterElement> chaptersElements = new();
            //Загружаем список разделов
            var chapters = ChaptersExtensions.GetAll(false);
            for (int i = 0; i < chapters.Count; i++)
            {
                List<SectionElement> sectionElements = new();
                List<QDbSection> sections = SectionsExtensions.GetAll(chapters[i].Id, false);
                ChapterElement CE = new()
                {
                    Chapter = chapters[i],
                    Sections = sectionElements
                };
                for (int j = 0; j < sections.Count; j++)
                {
                    SectionElement SE = new(CE)
                    {
                        Section = sections[j]
                    };
                    sectionElements.Add(SE);
                }
                chaptersElements.Add(CE);
            }
            return chaptersElements;
        }
        #region Selection Buffering
        const string bufferFile = "buffer.json";
        public void SaveSelectionToBuffer()
        {
            var serializedData = JsonSerializer.Serialize<List<ChapterSelectorElement>>(QElements);
            File.WriteAllText(bufferFile, serializedData);
        }
        public void ReadSelectionFromBuffer()
        {
            string serializedData = File.ReadAllText(bufferFile);
            try
            {
                JsonDocument doc = JsonDocument.Parse(serializedData);
                QElements =  JsonSerializer.Deserialize<List<ChapterSelectorElement>>(doc);
                qList.ItemsSource = QElements;
            }
            catch(JsonException ex)
            {
                Logger.Log(ex, "JsonException");
#if DEBUG
                MessageBox.Show("Error at reading json buffer file. Exception: " + ex.Message);
#endif
            }
        }
        #endregion
        #region Button events
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (CheckChoosedChapters())
            {
                ReadSelectedIndexes();
                SuccessfullEdit = true;
                this.Close();
            }
        }

        #endregion

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (qList.SelectedIndex > -1)
                SelectAllChapters(qList.SelectedIndex);
        }
        private void btnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            if (qList.SelectedIndex > -1)
                UnselectAllChapters(qList.SelectedIndex);
        }
        private void btnSelectForAllQuestions_Click(object sender, RoutedEventArgs e)
        {
            foreach(var question in QElements)
            {
                SelectAllChapters(question.Id - 1);
            }
        }
        private void btnUnselectForAllQuestions_Click(object sender, RoutedEventArgs e)
        {
            foreach (var question in QElements)
            {
                UnselectAllChapters(question.Id - 1);
            }
        }
    }
}
