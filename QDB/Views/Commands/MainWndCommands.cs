using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QDB.Views.Commands
{
    public static class MainWndCommands
    {
        public static RoutedUICommand cmdOpen = new RoutedUICommand(
            "Open",
            "Open",
            typeof(MainWindow),
            new InputGestureCollection()
            {
                new KeyGesture(Key.O, ModifierKeys.Control)
            });
        public static RoutedUICommand cmdSave = new RoutedUICommand(
            "Save",
            "Save",
            typeof(MainWindow),
            new InputGestureCollection()
            {
                new KeyGesture(Key.S, ModifierKeys.Control)
            });
        public static RoutedUICommand cmdGenerate = new RoutedUICommand(
            "Generate",
            "Generate",
            typeof(MainWindow));
        public static RoutedUICommand cmdLoadFromText = new RoutedUICommand(
            "From TXT",
            "LoadFromText",
            typeof(MainWindow));
        public static RoutedUICommand cmdLoadFromExcel = new RoutedUICommand(
            "From Excel",
            "LoadFromExcel",
            typeof(MainWindow));
        public static RoutedUICommand cmdClearDatabase = new RoutedUICommand(
            "Clear",
            "ClearDatabase",
            typeof(MainWindow));
        public static RoutedUICommand cmdExit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(MainWindow),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            });
        public static RoutedUICommand cmdAddQuestion = new RoutedUICommand(
            "Add question",
            "AddQuestio",
            typeof(MainWindow),
            new InputGestureCollection()
            {
                new KeyGesture(Key.D, ModifierKeys.Control)
            });
        public static RoutedUICommand cmdEditQuestion = new RoutedUICommand(
            "Edit question",
            "EditQuestion",
            typeof(MainWindow),
            new InputGestureCollection()
            {
                new KeyGesture(Key.E, ModifierKeys.Control)
            });
        public static RoutedUICommand cmdRemoveQuestion = new RoutedUICommand(
            "Remove Question",
            "RemoveQuestion",
            typeof(MainWindow),
            new InputGestureCollection()
            {
                new KeyGesture(Key.X, ModifierKeys.Control)
            });
    }
}
