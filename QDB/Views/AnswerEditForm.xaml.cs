using QDB.Database;
using QDB.Models.Answers;
using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
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
    /// Логика взаимодействия для AnswerEditForm.xaml
    /// </summary>
    public partial class AnswerEditForm : Window
    {
        private ProcessOperationType _operation;
        private QDbAnswer EditedAnswer { get; set; }
        public Dictionary<QDbAnswer.AType, string> AnswerTypes { get; set; } = new Dictionary<QDbAnswer.AType, string> {
            { QDbAnswer.AType.text, "Текстовый" },
            { QDbAnswer.AType.input,  "Ввод текста" },
            { QDbAnswer.AType.image,  "Картинка" }
        };
        public bool IsCorrect { get; set; } = true;
        public string AnswerContent { get; set; } = "Input answer here";
        public QDbAnswer.AType SelectedAnswerType { get; set; }
        public bool IsAttachEnabled { get; set; } = false;
        public AnswerEditForm(int questonId, QDbAnswer? answer = null)
        {
            InitializeComponent();
            if (answer == null)
            {
                _operation = ProcessOperationType.Add;
                EditedAnswer = new QDbAnswer() { Type = QDbAnswer.AType.text, QuestionId = questonId};
            }
            else
            {
                _operation = ProcessOperationType.Edit;
                EditedAnswer = answer;
            }
            Init(); 
        }

        public void Init()
        {
            DataContext = this;
            if (_operation == ProcessOperationType.Add)
            {
                SelectedAnswerType = QDbAnswer.AType.text;
                
            }
            else
            {
                SelectedAnswerType = AnswerTypes.Where(t => t.Key == EditedAnswer.Type).FirstOrDefault().Key;
                AnswerContent = EditedAnswer.Content;
                IsCorrect = EditedAnswer.IsCorrect;
            }
            btnOk.Click += (object sender, RoutedEventArgs e) =>
            {
                if (CheckInputs())
                {
                    this.Close();
                }
            };
        }

        private bool CheckInputs()
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(AnswerContent))
            {
                errors.AppendLine("Содержимое вопроса не может быть пустым полем");
            }
            if (errors.Length > 0)
            {
                errors.Insert(0, "Есть ошибки во входных данных:\n");
                MessageBox.Show(errors.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            EditedAnswer.IsCorrect = IsCorrect;
            EditedAnswer.Type = SelectedAnswerType;
            EditedAnswer.Content = AnswerContent;
            if (_operation == ProcessOperationType.Add)
            {
                AnswersExtensions.Add(EditedAnswer);
            }
            else
                AnswersExtensions.Update(EditedAnswer);
            return true;
        }

        private void AnswerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsAttachEnabled = SelectedAnswerType == QDbAnswer.AType.image;
        }
    }
}
