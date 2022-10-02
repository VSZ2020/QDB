using QDB.Database;
using QDB.Models;
using QDB.Utils.Logging;
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
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace QDB.Views
{
    /// <summary>
    /// Логика взаимодействия для ChaptersListForm.xaml
    /// </summary>
    public partial class ChaptersListForm : Window
    {
        public ObservableCollection<QDbChapter> Chapters { get; set; } = new ObservableCollection<QDbChapter>();
        public ObservableCollection<QDbSection> ChapterSections { get; set; } = new ObservableCollection<QDbSection>();
        public QDbChapter SelectedChapter { get; set; }
        public QDbSection SelectedSection { get; set; }

        public bool HasChanges { get; set; } = false;
        public ChaptersListForm()
        {
            InitializeComponent();
            DataContext = this;
            Init();
        }

        public void Init()
        {
            lvChapters.SelectionChanged += LvChapters_SelectionChanged;
            ReloadChapters();
            if (Chapters.Count > 0) 
                ReloadSections(Chapters[0]);
        }

        private void LvChapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedChapter != null)
            {
                ReloadSections(SelectedChapter);
            }
        }

        private void AddChapter()
        {
            EditChapterForm ecf = new EditChapterForm();
            ecf.ShowDialog();
            if (ecf.EditResult)
            {
                ReloadChapters();
                HasChanges = true;
            }
        }

        private void EditChapter()
        {
            if (SelectedChapter != null)
            {
                EditChapterForm ecf = new EditChapterForm(SelectedChapter);
                ecf.ShowDialog();
                if (ecf.EditResult)
                {
                    ReloadChapters();
                    HasChanges = true;
                }
            }
            else
                MessageBox.Show(
                    "Не выбран раздел для редактирования!",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
        }

        private void RemoveChapter()
        {
            if (SelectedChapter != null)
            {
                if (SelectedChapter.Id == 1)
                {
                    MessageBox.Show(
                        "Вы не можете удалить раздел по-умолчанию",
                        "Внимание",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }
                ChaptersExtensions.Remove(SelectedChapter, true);
                ReloadChapters();
                HasChanges = true;
            }
            else
                MessageBox.Show(
                    "Не выбран раздел для удаления!",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
        }

        private void AddSection()
        {
            if (SelectedChapter != null)
            {
                EditSectionForm esf = new EditSectionForm(SelectedChapter.Id);
                esf.ShowDialog();
                if (esf.EditResult)
                {
                    ReloadSections(SelectedChapter);
                    HasChanges = true;
                }
            }
            else
                MessageBox.Show("Для начала выберите раздел", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void EditSection()
        {
            if (SelectedChapter != null)
            {
                if (SelectedSection != null)
                {
                    EditSectionForm esf = new EditSectionForm(SelectedChapter.Id, SelectedSection);
                    esf.ShowDialog();
                    if (esf.EditResult)
                    {
                        ReloadSections(SelectedChapter);
                        HasChanges = true;
                    }
                }
                MessageBox.Show(
                    "Не выбран подраздел для редактирования!",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
                MessageBox.Show(
                    "Выберите основной раздел",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
        }
        private void RemoveSection()
        {
            if (SelectedChapter == null)
            {
                MessageBox.Show("Для начала выберите раздел","Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedSection != null)
            {
                if (SelectedChapter.Id == 1 && SelectedSection.Id == 1)
                {
                    MessageBox.Show(
                        "Вы не можете удалить подраздел по-умолчанию",
                        "Внимание",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }
                SectionsExtensions.Remove(SelectedSection, true);
                ReloadSections(SelectedChapter);
                HasChanges = true;
            }
            else
                MessageBox.Show(
                    "Не выбран подраздел для удаления!", 
                    "Внимание", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
        }
        private void ReloadChapters()
        {
            Chapters.Clear();
            var chpt = ChaptersExtensions.GetAll();
            for (int i = 0; i < chpt.Count; i++)
                Chapters.Add(chpt[i]);
            if (Chapters.Count > 0)
                SelectedChapter = Chapters[0];
        }

        private void ReloadSections(QDbChapter chapter)
        {
            ChapterSections.Clear();
            //Если выбран раздел "Все категории", то выводим все подкатегории всех категорий
            var chpt = (chapter.Id == 1) ? SectionsExtensions.GetAll() : SectionsExtensions.GetAll(chapter.Id);
            for (int i = 0; i < chpt.Count; i++)
                ChapterSections.Add(chpt[i]);
            if (ChapterSections.Count > 0)
                SelectedSection = ChapterSections[0];
        }


        private void ChapterEditRemove_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedChapter != null;
            
        }
        private void SectionEditRemove_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedSection != null;
        }

        private void ChapterAdd_Executed(object sender, ExecutedRoutedEventArgs e) => AddChapter();
        private void ChapterEdit_Executed(object sender, ExecutedRoutedEventArgs e) => EditChapter();
        private void ChapterRemove_Executed(object sender, ExecutedRoutedEventArgs e) => RemoveChapter();
        private void SectionAdd_Executed(object sender, ExecutedRoutedEventArgs e) => AddSection();
        private void SectionEdit_Executed(object sender, ExecutedRoutedEventArgs e) => EditSection();
        private void SectionRemove_Executed(object sender, ExecutedRoutedEventArgs e) => RemoveSection();
    }
}
