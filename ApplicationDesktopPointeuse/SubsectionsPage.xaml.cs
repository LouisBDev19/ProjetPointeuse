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
    /// Logique d'interaction pour SubsectionsPage.xaml
    /// </summary>
    public partial class SubsectionsPage : UserControl
    {
        private ObservableCollection<Subsections> allSubsections;
        public SubsectionsPage()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            SaveSubsectionButton.Visibility = Visibility.Hidden;
            ArchiveSubsectionButton.Visibility = Visibility.Hidden;
            DeleteSubsectionButton.Visibility = Visibility.Hidden;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                SubsectionsListGrid.Visibility = Visibility.Visible;
                var response = await _httpClient.GetAsync("https://localhost:7026/api/Subsections/getAllSubsectionsList");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var subsections = JsonConvert.DeserializeObject<List<Subsections>>(content);

                    List<Subsections> subsectionsList = new List<Subsections>();

                    foreach (var subsection in subsections)
                    {
                        Subsections subsectionInfo = new Subsections()
                        {
                            Id = subsection.Id,
                            Name = subsection.Name,
                            IsDeleted = subsection.IsDeleted
                        };
                        subsectionsList.Add(subsectionInfo);
                    }
                    allSubsections = new ObservableCollection<Subsections>(subsectionsList);
                    SubsectionsListGrid.ItemsSource = allSubsections;
                    SaveSubsectionButton.Visibility = Visibility.Visible;
                    ArchiveSubsectionButton.Visibility = Visibility.Visible;
                    DeleteSubsectionButton.Visibility = Visibility.Visible;
                }
            }
        }

        private async void Save_Subsection(object sender, RoutedEventArgs e)
        {
            bool isAdded = false;
            int countAdded = 0;
            bool isUpdated = false;
            int countUpdated = 0;

            using (HttpClient _httpClient = new HttpClient())
            {
                foreach (Subsections subsection in allSubsections)
                {
                    if (subsection.Id == 0)
                    {
                        var json = JsonConvert.SerializeObject(subsection);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync("https://localhost:7026/api/Subsections/addSubsection", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var newSubsection = JsonConvert.DeserializeObject<Subsections>(responseContent);
                            subsection.Id = newSubsection.Id;
                            isAdded = true;
                            countAdded++;
                        }
                    }
                    else
                    {
                        var response = await _httpClient.GetAsync($"https://localhost:7026/api/Subsections/getAllSubsection/{subsection.Id}");
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var originalSubsection = JsonConvert.DeserializeObject<Subsections>(content);

                            bool isChanged = false;
                            if (subsection.Name != originalSubsection.Name)
                            {
                                originalSubsection.Name = subsection.Name;
                                isChanged = true;
                            }

                            if (isChanged)
                            {
                                var json = JsonConvert.SerializeObject(originalSubsection);
                                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                                var putResponse = await _httpClient.PutAsync($"https://localhost:7026/api/Subsections/updateSubsection/{originalSubsection.Id}", stringContent);
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
                    buttonText = "La sous-section a été ajoutée";
                    if (countAdded > 1)
                    {
                        buttonText = "Les sous-sections ont été ajoutées";
                    }
                }
                else if (isUpdated && !isAdded)
                {
                    buttonText = "La sous-section a été modifiée";
                    if (countUpdated > 1)
                    {
                        buttonText = "Les sous-sections ont été modifiées";
                    }
                }
                else if (isAdded && isUpdated)
                {
                    buttonText = "Les sous-sections ont été ajoutées/modifiées";
                }
                buttonText += " avec succès !";

                MessageBox.Show(buttonText, "Enregistrement sous-section", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Aucun enregistrement effectué, veuillez réessayer", "Enregistrement sous-section", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Delete_Subsection(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (SubsectionsListGrid.SelectedItems.Count > 0)
                {
                    if (SubsectionsListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        SubsectionsListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isDeleted = false;
                    int countDeleted = 0;

                    MessageBoxResult messageBoxResult = MessageBox.Show("Etes-vous sûr de vouloir confirmer la suppression ?", "Suppression définitive", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    switch (messageBoxResult)
                    {
                        case MessageBoxResult.Yes:
                            List<Subsections> selectedSubsections = new List<Subsections>(SubsectionsListGrid.SelectedItems.Cast<Subsections>());
                            bool hasSchoolclass = false;

                            foreach (Subsections subsection in selectedSubsections)
                            {
                                if (subsection.Id != 0)
                                {
                                    var SubsectionsResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Schoolclasses/Subsection/getSchoolclass/{subsection.Id}");
                                    hasSchoolclass = await SubsectionsResponse.Content.ReadAsAsync<bool>();
                                    if (!hasSchoolclass)
                                    {
                                        var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Subsections/deleteSubsection/{subsection.Id}");
                                        if (response.IsSuccessStatusCode)
                                        {
                                            allSubsections.Remove(subsection);
                                            isDeleted = true;
                                            countDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Suppression de la sous-section {subsection.Name} impossible car déjà utilisée dans une classe", "Sous-section déjà utilisée", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    allSubsections.Remove(subsection);
                                    isDeleted = true;
                                    countDeleted++;
                                }
                            }
                            if (isDeleted)
                            {
                                string messageText = "La sous-section a été supprimée";
                                if (countDeleted > 1)
                                {
                                    messageText = "Les sous-sections ont été supprimées";
                                }
                                messageText += " avec succès !";
                                MessageBox.Show(messageText, "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Aucune sous-section supprimée, veuillez réessayer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Vous devez sélectionner des sous-sections à supprimer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private async void Archive_Subsection(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (SubsectionsListGrid.SelectedItems.Count > 0)
                {
                    if (SubsectionsListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        SubsectionsListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isSoftDeleted = false;
                    int countSoftDeleted = 0;
                    bool hasSchoolclass = false;

                    List<Subsections> selectedSubsections = new List<Subsections>(SubsectionsListGrid.SelectedItems.Cast<Subsections>());

                    foreach (Subsections subsection in selectedSubsections)
                    {
                        var SubsectionsResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Schoolclasses/Subsection/getSchoolclass/{subsection.Id}");
                        hasSchoolclass = await SubsectionsResponse.Content.ReadAsAsync<bool>();
                        if (!hasSchoolclass)
                        {
                            var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Subsections/softDeleteSubsection/{subsection.Id}");
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();
                                bool isDeletedStatus = bool.Parse(responseContent);
                                subsection.IsDeleted = isDeletedStatus;
                                int index = allSubsections.IndexOf(subsection);
                                allSubsections.Remove(subsection);
                                allSubsections.Insert(index, subsection);
                                isSoftDeleted = true;
                                countSoftDeleted++;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Archivage de la sous-section {subsection.Name} impossible car déjà utilisée dans une classe", "Sous-section déjà utilisée", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (isSoftDeleted)
                    {
                        string messageText = "La sous-section a été archivée/désarchivée";
                        if (countSoftDeleted > 1)
                        {
                            messageText = "Les sous-sections ont été archivées/désarchivées";
                        }
                        messageText += " avec succès !";
                        MessageBox.Show(messageText, "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucune sous-section archivée/désarchivée, veuillez réessayer", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner des sous-sections à archiver/désarchiver", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
    }
}
