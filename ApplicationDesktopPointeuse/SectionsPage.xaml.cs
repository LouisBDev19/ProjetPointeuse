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
    /// Logique d'interaction pour SectionsPage.xaml
    /// </summary>
    public partial class SectionsPage : UserControl
    {
        private ObservableCollection<Sections> allSections;
        public SectionsPage()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            SaveSectionButton.Visibility = Visibility.Hidden;
            ArchiveSectionButton.Visibility = Visibility.Hidden;
            DeleteSectionButton.Visibility = Visibility.Hidden;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                SectionsListGrid.Visibility = Visibility.Visible;
                var response = await _httpClient.GetAsync("https://localhost:7026/api/Sections/getAllSectionsList");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var sections = JsonConvert.DeserializeObject<List<Sections>>(content);

                    List<Sections> sectionsList = new List<Sections>();

                    foreach (var section in sections)
                    {
                        Sections sectionInfo = new Sections()
                        {
                            Id = section.Id,
                            Name = section.Name,
                            IsDeleted = section.IsDeleted
                        };
                        sectionsList.Add(sectionInfo);
                    }
                    allSections = new ObservableCollection<Sections>(sectionsList);
                    SectionsListGrid.ItemsSource = allSections;
                    SaveSectionButton.Visibility = Visibility.Visible;
                    ArchiveSectionButton.Visibility = Visibility.Visible;
                    DeleteSectionButton.Visibility = Visibility.Visible;
                }
            }
        }

        private async void Save_Section(object sender, RoutedEventArgs e)
        {
            bool isAdded = false;
            int countAdded = 0;
            bool isUpdated = false;
            int countUpdated = 0;

            using (HttpClient _httpClient = new HttpClient())
            {
                foreach (Sections section in allSections)
                {
                    if (section.Id == 0)
                    {
                        var json = JsonConvert.SerializeObject(section);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync("https://localhost:7026/api/Sections/addSection", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var newSection = JsonConvert.DeserializeObject<Sections>(responseContent);
                            section.Id = newSection.Id;
                            isAdded = true;
                            countAdded++;
                        }
                    }
                    else
                    {
                        var response = await _httpClient.GetAsync($"https://localhost:7026/api/Sections/getAllSection/{section.Id}");
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var originalSection = JsonConvert.DeserializeObject<Sections>(content);

                            bool isChanged = false;
                            if (section.Name != originalSection.Name)
                            {
                                originalSection.Name = section.Name;
                                isChanged = true;
                            }

                            if (isChanged)
                            {
                                var json = JsonConvert.SerializeObject(originalSection);
                                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                                var putResponse = await _httpClient.PutAsync($"https://localhost:7026/api/Sections/updateSection/{originalSection.Id}", stringContent);
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
                    buttonText = "La section a été ajoutée";
                    if (countAdded > 1)
                    {
                        buttonText = "Les sections ont été ajoutées";
                    }
                }
                else if (isUpdated && !isAdded)
                {
                    buttonText = "La section a été modifiée";
                    if (countUpdated > 1)
                    {
                        buttonText = "Les sections ont été modifiées";
                    }
                }
                else if (isAdded && isUpdated)
                {
                    buttonText = "Les sections ont été ajoutées/modifiées";
                }
                buttonText += " avec succès !";

                MessageBox.Show(buttonText, "Enregistrement section", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Aucun enregistrement effectué, veuillez réessayer", "Enregistrement section", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Delete_Section(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (SectionsListGrid.SelectedItems.Count > 0)
                {
                    if (SectionsListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        SectionsListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isDeleted = false;
                    int countDeleted = 0;

                    MessageBoxResult messageBoxResult = MessageBox.Show("Etes-vous sûr de vouloir confirmer la suppression ?", "Suppression définitive", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    switch (messageBoxResult)
                    {
                        case MessageBoxResult.Yes:
                            List<Sections> selectedSections = new List<Sections>(SectionsListGrid.SelectedItems.Cast<Sections>());
                            bool hasSchoolclass = false;

                            foreach (Sections section in selectedSections)
                            {
                                if (section.Id != 0)
                                {
                                    var SectionsResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Schoolclasses/Section/getSchoolclass/{section.Id}");
                                    hasSchoolclass = await SectionsResponse.Content.ReadAsAsync<bool>();
                                    if (!hasSchoolclass)
                                    {
                                        var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Sections/deleteSection/{section.Id}");
                                        if (response.IsSuccessStatusCode)
                                        {
                                            allSections.Remove(section);
                                            isDeleted = true;
                                            countDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Suppression de la section {section.Name} impossible car déjà utilisé dans une classe", "Section déjà utilisée", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    allSections.Remove(section);
                                    isDeleted = true;
                                    countDeleted++;
                                }
                            }
                            if (isDeleted)
                            {
                                string messageText = "La section a été supprimée";
                                if (countDeleted > 1)
                                {
                                    messageText = "Les sections ont été supprimées";
                                }
                                messageText += " avec succès !";
                                MessageBox.Show(messageText, "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Aucune section supprimée, veuillez réessayer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Vous devez sélectionner des sections à supprimer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private async void Archive_Section(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (SectionsListGrid.SelectedItems.Count > 0)
                {
                    if (SectionsListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        SectionsListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isSoftDeleted = false;
                    int countSoftDeleted = 0;
                    bool hasSchoolclass = false;

                    List<Sections> selectedSections = new List<Sections>(SectionsListGrid.SelectedItems.Cast<Sections>());

                    foreach (Sections section in selectedSections)
                    {
                        var SectionsResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Schoolclasses/Section/getSchoolclass/{section.Id}");
                        hasSchoolclass = await SectionsResponse.Content.ReadAsAsync<bool>();
                        if (!hasSchoolclass)
                        {
                            var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Sections/softDeleteSection/{section.Id}");
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();
                                bool isDeletedStatus = bool.Parse(responseContent);
                                section.IsDeleted = isDeletedStatus;
                                int index = allSections.IndexOf(section);
                                allSections.Remove(section);
                                allSections.Insert(index, section);
                                isSoftDeleted = true;
                                countSoftDeleted++;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Archivage de la section {section.Name} impossible car déjà utilisée dans une classe", "Section déjà utilisée", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (isSoftDeleted)
                    {
                        string messageText = "La section a été archivée/désarchivée";
                        if (countSoftDeleted > 1)
                        {
                            messageText = "Les sections ont été archivées/désarchivées";
                        }
                        messageText += " avec succès !";
                        MessageBox.Show(messageText, "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucune section archivée/désarchivée, veuillez réessayer", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner des sections à archiver/désarchiver", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
    }
}
