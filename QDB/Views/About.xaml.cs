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
    /// Логика взаимодействия для About.xaml
    /// </summary>
    public partial class About : Window
    {
        public bool HasUpdate { get; set; } = false;
        public About()
        {
            InitializeComponent();
        }

        private void btnCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                HasUpdate = true;
            }
        }
    }
}
