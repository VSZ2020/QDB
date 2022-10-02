using QDB.Controllers;
using QDB.Database;
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
        public QDbQuestion.QType SelectedQuestionType { get; set; }
        public QDbChapter SelectedChapter { get; set; }
        public QDbSection SelectedSection { get; set; }
        public QDbDifficulty SelectedDifficulty { get; set; }
        public QDbAnswer SelectedAnswer { get; set; }
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
                    Id = QuestionExtensions.Count(),
                    ChapterId = -1,
                    SectionId = -1,
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
            //Выбираем раздел в зависимости от типа операции
            if (_QuestionProcessingType == OperationType.Add)
                SelectedChapter = Chapters[0];
            else
                SelectedChapter = Chapters.Where(c => c.Id == _EditedQuestion.ChapterId).FirstOrDefault() ?? Chapters[0];

            /*
             * Загружаем список подразделов для данного раздела
             */
            //ReloadSections(Chapters[0]);
            
            //Выбираем первый подраздел как подраздел по-умолчанию
            //if (_EditedQuestion.SectionId == -1)
            //    SelectedSection = Sections[0];
            //else
            //    SelectedSection = Sections.Where(s => s.Id == _EditedQuestion.SectionId).FirstOrDefault() ?? Sections[0];

            /*
             * Загружаем варианты трудоемкости вопроса
             */
            LoadDifficulties();

            //Выбираем трудоемкость из списка
            SelectedDifficulty = Difficulties.Where(d => d.Id == _EditedQuestion.Difficulty).FirstOrDefault() ?? Difficulties[0];

            //Заполняем текст вопроса
            tbQuestionContent.AppendText(_EditedQuestion.Text);

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

        private void ReloadSections(QDbChapter chapter)
        {
            Sections.Clear();
            //Если выбран раздел "Все категории", то выводим все подкатегории всех категорий
            if (chapter.Id == 1)
                Sections.AddRange(SectionsExtensions.GetAll());
            else
            {
                Sections.AddRange(SectionsExtensions.GetAll(chapter.Id));
            }
            //Добавляем служебные поля
            Sections.AddRange(QDbSection.AddServiceFields());
        }
        private void LoadDifficulties()
        {
            Difficulties.AddRange(DifficultiesExtensions.GetAll());
        }
        private void RecreateAnswers()
        {
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

        private void AddChapterForm()
        {
            EditChapterForm ecf = new EditChapterForm();
            ecf.ShowDialog();
            if(ecf.EditResult)
                ReloadChapters();
        }

        private void AddSectionForm()
        {
            EditSectionForm esf = new EditSectionForm(SelectedChapter.Id);
            esf.ShowDialog();
            if (esf.EditResult)
                ReloadSections(SelectedChapter);
        }

        private void Chapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedChapter != null)
            {
                if (SelectedChapter.Id > -1)
                {
                    _EditedQuestion.ChapterId = SelectedChapter.Id;
                    ReloadSections(SelectedChapter);
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
            //ComboBox cbox = (ComboBox)e.Source;
            //int addItemIndex = cbox.Items.Count - 1;
            if (SelectedSection != null)
            {
                if (SelectedSection.Id > -1)
                {
                    _EditedQuestion.SectionId = SelectedSection.Id;
                }
                else 
                if (SelectedSection.Id == Configuration.ServiceFieldId_Add)
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
            _EditedQuestion.Difficulty = SelectedDifficulty.Id;
        }

        private void AnswersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedAnswer != null)
            {
                AnswerEditForm aef = new AnswerEditForm(SelectedAnswer.QuestionId, SelectedAnswer);
                aef.ShowDialog();
            }
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
            }
            if (e.Command == QuestionEditCommands.cmdEditAnswer)
            {
                AnswerEditForm aef = new AnswerEditForm(this._EditedQuestion.Id, SelectedAnswer);
                aef.ShowDialog();
            }
            if (e.Command == QuestionEditCommands.cmdRemoveAnswer)
            {
                Answers.Remove(SelectedAnswer);
                //SelectedAnswer = null;
            }
        }
    }
}
