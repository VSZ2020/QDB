using QDB.Database;
using QDB.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
    /// Логика взаимодействия для AddChapterForm.xaml
    /// </summary>
    public partial class EditChapterForm : Window
    {
        private ProcessOperationType _operationType;
        public bool EditResult { get; set; } = false;
        public QDbChapter EditedChapter { get; set; }
        public EditChapterForm(QDbChapter? chapter = null)
        {
            InitializeComponent();
            if (chapter == null)
            {
                EditedChapter = new QDbChapter() { Header = "Chapter" };
                _operationType = ProcessOperationType.Add;
            }
            else
            {
                EditedChapter = chapter;
                tbChapterHeader.Text = EditedChapter.Header;
                _operationType = ProcessOperationType.Edit;
            }
            EventHandlers();
        }

        public void EventHandlers()
        {
            btnOk.Click += (object sender, RoutedEventArgs e) =>
            {
                if (!string.IsNullOrEmpty(tbChapterHeader.Text))
                    EditedChapter.Header = tbChapterHeader.Text;
                else
                {
                    MessageBox.Show(
                        "Имя раздела не может быть пустым!",
                        "Внимание",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
                //В зависимости от выполняемой операции делать...
                if (_operationType == ProcessOperationType.Add)
                {
                    ChaptersExtensions.Add(EditedChapter);
                }
                else
                {
                    ChaptersExtensions.Update(EditedChapter);
                }
                EditResult = true;
                this.Close();
            };
            btnCancel.Click += (object sender, RoutedEventArgs e) =>
            {
                //EditResult = false;
                //this.Close();
            };
        }
    }
}
