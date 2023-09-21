using APIPointeuse.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ApplicationDesktopPointeuse
{
    /// <summary>
    /// Logique d'interaction pour SchoolclassesPage.xaml
    /// </summary>
    public partial class SchoolclassesPage : UserControl
    {
        private ObservableCollection<Schoolclasses> allSchoolclasses;
        public ObservableCollection<Cycles> cyclesList { get; set; }
        public ObservableCollection<Sections> sectionsList { get; set; }
        public ObservableCollection<Subsections> subsectionsList { get; set; }
        public SchoolclassesPage()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            AddSchoolclassButton.Visibility = Visibility.Hidden;
            SaveSchoolclassButton.Visibility = Visibility.Hidden;
            ArchiveSchoolclassButton.Visibility = Visibility.Hidden;
            DeleteSchoolclassButton.Visibility = Visibility.Hidden;
            cyclesList = new ObservableCollection<Cycles>();
            sectionsList = new ObservableCollection<Sections>();
            subsectionsList = new ObservableCollection<Subsections>();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                var CyclesResponse = await _httpClient.GetAsync("https://localhost:7026/api/Cycles/getCyclesList");
                if (CyclesResponse.IsSuccessStatusCode)
                {
                    var CyclesContent = await CyclesResponse.Content.ReadAsStringAsync();
                    var cycles = JsonConvert.DeserializeObject<List<Cycles>>(CyclesContent);

                    foreach (var cycle in cycles)
                    {
                        cyclesList.Add(cycle);
                    }

                    DataContext = this;
                }

                var SectionsResponse = await _httpClient.GetAsync("https://localhost:7026/api/Sections/getSectionsList");
                if (SectionsResponse.IsSuccessStatusCode)
                {
                    var SectionsContent = await SectionsResponse.Content.ReadAsStringAsync();
                    var sections = JsonConvert.DeserializeObject<List<Sections>>(SectionsContent);

                    foreach (var section in sections)
                    {
                        sectionsList.Add(section);
                    }

                    DataContext = this;
                }

                var SubsectionsResponse = await _httpClient.GetAsync("https://localhost:7026/api/Subsections/getSubsectionsList");
                if (SubsectionsResponse.IsSuccessStatusCode)
                {
                    var SubsectionsContent = await SubsectionsResponse.Content.ReadAsStringAsync();
                    var subsections = JsonConvert.DeserializeObject<List<Subsections>>(SubsectionsContent);

                    foreach (var subsection in subsections)
                    {
                        subsectionsList.Add(subsection);
                    }

                    DataContext = this;
                }

                SchoolclassesListGrid.Visibility = Visibility.Visible;
                var response = await _httpClient.GetAsync("https://localhost:7026/api/Schoolclasses/getAllSchoolclassesList");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var schoolclasses = JsonConvert.DeserializeObject<List<Schoolclasses>>(content);

                    List<Schoolclasses> schoolclassesList = new List<Schoolclasses>();

                    foreach (var schoolclass in schoolclasses)
                    {
                        Schoolclasses schoolclassInfo = new Schoolclasses()
                        {
                            Id = schoolclass.Id,
                            IdCycle = schoolclass.IdCycle,
                            IdSection = schoolclass.IdSection,
                            IdSubsection = schoolclass.IdSubsection,
                            IsDeleted = schoolclass.IsDeleted
                        };
                        schoolclassesList.Add(schoolclassInfo);
                    }
                    allSchoolclasses = new ObservableCollection<Schoolclasses>(schoolclassesList);
                    SchoolclassesListGrid.ItemsSource = allSchoolclasses;
                    AddSchoolclassButton.Visibility = Visibility.Visible;
                    SaveSchoolclassButton.Visibility = Visibility.Visible;
                    ArchiveSchoolclassButton.Visibility = Visibility.Visible;
                    DeleteSchoolclassButton.Visibility = Visibility.Visible;
                }
            }
        }

        private async void Save_Schoolclass(object sender, RoutedEventArgs e)
        {
            bool isAdded = false;
            int countAdded = 0;
            bool isUpdated = false;
            int countUpdated = 0;

            using (HttpClient _httpClient = new HttpClient())
            {
                foreach (Schoolclasses schoolclass in allSchoolclasses)
                {
                    if (schoolclass.Id == null)
                    {
                        var json = JsonConvert.SerializeObject(schoolclass);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync("https://localhost:7026/api/Schoolclasses/addSchoolclass", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var newSchoolclass = JsonConvert.DeserializeObject<Schoolclasses>(responseContent);
                            schoolclass.Id = newSchoolclass.Id;

                            Periods period = new Periods();
                            period.IdSchoolclass = (int)schoolclass.Id;
                            period.BeginningPeriod = new DateTime(DateTime.Now.Year, 9, 1);
                            period.EndingPeriod = new DateTime(DateTime.Now.Year + 1, 6, 1);
                            var periodJson = JsonConvert.SerializeObject(period);
                            var periodContent = new StringContent(periodJson, Encoding.UTF8, "application/json");
                            var periodResponse = await _httpClient.PostAsync("https://localhost:7026/api/Periods/addPeriod", periodContent);
                            isAdded = true;
                            countAdded++;
                        }
                    }
                    else
                    {
                        var response = await _httpClient.GetAsync($"https://localhost:7026/api/Schoolclasses/getAllSchoolclassNoIncludes/{schoolclass.Id}");
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var originalSchoolclass = JsonConvert.DeserializeObject<Schoolclasses>(content);

                            bool isChanged = false;
                            if (schoolclass.IdCycle != originalSchoolclass.IdCycle)
                            {
                                originalSchoolclass.IdCycle = schoolclass.IdCycle;
                                isChanged = true;
                            }
                            if (schoolclass.IdSection != originalSchoolclass.IdSection)
                            {
                                originalSchoolclass.IdSection = schoolclass.IdSection;
                                isChanged = true;
                            }
                            if (schoolclass.IdSubsection != originalSchoolclass.IdSubsection)
                            {
                                originalSchoolclass.IdSubsection = schoolclass.IdSubsection;
                                isChanged = true;
                            }

                            if (isChanged)
                            {
                                var json = JsonConvert.SerializeObject(originalSchoolclass);
                                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                                var putResponse = await _httpClient.PutAsync($"https://localhost:7026/api/Schoolclasses/updateSchoolclass/{originalSchoolclass.Id}", stringContent);
                                if (putResponse.IsSuccessStatusCode)
                                {
                                    isUpdated = true;
                                    countUpdated++;
                                }
                            }
                        }
                    }
                }
            }

            if (isAdded || isUpdated)
            {
                string buttonText = string.Empty;
                if (isAdded && !isUpdated)
                {
                    buttonText = "La classe a été ajoutée";
                    if (countAdded > 1)
                    {
                        buttonText = "Les classes ont été ajoutées";
                    }
                }
                else if (isUpdated && !isAdded)
                {
                    buttonText = "La classe a été modifiée";
                    if (countUpdated > 1)
                    {
                        buttonText = "Les classes ont été modifiées";
                    }
                }
                else if (isAdded && isUpdated)
                {
                    buttonText = "Les classes ont été ajoutées/modifiées";
                }
                buttonText += " avec succès !";

                MessageBox.Show(buttonText, "Enregistrement classe", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Aucun enregistrement effectué, veuillez réessayer", "Enregistrement classe", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_Schoolclass(object sender, RoutedEventArgs e)
        {
            var newSchoolclass = new Schoolclasses();
            allSchoolclasses.Add(newSchoolclass);

            SchoolclassesListGrid.Items.Refresh();
        }

        private async void Delete_Schoolclass(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (SchoolclassesListGrid.SelectedItems.Count > 0)
                {
                    if (SchoolclassesListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        SchoolclassesListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isDeleted = false;
                    int countDeleted = 0;

                    MessageBoxResult messageBoxResult = MessageBox.Show("Etes-vous sûr de vouloir confirmer la suppression ?", "Suppression définitive", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    switch (messageBoxResult)
                    {
                        case MessageBoxResult.Yes:
                            List<Schoolclasses> selectedSchoolclasses = new List<Schoolclasses>(SchoolclassesListGrid.SelectedItems.Cast<Schoolclasses>());
                            bool hasStudent = false;

                            foreach (Schoolclasses schoolclass in selectedSchoolclasses)
                            {
                                if (schoolclass.Id != null)
                                {
                                    var SchoolclassesResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Students/getSchoolclass/{schoolclass.Id}");
                                    hasStudent = await SchoolclassesResponse.Content.ReadAsAsync<bool>();
                                    if (!hasStudent)
                                    {
                                        var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Schoolclasses/deleteSchoolclass/{schoolclass.Id}");
                                        if (response.IsSuccessStatusCode)
                                        {
                                            allSchoolclasses.Remove(schoolclass);
                                            isDeleted = true;
                                            countDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Suppression de la classe impossible car déjà utilisé dans une classe", "Classe déjà utilisée", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    allSchoolclasses.Remove(schoolclass);
                                    isDeleted = true;
                                    countDeleted++;
                                }
                            }
                            if (isDeleted)
                            {
                                string messageText = "La classe a été supprimée";
                                if (countDeleted > 1)
                                {
                                    messageText = "Les classes ont été supprimées";
                                }
                                messageText += " avec succès !";
                                MessageBox.Show(messageText, "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Aucune classe supprimée, veuillez réessayer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            break;
                        case MessageBoxResult.No:
                            break;
                        case MessageBoxResult.Cancel:
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner des classes à supprimer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private async void Archive_Schoolclass(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (SchoolclassesListGrid.SelectedItems.Count > 0)
                {
                    if (SchoolclassesListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        SchoolclassesListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isSoftDeleted = false;
                    int countSoftDeleted = 0;
                    bool hasStudent = false;

                    List<Schoolclasses> selectedSchoolclasses = new List<Schoolclasses>(SchoolclassesListGrid.SelectedItems.Cast<Schoolclasses>());

                    foreach (Schoolclasses schoolclass in selectedSchoolclasses)
                    {
                        var SchoolclassesResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Students/getSchoolclass/{schoolclass.Id}");
                        hasStudent = await SchoolclassesResponse.Content.ReadAsAsync<bool>();
                        if (!hasStudent)
                        {
                            var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Schoolclasses/softDeleteSchoolclass/{schoolclass.Id}");
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();
                                bool isDeletedStatus = bool.Parse(responseContent);
                                schoolclass.IsDeleted = isDeletedStatus;
                                int index = allSchoolclasses.IndexOf(schoolclass);
                                allSchoolclasses.Remove(schoolclass);
                                allSchoolclasses.Insert(index, schoolclass);
                                isSoftDeleted = true;
                                countSoftDeleted++;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Archivage de la classe impossible car déjà utilisée dans une classe", "Classe déjà utilisée", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (isSoftDeleted)
                    {
                        string messageText = "La classe a été archivée/désarchivée";
                        if (countSoftDeleted > 1)
                        {
                            messageText = "Les classes ont été archivées/désarchivées";
                        }
                        messageText += " avec succès !";
                        MessageBox.Show(messageText, "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucune classe archivée/désarchivée, veuillez réessayer", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner des classes à archiver/désarchiver", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void Get_Periods(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                if (dataGrid.SelectedItem != null)
                {
                    SchoolclassesListGrid.Visibility = Visibility.Collapsed;
                    AddSchoolclassButton.Visibility = Visibility.Collapsed;
                    SaveSchoolclassButton.Visibility = Visibility.Collapsed;
                    ArchiveSchoolclassButton.Visibility = Visibility.Collapsed;
                    DeleteSchoolclassButton.Visibility = Visibility.Collapsed;
                    Schoolclasses schoolclass = (Schoolclasses)dataGrid.SelectedItem;
                    PeriodsPage periodsPage = new PeriodsPage(schoolclass);
                    Schoolclasses.Content = periodsPage;
                }
            }
        }
    }
}
