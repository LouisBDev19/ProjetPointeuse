using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;

namespace ApplicationDesktopPointeuse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void studentRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Main.Content = new StudentsPage();
        }

        private void schoolclassesRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Main.Content = new SchoolclassesPage();
        }

        private void cyclesRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Main.Content = new CyclesPage();
        }

        private void sectionsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Main.Content = new SectionsPage();
        }

        private void subsectionsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Main.Content = new SubsectionsPage();
        }
    }
}
