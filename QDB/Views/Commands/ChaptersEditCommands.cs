using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QDB.Views.Commands
{
    public static class ChaptersEditCommands
    {
        public static RoutedUICommand cmdAddChapter = new RoutedUICommand(
            "Add Chapter",
            "AddChapter",
            typeof(ChaptersListForm));
        public static RoutedUICommand cmdEditChapter = new RoutedUICommand(
            "Edit Chapter",
            "EditChapter",
            typeof(ChaptersListForm));
        public static RoutedUICommand cmdRemoveChapter = new RoutedUICommand(
            "Remove Chapter",
            "RemoveChapter",
            typeof(ChaptersListForm));

        public static RoutedUICommand cmdAddSection = new RoutedUICommand(
            "Add Section",
            "AddSection",
            typeof(ChaptersListForm));
        public static RoutedUICommand cmdEditSection = new RoutedUICommand(
            "Edit Section",
            "EditSection",
            typeof(ChaptersListForm));
        public static RoutedUICommand cmdRemoveSection = new RoutedUICommand(
            "Remove Section",
            "RemoveSection",
            typeof(ChaptersListForm));
    }
}
