using APIPointeuse.Models;
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
    /// Logique d'interaction pour CyclesPage.xaml
    /// </summary>
    public partial class CyclesPage : UserControl
    {
        private ObservableCollection<Cycles> allCycles;
        public CyclesPage()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            SaveCycleButton.Visibility = Visibility.Hidden;
            ArchiveCycleButton.Visibility = Visibility.Hidden;
            DeleteCycleButton.Visibility = Visibility.Hidden;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                CyclesListGrid.Visibility = Visibility.Visible;
                var response = await _httpClient.GetAsync("https://localhost:7026/api/Cycles/getAllCyclesList");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var cycles = JsonConvert.DeserializeObject<List<Cycles>>(content);

                    List<Cycles> cyclesList = new List<Cycles>();

                    foreach (var cycle in cycles)
                    {
                        Cycles cycleInfo = new Cycles()
                        {
                            Id = cycle.Id,
                            Name = cycle.Name,
                            IsDeleted = cycle.IsDeleted
                        };
                        cyclesList.Add(cycleInfo);
                    }
                    allCycles = new ObservableCollection<Cycles>(cyclesList);
                    CyclesListGrid.ItemsSource = allCycles;
                    SaveCycleButton.Visibility = Visibility.Visible;
                    ArchiveCycleButton.Visibility = Visibility.Visible;
                    DeleteCycleButton.Visibility = Visibility.Visible;
                }
            }
        }

        private async void Save_Cycle(object sender, RoutedEventArgs e)
        {
            bool isAdded = false;
            int countAdded = 0;
            bool isUpdated = false;
            int countUpdated = 0;

            using (HttpClient _httpClient = new HttpClient())
            {
                foreach (Cycles cycle in allCycles)
                {
                    if (cycle.Id == 0)
                    {
                        var json = JsonConvert.SerializeObject(cycle);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync("https://localhost:7026/api/Cycles/addCycle", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var newCycle = JsonConvert.DeserializeObject<Cycles>(responseContent);
                            cycle.Id = newCycle.Id;
                            isAdded = true;
                            countAdded++;
                        }
                    }
                    else
                    {
                        var response = await _httpClient.GetAsync($"https://localhost:7026/api/Cycles/getAllCycle/{cycle.Id}");
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var originalCycle = JsonConvert.DeserializeObject<Cycles>(content);

                            bool isChanged = false;
                            if (cycle.Name != originalCycle.Name)
                            {
                                originalCycle.Name = cycle.Name;
                                isChanged = true;
                            }

                            if (isChanged)
                            {
                                var json = JsonConvert.SerializeObject(originalCycle);
                                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                                var putResponse = await _httpClient.PutAsync($"https://localhost:7026/api/Cycles/updateCycle/{originalCycle.Id}", stringContent);
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
                    buttonText = "Le cycle a été ajouté";
                    if (countAdded > 1)
                    {
                        buttonText = "Les cycles ont été ajoutés";
                    }
                }
                else if (isUpdated && !isAdded)
                {
                    buttonText = "Le cycle a été modifié";
                    if (countUpdated > 1)
                    {
                        buttonText = "Les cycles ont été modifiés";
                    }
                }
                else if (isAdded && isUpdated)
                {
                    buttonText = "Les cycles ont été ajoutés/modifiés";
                }
                buttonText += " avec succès !";

                MessageBox.Show(buttonText, "Enregistrement cycle", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Aucun enregistrement effectué, veuillez réessayer", "Enregistrement cycle", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Delete_Cycle(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (CyclesListGrid.SelectedItems.Count > 0)
                {
                    if (CyclesListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        CyclesListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isDeleted = false;
                    int countDeleted = 0;

                    MessageBoxResult messageBoxResult = MessageBox.Show("Etes-vous sûr de vouloir confirmer la suppression ?", "Suppression définitive", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    switch (messageBoxResult)
                    {
                        case MessageBoxResult.Yes:
                            List<Cycles> selectedCycles = new List<Cycles>(CyclesListGrid.SelectedItems.Cast<Cycles>());
                            bool hasSchoolclass = false;

                            foreach (Cycles cycle in selectedCycles)
                            {
                                if (cycle.Id != 0)
                                {
                                    var CyclesResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Schoolclasses/Cycle/getSchoolclass/{cycle.Id}");
                                    hasSchoolclass = await CyclesResponse.Content.ReadAsAsync<bool>();
                                    if (!hasSchoolclass)
                                    {
                                        var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Cycles/deleteCycle/{cycle.Id}");
                                        if (response.IsSuccessStatusCode)
                                        {
                                            allCycles.Remove(cycle);
                                            isDeleted = true;
                                            countDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Suppression du cycle {cycle.Name} impossible car déjà utilisé dans une classe", "Cycle déjà utilisé", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    allCycles.Remove(cycle);
                                    isDeleted = true;
                                    countDeleted++;
                                }
                            }
                            if (isDeleted)
                            {
                                string messageText = "Le cycle a été supprimé";
                                if (countDeleted > 1)
                                {
                                    messageText = "Les cycles ont été supprimés";
                                }
                                messageText += " avec succès !";
                                MessageBox.Show(messageText, "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Aucun cycle supprimé, veuillez réessayer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Vous devez sélectionner des cycles à supprimer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private async void Archive_Cycle(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (CyclesListGrid.SelectedItems.Count > 0)
                {
                    if (CyclesListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        CyclesListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isSoftDeleted = false;
                    int countSoftDeleted = 0;
                    bool hasSchoolclass = false;

                    List<Cycles> selectedCycles = new List<Cycles>(CyclesListGrid.SelectedItems.Cast<Cycles>());

                    foreach (Cycles cycle in selectedCycles)
                    {
                        var CyclesResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Schoolclasses/Cycle/getSchoolclass/{cycle.Id}");
                        hasSchoolclass = await CyclesResponse.Content.ReadAsAsync<bool>();
                        if (!hasSchoolclass)
                        {
                            var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Cycles/softDeleteCycle/{cycle.Id}");
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();
                                bool isDeletedStatus = bool.Parse(responseContent);
                                cycle.IsDeleted = isDeletedStatus;
                                int index = allCycles.IndexOf(cycle);
                                allCycles.Remove(cycle);
                                allCycles.Insert(index, cycle);
                                isSoftDeleted = true;
                                countSoftDeleted++;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Archivage du cycle {cycle.Name} impossible car déjà utilisé dans une classe", "Cycle déjà utilisé", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (isSoftDeleted)
                    {
                        string messageText = "Le cycle a été archivé/désarchivé";
                        if (countSoftDeleted > 1)
                        {
                            messageText = "Les cycles ont été archivés/désarchivés";
                        }
                        messageText += " avec succès !";
                        MessageBox.Show(messageText, "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucun cycle archivé/désarchivé, veuillez réessayer", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner des cycles à archiver/désarchiver", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
    }
}
