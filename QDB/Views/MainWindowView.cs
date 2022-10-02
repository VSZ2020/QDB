using QDB.Models.Questions;
using QDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Views
{
    public class MainWindowView : INotifyPropertyChanged
    {
        #region InfoMessage
        private string _infoMsgHeader = string.Empty;
        private string _infoMsgText = string.Empty;
        private bool _infoMsgVisibile = false;
        public string InfoMessageHeader { get => _infoMsgHeader;
            set
            {
                _infoMsgHeader = value;
                OnRaised(nameof(InfoMessageHeader));
            }
        }
        public string InfoMessageText { get => _infoMsgText;
            set
            {
                _infoMsgText = value;
                OnRaised(nameof(InfoMessageText));
            }
        }
        public bool InfoMessageVisibility
        {
            get => _infoMsgVisibile;
            set
            {
                _infoMsgVisibile = value;
                OnRaised(nameof(InfoMessageVisibility));
            }
        }
        public void ShowMessage(string title, string message)
        {
            InfoMessageVisibility = true;
            InfoMessageHeader = title;
            InfoMessageText = message;
        }
        public void ClearMessage()
        {
            InfoMessageHeader = string.Empty;
            InfoMessageText = string.Empty;
            InfoMessageVisibility = false;
        }
        #endregion InfoMessage
        public ObservableCollection<QDbChapter> Chapters { get; set; } = new ObservableCollection<QDbChapter>();
        public ObservableCollection<QDbSection> Sections { get; set; } = new ObservableCollection<QDbSection>();

        public ObservableCollection<QDbQuestion> Questions { get; set; } = new ObservableCollection<QDbQuestion>();

        public QDbQuestion? SelectedQuestion { get; set; }
        public QDbChapter? SelectedChapter { get; set; }
        public QDbSection? SelectedSection { get; set; }

        #region INotifyPropertyChanged region
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnRaised(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged region
    }
}
