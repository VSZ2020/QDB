using DocumentFormat.OpenXml.InkML;
using Microsoft.Win32;
using QDB.Database;
using QDB.Models;
using QDB.Models.Questions;
using QDB.Utils.Readers;
using QDB.Views;
using QDB.Views.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowView _mwView;
        private bool IsModified { get; set; } = false;
        public string SearchQuery { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            _mwView = (MainWindowView)Resources["mwView"];

            _mwView.ShowMessage("Загрузка", "Инициализация программы");
            ReloadChapters();
            questionsListView.SelectionChanged += QuestionsListView_SelectionChanged;
            questionsListView.MouseDoubleClick += QuestionsListView_MouseDoubleClick;
            TesterButtonEvents();
            _mwView.ClearMessage();
        }

        

        private void TesterButtonEvents()
        {
            TesterBtn_1.Click += (object sender, RoutedEventArgs e) =>
            {
                QuestionEditForm qef = new QuestionEditForm();
                qef.ShowDialog();
                ReloadChapters();
                ReloadQuestions();
            };
            TesterBtn_2.Click += (object sender, RoutedEventArgs e) =>
            {
                ChaptersListForm clf = new ChaptersListForm();
                clf.ShowDialog();
                if (clf.HasChanges)
                    ReloadChapters();
            };
        }
        private void ReloadChapters()
        {
            _mwView.Chapters.Clear();
            _mwView.Chapters.AddRange(ChaptersExtensions.GetAll());
            if (_mwView.Chapters.Count > 0)
            {
                cbChapters.SelectedItem = _mwView.Chapters[0];
                ReloadSections(_mwView.Chapters[0].Id);
            }
        }
        private void ReloadQuestions()
        {
            int chapterId = _mwView.SelectedChapterId ?? -1;
            int sectionId = _mwView.SelectedSectionId ?? -1;
            System.Diagnostics.Trace.WriteLine($"ChapterID: {chapterId}. SectionID: {sectionId}");
            if (chapterId > -1)
            {
                if (sectionId == -1)
                {
                    sectionId = 0;
                }
                _mwView.Questions.Clear();
                _mwView.Questions.AddRange(QuestionExtensions.GetAll(chapterId, sectionId));
            }
        }
        private void ReloadSections(int chapterId = 0)
        {
            _mwView.Sections.Clear();
            _mwView.Sections.AddRange(SectionsExtensions.GetAll(chapterId));
            if (_mwView.Sections.Count > 0)
                cbSections.SelectedItem = _mwView.Sections[0];
            ReloadQuestions();
        }
        private void Chapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int chapterId = _mwView.SelectedChapterId ?? 0;
            //Перезагружаем из базы подразделы, после смены раздела
            ReloadSections(chapterId);
        }

        private void Section_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Обновляем список вопросов, соответствующих данному подразделу
            ReloadQuestions();
            _mwView.StatusLabelText = $"Questions count: {_mwView.Questions.Count} | Chapter ID: {_mwView.SelectedChapterId ?? -1} | Section ID: {_mwView.SelectedSectionId ?? -1} |";
        }
        private void QuestionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mwView.SelectedQuestion != null)
            {
                _mwView.StatusLabelText = $"Question chapter ID: {_mwView.SelectedQuestion.ChapterId}, section ID: {_mwView.SelectedSectionId ?? -1} |";
            }
            
        }

        private void QuestionsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditQuestion_Executed(this, null);
        }
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == MainWndCommands.cmdAddQuestion || 
                e.Command == MainWndCommands.cmdOpen ||
                e.Command == MainWndCommands.cmdLoadFromText ||
                e.Command == MainWndCommands.cmdLoadFromExcel ||
                e.Command == MainWndCommands.cmdClearDatabase ||
                e.Command == MainWndCommands.cmdGenerate ||
                e.Command == MainWndCommands.cmdExit)
            {
                e.CanExecute = true;
            }
            if (e.Command == MainWndCommands.cmdEditQuestion || e.Command == MainWndCommands.cmdRemoveQuestion)
            {
                e.CanExecute = _mwView?.SelectedQuestion != null;
            }
            if (e.Command == MainWndCommands.cmdSave)
            {
                e.CanExecute = IsModified;
            }
        }

        private void AddQuestion_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            QuestionEditForm qef = new QuestionEditForm();
            qef.ShowDialog();
            ReloadChapters();
        }
        private void EditQuestion_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_mwView.SelectedQuestion != null)
            {
                QuestionEditForm qef = new QuestionEditForm(_mwView.SelectedQuestion);
                qef.ShowDialog();
                ReloadChapters();
                
            }
        }
        private void RemoveQuestion_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_mwView.SelectedQuestion != null)
            {
                QuestionExtensions.Remove(_mwView.SelectedQuestion);
                ReloadQuestions();
            }
        }
        private void Generate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            QuestionTestGenerator qte = new QuestionTestGenerator();
            qte.ShowDialog();
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void ImportFrom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mwView.ShowMessage("Импрот", "Загрузка из внешнего файла. Пожалуйста подождите");
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "All files (*.*)|*.*";
            if (e.Command == MainWndCommands.cmdLoadFromExcel)
                dialog.Filter.Insert(0, "Microsoft Excel file (*.xlsx)|*.xlsx|");
            dialog.FilterIndex = 0;
            var isOk = dialog.ShowDialog();
            if (!isOk.HasValue || !isOk.Value)
                return;

            if (e.Command == MainWndCommands.cmdLoadFromExcel)
            {
                IDatabaseReader reader = new ExcelReader();
                reader.LoadQuestions(dialog.FileName);
            }
            if (e.Command == MainWndCommands.cmdLoadFromText)
            {
                //IDatabaseReader reader = new ExcelReader();
                //reader.LoadQuestions(dialog.FileName);
            }
            ReloadChapters();
            _mwView.ClearMessage();
        }
        private void Clear_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var diagResult = MessageBox.Show(
                "Are you sure, that you want totally clear database?",
                "Warning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (diagResult == MessageBoxResult.Yes)
            {
                QDbContext.GetInstance().RecreateDb();
                ReloadQuestions();
            }
        }
        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            About aboutForm = new About();
            aboutForm.ShowDialog();
        }
    }
}
