using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QDB.Views.Commands
{
    public static class QuestionEditCommands
    {
        public static RoutedUICommand cmdAddAnswer = new RoutedUICommand(
            "Add answer",
            "AddAnswer",
            typeof(QuestionEditForm),
            new InputGestureCollection()
            {
                new KeyGesture(Key.A, ModifierKeys.Control)
            });
        public static RoutedUICommand cmdEditAnswer = new RoutedUICommand(
            "Edit answer",
            "EditAnswer",
            typeof(QuestionEditForm),
            new InputGestureCollection()
            {
                new KeyGesture(Key.E, ModifierKeys.Control)
            });
        public static RoutedUICommand cmdRemoveAnswer = new RoutedUICommand(
            "Remove answer",
            "RemoveAnswer",
            typeof(QuestionEditForm),
            new InputGestureCollection()
            {
                new KeyGesture(Key.X, ModifierKeys.Control)
            });
    }
}
