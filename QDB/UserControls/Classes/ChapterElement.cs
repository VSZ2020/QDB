using QDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.UserControls.Classes
{
    /// <summary>
    /// Раздел с логическим полем (выбран/не выбран)
    /// </summary>
    public class ChapterElement: INotifyPropertyChanged
    {
        private bool? _IsChecked = false;
        public string Header { get => Chapter.Header; }
        public QDbChapter Chapter { get; set; } = new();
        public bool? IsChecked
        {
            get => _IsChecked; set
            {
                SetIsChecked(value, true);
            }
        }
        public List<SectionElement> Sections { get; set; } = new();

        public void CheckSectionsCheckState()
        {
            if (Sections == null)
                return;
            bool? thisState = null;
            for (int i = 0; i < Sections.Count; i++)
            {
                bool sectionState = Sections[i].IsChecked;
                if (i == 0)
                {
                    thisState = sectionState;
                }
                else if (thisState != sectionState)
                {
                    thisState = null;
                    break;
                }
            }
            this.SetIsChecked(thisState, false);

        }
        public void SetIsChecked(bool? newValue, bool UpdateChildren)
        {
            if (_IsChecked == newValue)
                return;
            _IsChecked = newValue;
            if (UpdateChildren && Sections.Count > 0)
            {
                if (newValue.HasValue)
                    Sections.ForEach(s => s.IsChecked = newValue.Value);
            }
            OnPropertyChanged(nameof(IsChecked));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
