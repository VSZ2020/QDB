using QDB.Controllers;
using QDB.Database;
using QDB.Database.Configurations;
using QDB.Models;
using QDB.Models.Answers;
using QDB.Models.Questions;
using QDB.Utils.Logging;
using QDB.Views.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для QuestionEditForm.xaml
    /// </summary>
    public partial class QuestionEditForm : Window
    {
        public enum OperationType
        {
            Add,
            Edit
        }
        private OperationType _QuestionProcessingType;

        private QDbQuestion _EditedQuestion = null!;
        public bool EditResult { get; set; } = false;
        public ObservableCollection<QDbChapter> Chapters { get; set; } = new ObservableCollection<QDbChapter>();
        public ObservableCollection<QDbSection> Sections { get; set; } = new ObservableCollection<QDbSection>();
        public ObservableCollection<QDbAnswer> Answers { get; set; } = new ObservableCollection<QDbAnswer>();
        public Dictionary<QDbQuestion.QType, string> QuestionTypes { get; set; } = new Dictionary<QDbQuestion.QType, string> {
            { QDbQuestion.QType.Choise, "С выбором ответа" },
            { QDbQuestion.QType.Input,  "Ввод ответа" },
            { QDbQuestion.QType.Match,  "Сопоставление" }
        };
        public List<QDbDifficulty> Difficulties { get; set; } = new List<QDbDifficulty>();
        public QDbQuestion.QType? SelectedQuestionType { get; set; }
        public QDbChapter? SelectedChapter { get; set; }
        public QDbSection? SelectedSection { get; set; }
        public QDbDifficulty? SelectedDifficulty { get; set; }
        public QDbAnswer? SelectedAnswer { get; set; }
        public QuestionEditForm(QDbQuestion? question)
        {
            InitializeComponent();
            if (question != null)
            {
                _EditedQuestion = question;
                _QuestionProcessingType = OperationType.Edit;
            }
            else
            {
                _EditedQuestion = new QDbQuestion()
                {
                    Id = QuestionExtensions.Count() + 1,
                    ChapterId = QDatabaseConfig.AllCategoriesId,
                    SectionId = QDatabaseConfig.AllCategoriesId,
                    Difficulty = 1,
                    Type = QDbQuestion.QType.Choise,
                    Text = "Input question text here..."
                };
                _QuestionProcessingType = OperationType.Add;
            }
            DataContext = this;
            Init();
        }
        public QuestionEditForm():this(null){ }

        public void Init()
        {
            /*
             * Загружаем список разделов
             */
            ReloadChapters();
            //Выбираем раздел для вопроса
            SelectedChapter = Chapters.Where(c => c.Id == _EditedQuestion.ChapterId).FirstOrDefault(Chapters[0]);
            cbChapters.SelectedItem = SelectedChapter;

            ReloadSections(SelectedChapter?.Id ?? QDatabaseConfig.AllCategoriesId);
            //Выбираем подраздел
            //TODO: Не отображается на форме выбранный/найденый подраздел
            SelectedSection = Sections.Where(s => s.Id == _EditedQuestion.SectionId).FirstOrDefault(Sections[0]);
            cbSections.SelectedItem = SelectedSection;

            /*
             * Загружаем варианты трудоемкости вопроса
             */
            LoadDifficulties();

            //Выбираем трудоемкость из списка
            SelectedDifficulty = Difficulties.Where(d => d.Id == _EditedQuestion.Difficulty).FirstOrDefault(Difficulties[0]);

            //Выбираем тип вопроса
            SelectedQuestionType = QuestionTypes.Where(qt => qt.Key == _EditedQuestion.Type).FirstOrDefault().Key;

            //Заполняем текст вопроса
            TextRange rng = new TextRange(
                tbQuestionContent.Document.ContentStart,
                tbQuestionContent.Document.ContentEnd);
            rng.Text = _EditedQuestion.Text;

            //Задаем прикрепленное изображение вопроса, если есть
            AttachImage();

            //Обновляем список вопросов
            RecreateAnswers();

            btnOk.Click += BtnOk_Click;
        }

        

        private void ReloadChapters(bool KeepSelection = false)
        {
            QDbChapter cachedChapter = SelectedChapter;
            Chapters.Clear();
            Chapters.AddRange(ChaptersExtensions.GetAll());
            //Добавляем служебные поля
            Chapters.AddRange(QDbChapter.AddServiceFields());

            if (KeepSelection && Chapters.Contains(cachedChapter))
                SelectedChapter = cachedChapter;
        }

        private void ReloadSections(int chapterId)
        {
            Sections.Clear();
            Sections.AddRange(SectionsExtensions.GetAll(chapterId));
            //Добавляем служебные поля
            Sections.AddRange(QDbSection.AddServiceFields());
        }
        private void LoadDifficulties()
        {
            Difficulties.AddRange(DifficultiesExtensions.GetAll());
        }
        private void RecreateAnswers()
        {
            Answers.Clear();
            if (_QuestionProcessingType == OperationType.Edit)
            {
                Answers.AddRange(AnswersExtensions.GetAll(_EditedQuestion.Id));
            }
            //Если список ответов пуст, то создаем несколько дефолтных ответов
            if (_QuestionProcessingType == OperationType.Add && Answers.Count == 0)
            {
                for (int i = 0; i < Configuration.DefaultAnswersCount; i++)
                {
                    var answer = QDbAnswer.GetDefaultAnswer(_EditedQuestion.Id);
                    //answer.Id = i + 1;
                    answer.Content += $" {i + 1}";
                    Answers.Add(answer);
                }
                Answers[0].IsCorrect = true;
            }
        }
        private void AttachImage()
        {
            if (_QuestionProcessingType == OperationType.Edit)
            {
                //Ищем прикрепленное изображение и отображаем его, если изображение есть
                if (!string.IsNullOrEmpty(_EditedQuestion.PictureURI))
                {
                    //TODO: Загружаем изображение что-то
                }
            }
        }

        private void ChaptersEditorForm()
        {
            ChaptersListForm clf = new ChaptersListForm();
            clf.ShowDialog();
            if (clf.HasChanges)
            {
                ReloadChapters();
            }
        }

        /// <summary>
        /// Открывает окно добавления раздела в базу
        /// </summary>
        private void AddChapterForm()
        {
            EditChapterForm ecf = new EditChapterForm();
            ecf.ShowDialog();
            if (ecf.EditResult)
            {
                ReloadChapters();
                if (Chapters.Count > 2) SelectedChapter = Chapters[^3];
            }
            else
                cbChapters.SelectedItem = Chapters[0];
        }

        /// <summary>
        /// Открывает окно добавления подраздела в базу
        /// </summary>
        private void AddSectionForm()
        {
            int chapterId = SelectedChapter.Id;
            if (chapterId < 1)
                chapterId = 1;

            EditSectionForm esf = new EditSectionForm(chapterId);
            esf.ShowDialog();
            if (esf.EditResult)
            {
                ReloadSections(chapterId);
                if (Sections.Count > 1) cbSections.SelectedItem = Sections[^2];
            }
            else
                cbSections.SelectedItem = Sections[0];
        }

        /// <summary>
        /// Событие: Изменяется выбранный подраздел
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedChapter != null)
            {
                int chapterId = SelectedChapter.Id;
                if (chapterId > -1)
                {
                    _EditedQuestion.ChapterId = chapterId;
                    ReloadSections(chapterId);
                }
                else
                {
                    if (SelectedChapter.Id == Configuration.ServiceFieldId_Add)
                        AddChapterForm();
                    else if (SelectedChapter.Id == Configuration.ServiceFieldId_EditAll)
                        ChaptersEditorForm();
                }
            }
            
        }

        private void Section_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedSection != null)
            {
                int sectionId = SelectedSection.Id;
                if (sectionId > -1)
                {
                    _EditedQuestion.SectionId = sectionId;
                }
                else 
                if (sectionId == Configuration.ServiceFieldId_Add)
                    AddSectionForm();
            }
        }
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder questionFormErrors = new StringBuilder();
            if (SelectedChapter == null || 
                SelectedChapter.Id == Configuration.ServiceFieldId_Add || 
                SelectedChapter.Id == Configuration.ServiceFieldId_EditAll)
            {
                questionFormErrors.AppendLine("Необходимо выбрать раздел");
            }
            if (SelectedSection == null ||
                SelectedSection.Id == Configuration.ServiceFieldId_Add)
            {
                questionFormErrors.AppendLine("Необходимо выбрать подраздел");
            }
            TextRange tr = new TextRange(tbQuestionContent.Document.ContentStart, tbQuestionContent.Document.ContentEnd);
            if (string.IsNullOrEmpty(tr.Text))
            {
                questionFormErrors.AppendLine("Введите текст вопроса");
            }
            if (Answers.Count == 0)
            {
                questionFormErrors.AppendLine("Как минимум один ответ должен быть в вопросе");
            }
            if (questionFormErrors.Length > 0)
            {
                questionFormErrors.Insert(0, "Невозможно создать вопрос, так как есть ошибки.\nСписок ошибок:\n");
                MessageBox.Show(questionFormErrors.ToString(),
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            //Иначе создаем вопрос
            {
                //Задаем текст вопроса
                _EditedQuestion.Text = tr.Text;

                //Сохраняем прикрепленное изображение

                //Сохраняем ответы и сам вопрос
                if (_QuestionProcessingType == OperationType.Add)
                {
                    QuestionExtensions.Add(_EditedQuestion);
                    foreach(var answer in Answers)
                        AnswersExtensions.Add(answer);
                }
                else
                {
                    //Удаляем все ответы и добавляем заново
                    AnswersExtensions.Replace(_EditedQuestion.Id, Answers.ToList());
                    QuestionExtensions.Update(_EditedQuestion);
                }
                    
                EditResult = true;
                this.Close();
            }
        }

        private void Difficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _EditedQuestion.Difficulty = SelectedDifficulty?.Id ?? 1;
        }

        private void AnswersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            QuestionEditCommands.cmdEditAnswer.Execute(null, BtnEditAnswer);
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == QuestionEditCommands.cmdAddAnswer)
            {
                e.CanExecute = true;
            } else if (e.Command == QuestionEditCommands.cmdEditAnswer || e.Command == QuestionEditCommands.cmdRemoveAnswer)
            {
                e.CanExecute = SelectedAnswer != null;
            } 
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == QuestionEditCommands.cmdAddAnswer)
            {
                AnswerEditForm aef = new AnswerEditForm(this._EditedQuestion.Id);
                aef.ShowDialog();
                if (aef.EditCompleted)
                    Answers.Add(aef.EditedAnswer);
            }
            if (e.Command == QuestionEditCommands.cmdEditAnswer)
            {
                int selectedAnswerIndex = lvAnswers.SelectedIndex;
                AnswerEditForm aef = new AnswerEditForm(this._EditedQuestion.Id, SelectedAnswer.Copy());
                aef.ShowDialog();
                if (aef.EditCompleted && selectedAnswerIndex > -1)
                {
                    Answers[selectedAnswerIndex] = aef.EditedAnswer;
                    lvAnswers.Items.Refresh();
                }
            }
            if (e.Command == QuestionEditCommands.cmdRemoveAnswer)
            {
                if (SelectedAnswer != null)
                {
                    AnswersExtensions.Remove(SelectedAnswer);
                    Answers.Remove(SelectedAnswer);
                }
                
                //SelectedAnswer = null;
            }
        }
    }
}
