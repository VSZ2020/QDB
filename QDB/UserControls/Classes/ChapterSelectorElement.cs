using QDB.Models;
using QDB.Utils.Generator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.UserControls.Classes
{
    public class ChapterSelectorElement
    {
        public int Id { get; set; }
        /// <summary>
        /// Наизвание группы разделов (номер вопроса)
        /// </summary>
        public string GroupName { get; set; } = string.Empty;
        /// <summary>
        /// Массив доступных сложностей
        /// </summary>
        public List<QDbDifficulty> Difficulties { get; set; } = new();
        /// <summary>
        /// Индекс выбранной сложности
        /// </summary>
        public int SelectedDifficultyIndex { get; set; } = 0;
        /// <summary>
        /// Список разделов с флагами (выбран/не выбран)
        /// </summary>
        public ObservableCollection<ChapterElement> Chapters { get; set; } = new();

    }
}
