using APIPointeuse.Models;
using Azure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour PeriodsPage.xaml
    /// </summary>
    public partial class PeriodsPage : UserControl
    {
        private Schoolclasses selectedSchoolclass;
        private ObservableCollection<Periods> allPeriods;
        public PeriodsPage(Schoolclasses schoolclass)
        {
            InitializeComponent();
            selectedSchoolclass = schoolclass;
            Loaded += Window_Loaded;
            AddPeriodButton.Visibility = Visibility.Hidden;
            SavePeriodButton.Visibility = Visibility.Hidden;
            ArchivePeriodButton.Visibility = Visibility.Hidden;
            DeletePeriodButton.Visibility = Visibility.Hidden;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                PeriodsListGrid.Visibility = Visibility.Visible;
                var response = await _httpClient.GetAsync($"https://localhost:7026/api/Periods/getPeriods/{selectedSchoolclass.Id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var periods = JsonConvert.DeserializeObject<List<Periods>>(content);

                    List<Periods> periodsList = new List<Periods>();

                    foreach (var period in periods)
                    {
                        Periods periodInfo = new Periods()
                        {
                            Id = period.Id,
                            BeginningPeriod = period.BeginningPeriod,
                            EndingPeriod = period.EndingPeriod,
                            IsDeleted = period.IsDeleted
                        };
                        periodsList.Add(periodInfo);
                    }
                    allPeriods = new ObservableCollection<Periods>(periodsList);
                    PeriodsListGrid.ItemsSource = allPeriods;
                    AddPeriodButton.Visibility = Visibility.Visible;
                    SavePeriodButton.Visibility = Visibility.Visible;
                    ArchivePeriodButton.Visibility = Visibility.Visible;
                    DeletePeriodButton.Visibility = Visibility.Visible;
                }
            }
        }

        private async void Save_Period(object sender, RoutedEventArgs e)
        {
            bool isAdded = false;
            int countAdded = 0;
            bool isUpdated = false;
            int countUpdated = 0;
            bool hasConflict = false;

            using (HttpClient _httpClient = new HttpClient())
            {
                foreach (Periods period in allPeriods)
                {
                    if (period.BeginningPeriod < period.EndingPeriod)
                    {
                        hasConflict = false;
                        foreach (Periods dates in allPeriods)
                        {
                            if (dates.Id != period.Id)
                            {
                                if (period.BeginningPeriod >= dates.BeginningPeriod && period.BeginningPeriod <= dates.EndingPeriod)
                                {
                                    hasConflict = true;
                                }

                                if (period.EndingPeriod >= dates.BeginningPeriod && period.EndingPeriod <= dates.EndingPeriod)
                                {
                                    hasConflict = true;
                                }
                            }
                        }
                        if (period.Id == 0)
                        {

                            if (!hasConflict)
                            {
                                period.IdSchoolclass = (int)selectedSchoolclass.Id;
                                var json = JsonConvert.SerializeObject(period);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await _httpClient.PostAsync("https://localhost:7026/api/Periods/addPeriod", content);

                                if (response.IsSuccessStatusCode)
                                {
                                    var responseContent = await response.Content.ReadAsStringAsync();
                                    var newPeriod = JsonConvert.DeserializeObject<Periods>(responseContent);
                                    period.Id = newPeriod.Id;
                                    isAdded = true;
                                    countAdded++;
                                }
                            }
                        }
                        else
                        {
                            if (!hasConflict)
                            {
                                var response = await _httpClient.GetAsync($"https://localhost:7026/api/Periods/getAllPeriod/{period.Id}");
                                if (response.IsSuccessStatusCode)
                                {
                                    var content = await response.Content.ReadAsStringAsync();
                                    var originalPeriod = JsonConvert.DeserializeObject<Periods>(content);

                                    bool isChanged = false;
                                    if (period.BeginningPeriod != originalPeriod.BeginningPeriod)
                                    {
                                        originalPeriod.BeginningPeriod = period.BeginningPeriod;
                                        isChanged = true;
                                    }
                                    if (period.EndingPeriod != originalPeriod.EndingPeriod)
                                    {
                                        originalPeriod.EndingPeriod = period.EndingPeriod;
                                        isChanged = true;
                                    }

                                    if (isChanged)
                                    {
                                        var json = JsonConvert.SerializeObject(originalPeriod);
                                        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                                        var putResponse = await _httpClient.PutAsync($"https://localhost:7026/api/Periods/updatePeriod/{originalPeriod.Id}", stringContent);
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
                    else
                    {
                        hasConflict = true;
                    }
                }
            }

            if(hasConflict)
            {
                MessageBox.Show("La logique des dates n'est pas respectée", "Enregistrement impossible", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (isAdded || isUpdated)
            {
                string buttonText = string.Empty;
                if (isAdded && !isUpdated)
                {
                    buttonText = "La période a été ajoutée";
                    if (countAdded > 1)
                    {
                        buttonText = "Les périodes ont été ajoutées";
                    }
                }
                else if (isUpdated && !isAdded)
                {
                    buttonText = "La période a été modifiée";
                    if (countUpdated > 1)
                    {
                        buttonText = "Les périodes ont été modifiées";
                    }
                }
                else if (isAdded && isUpdated)
                {
                    buttonText = "Les périodes ont été ajoutées/modifiées";
                }
                buttonText += " avec succès !";

                MessageBox.Show(buttonText, "Enregistrement période", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Aucun enregistrement effectué, veuillez réessayer", "Enregistrement période", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_Period(object sender, RoutedEventArgs e)
        {
            var newPeriod = new Periods();
            newPeriod.BeginningPeriod = DateTime.Now;
            newPeriod.EndingPeriod = DateTime.Now.AddDays(1);
            allPeriods.Add(newPeriod);

            PeriodsListGrid.Items.Refresh();
        }

        private async void Archive_Period(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (PeriodsListGrid.SelectedItems.Count > 0)
                {
                    if (PeriodsListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        PeriodsListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isSoftDeleted = false;
                    int countSoftDeleted = 0;

                    List<Periods> selectedPeriods = new List<Periods>(PeriodsListGrid.SelectedItems.Cast<Periods>());

                    foreach (Periods period in selectedPeriods)
                    {
                        var PeriodsResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Periods/getValidatedPeriods/{selectedSchoolclass.Id}/{period.Id}");
                        if (PeriodsResponse.IsSuccessStatusCode)
                        {
                            var content = await PeriodsResponse.Content.ReadAsStringAsync();

                            var periodsList = JsonConvert.DeserializeObject<List<Periods>>(content);
                            if (periodsList.Count >= 1)
                            {
                                var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Periods/softDeletePeriod/{period.Id}");
                                if (response.IsSuccessStatusCode)
                                {
                                    string responseContent = await response.Content.ReadAsStringAsync();
                                    bool isDeletedStatus = bool.Parse(responseContent);
                                    period.IsDeleted = isDeletedStatus;
                                    int index = allPeriods.IndexOf(period);
                                    allPeriods.Remove(period);
                                    allPeriods.Insert(index, period);
                                    isSoftDeleted = true;
                                    countSoftDeleted++;
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Archivage de la période impossible car il faut au moins une période active", "Archivage impossible", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    if (isSoftDeleted)
                    {
                        string messageText = "La période a été archivée/désarchivée";
                        if (countSoftDeleted > 1)
                        {
                            messageText = "Les périodes ont été archivées/désarchivées";
                        }
                        messageText += " avec succès !";
                        MessageBox.Show(messageText, "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucune période archivée/désarchivée, veuillez réessayer", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner des périodes à archiver/désarchiver", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private async void Delete_Period(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (PeriodsListGrid.SelectedItems.Count > 0)
                {
                    if (PeriodsListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        PeriodsListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isDeleted = false;
                    int countDeleted = 0;

                    MessageBoxResult messageBoxResult = MessageBox.Show("Etes-vous sûr de vouloir confirmer la suppression ?", "Suppression définitive", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    switch (messageBoxResult)
                    {
                        case MessageBoxResult.Yes:
                            List<Periods> selectedPeriods = new List<Periods>(PeriodsListGrid.SelectedItems.Cast<Periods>());

                            foreach (Periods period in selectedPeriods)
                            {
                                if (period.Id != 0)
                                {
                                    var PeriodsResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Periods/getValidatedPeriods/{selectedSchoolclass.Id}/{period.Id}");

                                    if(PeriodsResponse.IsSuccessStatusCode)
                                    {
                                        var content = await PeriodsResponse.Content.ReadAsStringAsync();

                                        var periodsList = JsonConvert.DeserializeObject<List<Periods>>(content);
                                        if (periodsList.Count >= 1)
                                        {
                                            var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Periods/deletePeriod/{period.Id}");
                                            if (response.IsSuccessStatusCode)
                                            {
                                                allPeriods.Remove(period);
                                                isDeleted = true;
                                                countDeleted++;
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show($"Suppression de la période impossible car il en faut au moins une", "Suppression impossible", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    allPeriods.Remove(period);
                                    isDeleted = true;
                                    countDeleted++;
                                }
                            }
                            if (isDeleted)
                            {
                                string messageText = "La période a été supprimée";
                                if (countDeleted > 1)
                                {
                                    messageText = "Les périodes ont été supprimées";
                                }
                                messageText += " avec succès !";
                                MessageBox.Show(messageText, "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Aucune période supprimée, veuillez réessayer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Vous devez sélectionner des périodes à supprimer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void Go_Back(object sender, RoutedEventArgs e)
        {
            PeriodsListGrid.Visibility = Visibility.Collapsed;
            AddPeriodButton.Visibility = Visibility.Collapsed;
            SavePeriodButton.Visibility = Visibility.Collapsed;
            ArchivePeriodButton.Visibility = Visibility.Collapsed;
            DeletePeriodButton.Visibility = Visibility.Collapsed;
            GoBackButton.Visibility = Visibility.Collapsed;
            Periods.Content = new SchoolclassesPage();
        }
    }
}
