using QDB.Utils.Generator;
using QDB.Utils.Writers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для QuestionTestGenerator.xaml
    /// </summary>
    public partial class QuestionTestGenerator : Window
    {
        public enum ExportFileFormats
        {
            XLSX, DOCX, TXT
        }
        public Dictionary<ExportFileFormats, string /*Format name*/> Formats { get; set; } = new()
        {
            {ExportFileFormats.XLSX, "Excel (*.xlsx)" },
            {ExportFileFormats.DOCX, "Word (*.docx)" },
            {ExportFileFormats.TXT, "Plain Text (*.txt)" }
        };
        public ExportFileFormats SelectedFormatType { get; set; }
        public int VariantsAmount { get; set; } = 1;
        public int QuestionsCount { get; set; } = 1;
        public string OutputPath { get; set; } = string.Empty;
        public List<QuestionGenData>? GenQuestions { get; set; }

        private List<ExportFileFormats> _FormatsToExport;
        public bool IsGenerated { get; private set; } = false;
        public List<QVariant> Variants { get; set; } = new();
        public QuestionTestGenerator()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Init()
        {
            cbFileType.DataContext = this;
            //cbFileType.ItemsSource = Formats;
            cbFileType.SelectedIndex = 0;
        }
        public bool CheckQuestionsCountField()
        {
            int questionsCount = 1;
            if (!int.TryParse(tbQuestionsCount.Text, out questionsCount) || questionsCount < 1)
            {
                return false;
            }
            else
                QuestionsCount = questionsCount;
            return true;
        }

        private bool CheckInputs()
        {
            StringBuilder errorsMsg = new();
            int variantsCount = 1;
            if (!int.TryParse(tbVariantsCount.Text, out variantsCount) || variantsCount < 1)
            {
                errorsMsg.AppendLine("- Поле с количеством вариантов содержит неверное значение");
            }
            else
                VariantsAmount = variantsCount;

            if (CheckQuestionsCountField())
            {
                errorsMsg.AppendLine("- Поле с количеством вопросов содержит неверное значение");
            }

            if (string.IsNullOrEmpty(tbOutputPath.Text))
            {
                errorsMsg.AppendLine("- Поле с конечной папкой для файлов не может быть пустым");
            }
            else if (!Directory.Exists(tbOutputPath.Text))
            {
                errorsMsg.AppendLine("- Конечной папки не существует. Проверьте введенный путь");
            }
            else
                OutputPath = tbOutputPath.Text;

            if (GenQuestions == null || GenQuestions.Count == 0)
            {
                errorsMsg.AppendLine("- Не выбраны разделы для подбора вопросов");
            }
            if (errorsMsg.Length > 0)
            {
                MessageBox.Show(
                    errorsMsg.ToString(),
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            return errorsMsg.Length == 0;
        }
        private void btnChooseChapters_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckQuestionsCountField())
            {
                MessageBox.Show(
                    "Поле с количеством вопросов содержит неверное значение",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            ChaptersChooseWindow ccw = new ChaptersChooseWindow(QuestionsCount);
            ccw.ShowDialog();
            if (!ccw.SuccessfullEdit)
                return;
            //Выбранные индексы
            GenQuestions = ccw.Questions;

            //Отображаем выбранные индексы разделов
            string fullLabel = "Chapters by question ID: ";
            for (int i = 0; i < GenQuestions.Count; i++)
            {
                fullLabel += "[";
                foreach(var chapter in GenQuestions[i].ChaptersIds.Indexes)
                {
                    for(int j = 0; j < chapter.Value.Count; j++)
                        fullLabel += Convert.ToString(chapter.Value[j]) + ",";
                }
                fullLabel.TrimEnd(',');
                fullLabel += "], ";
            }
            fullLabel.TrimEnd().TrimEnd(',');
            lbChaptersById.Text = fullLabel;
        }
        private List<QVariant> GenerateVariants()
        {
            QTestGenerator generator = new QTestGenerator();
            List<QVariant> variants = generator.Generate(GenQuestions, VariantsAmount);
            return variants;
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInputs())
            {
                Variants = GenerateVariants();
                WordExporter wexp = new WordExporter();
                wexp.Export(System.IO.Path.Combine(OutputPath, "TestVariants.docx"), Variants, "Вводный тест");
            }
        }

        private void btnOutPath_Click(object sender, RoutedEventArgs e)
        {
            OutputPath = Environment.CurrentDirectory;
            Trace.WriteLine($"Output path set to {OutputPath}");
        }
    }
}
