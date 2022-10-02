using QDB.Database;
using QDB.Models;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EditSectionForm.xaml
    /// </summary>
    public partial class EditSectionForm : Window
    {
        private ProcessOperationType _operationType;
        public bool EditResult { get; set; } = false;
        public QDbSection EditedSection { get; set; }
        public EditSectionForm(int parentId, QDbSection? section = null)
        {
            InitializeComponent();
            if (section == null)
            {
                EditedSection = new QDbSection() { Header = "Section", ChapterId = parentId };
                _operationType = ProcessOperationType.Add;
            }
            else
            {
                EditedSection = section;
                tbSectionHeader.Text = EditedSection.Header;
                _operationType = ProcessOperationType.Edit;
            }
            EventHandlers();
        }

        public void EventHandlers()
        {
            btnOk.Click += (object sender, RoutedEventArgs e) =>
            {
                if (!string.IsNullOrEmpty(tbSectionHeader.Text))
                    EditedSection.Header = tbSectionHeader.Text;
                else
                {
                    MessageBox.Show(
                        "Имя подраздела не может быть пустым!",
                        "Внимание",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
                //В зависимости от выполняемой операции делать...
                if (_operationType == ProcessOperationType.Add)
                {
                    SectionsExtensions.Add(EditedSection);
                }
                else
                {
                    SectionsExtensions.Update(EditedSection);
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
