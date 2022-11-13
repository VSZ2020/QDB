using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.Charts;
using QDB.Database.Configurations;
using QDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.UserControls.Classes
{
    public class SectionElement: INotifyPropertyChanged
    {
        private bool _IsChecked = false;
        public string Header { get => Section?.Header ?? ""; }
        public QDbSection? Section { get; set; }
        public bool IsChecked { get => _IsChecked; set => SetIsChecked(value, true);}
        private ChapterElement Parent;

        public SectionElement(ChapterElement parent) {
            this.Parent = parent;
        }
        public void SetIsChecked(bool newValue, bool UpdateParent)
        {
            if (_IsChecked == newValue)
                return;
            _IsChecked = newValue;

            if (UpdateParent)
                Parent.CheckSectionsCheckState();

            OnPropertyChanged(nameof(IsChecked));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
