using QDB.Database;
using QDB.Models;
using QDB.Models.Questions;
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
        private bool IsModified { get; set; }
        public string SearchQuery { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            _mwView = (MainWindowView)Resources["mwView"];
            
            QDbContext.GetInstance().RecreateDb();
            ReloadChapters();

            TesterButtonEvents();
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
                _mwView.SelectedChapter = _mwView.Chapters[0];
                ReloadSections(_mwView.Chapters[0].Id);
            }
        }
        private void ReloadQuestions()
        {
            if (_mwView.SelectedSection != null)
            {
                int sectionId = _mwView.SelectedSection.Id;
                _mwView.Questions.Clear();
                if (sectionId == 1)
                    _mwView.Questions.AddRange(QuestionExtensions.GetAll());
                else
                    _mwView.Questions.AddRange(QuestionExtensions.GetAll(_mwView.SelectedSection.Id));
            }
        }
        private void ReloadSections(int chapterId = 1)
        {
            _mwView.Sections.Clear();
            if (chapterId == 1)
                _mwView.Sections.AddRange(SectionsExtensions.GetAll());
            else
                _mwView.Sections.AddRange(SectionsExtensions.GetAll(chapterId));
            if (_mwView.Sections.Count > 0)
                _mwView.SelectedSection = _mwView.Sections[0];
        }
        private void Chapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Перезагружаем из базы подразделы, после смены раздела
            if (_mwView.SelectedChapter != null)
            {
                ReloadSections(_mwView.SelectedChapter.Id);   
            }
        }

        private void Section_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Обновляем список вопросов, соответствующих данному подразделу
            ReloadQuestions();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == MainWndCommands.cmdAddQuestion)
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

        }
        private void EditQuestion_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void RemoveQuestion_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {

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
