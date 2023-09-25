using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;

namespace ApplicationDesktopPointeuse
{
    /// <summary>
    /// Interaction logic for ChoicesWindow.xaml
    /// </summary>
    public partial class ChoicesWindow : Window
    {
        public ChoicesWindow()
        {
            InitializeComponent();
        }

        private void studentRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Choices.Content = new StudentsPage();
        }

        private void schoolclassesRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Choices.Content = new SchoolclassesPage();
        }

        private void cyclesRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Choices.Content = new CyclesPage();
        }

        private void sectionsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Choices.Content = new SectionsPage();
        }

        private void subsectionsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Choices.Content = new SubsectionsPage();
        }
    }
}