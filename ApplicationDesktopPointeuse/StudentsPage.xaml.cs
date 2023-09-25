using APIPointeuse.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
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
    /// Logique d'interaction pour StudentsPage.xaml
    /// </summary>
    public partial class StudentsPage : UserControl
    {
        private ObservableCollection<Students> allStudents;
        public ObservableCollection<Schoolclasses> schoolclassesList { get; set; }
        private SecureString token;
        public StudentsPage()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            CSVImportButton.Visibility = Visibility.Hidden;
            SaveStudentButton.Visibility = Visibility.Hidden;
            ArchiveStudentButton.Visibility = Visibility.Hidden;
            DeleteStudentButton.Visibility = Visibility.Hidden;
            schoolclassesList = new ObservableCollection<Schoolclasses>();
            token = TokenManager.Token;
        }

        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                var SchoolclassesResponse = await _httpClient.GetAsync("https://localhost:7026/api/Schoolclasses/getSchoolclassesList");
                if (SchoolclassesResponse.IsSuccessStatusCode)
                {
                    var SchoolclassesContent = await SchoolclassesResponse.Content.ReadAsStringAsync();
                    var schoolclasses = JsonConvert.DeserializeObject<List<Schoolclasses>>(SchoolclassesContent);

                    foreach (var schoolclass in schoolclasses)
                    {
                        schoolclassesList.Add(schoolclass);
                    }
                    Schoolclasses noneSchoolclass = new Schoolclasses()
                    {
                        Id = null
                    };
                    schoolclassesList.Add(noneSchoolclass);

                    DataContext = this;
                }

                var response = await _httpClient.GetAsync("https://localhost:7026/api/Students/getAllStudentsList");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var students = JsonConvert.DeserializeObject<List<Students>>(content);

                    List<Students> studentsList = new List<Students>();

                    foreach (var student in students)
                    {

                        Students studentInfo = new Students()
                        {
                            Id = student.Id,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            BirthDate = student.BirthDate,
                            Email = student.Email,
                            PhoneNumber = student.PhoneNumber,
                            IdSchoolclass = student.IdSchoolclass,
                            IsDeleted = student.IsDeleted
                        };
                        studentsList.Add(studentInfo);
                    }
                    allStudents = new ObservableCollection<Students>(studentsList);
                    studentsListGrid.ItemsSource = allStudents;
                }
            }
            CSVImportButton.Visibility = Visibility.Visible;
            SaveStudentButton.Visibility = Visibility.Visible;
            ArchiveStudentButton.Visibility = Visibility.Visible;
            DeleteStudentButton.Visibility = Visibility.Visible;
        }

        private async void Save_Student(object sender, RoutedEventArgs e)
        {
            bool isAdded = false;
            int countAdded = 0;
            bool isUpdated = false;
            int countUpdated = 0;
            bool isDuplicate = false;

            using (HttpClient _httpClient = new HttpClient())
            {
                foreach (Students student in allStudents)
                {
                    if (student.Id == 0)
                    {
                        var studentDuplicateResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Students/getDuplicateEmailStudent/{student.Email}");

                        if (studentDuplicateResponse.IsSuccessStatusCode)
                        {
                            isDuplicate = await studentDuplicateResponse.Content.ReadAsAsync<bool>();

                            if (isDuplicate)
                            {
                                MessageBox.Show($"Attention le mail {student.Email} existe déjà en base !", "Mail dupliqué", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                var json = JsonConvert.SerializeObject(student);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await _httpClient.PostAsync("https://localhost:7026/api/Students/addStudent", content);

                                if (response.IsSuccessStatusCode)
                                {
                                    var responseContent = await response.Content.ReadAsStringAsync();
                                    var newStudent = JsonConvert.DeserializeObject<Students>(responseContent);
                                    student.Id = newStudent.Id;
                                    isAdded = true;
                                    countAdded++;
                                }
                            }
                        }
                    }
                    else
                    {
                        var response = await _httpClient.GetAsync($"https://localhost:7026/api/Students/getAllStudent/{student.Id}");
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeAnonymousType(content, new { student = new Students() });
                            var originalStudent = result.student;

                            bool isChanged = false;
                            if (student.FirstName != originalStudent.FirstName)
                            {
                                originalStudent.FirstName = student.FirstName;
                                isChanged = true;
                            }
                            if (student.LastName != originalStudent.LastName)
                            {
                                originalStudent.LastName = student.LastName;
                                isChanged = true;
                            }
                            if (student.BirthDate != originalStudent.BirthDate)
                            {
                                originalStudent.BirthDate = student.BirthDate;
                                isChanged = true;
                            }
                            if (student.IdSchoolclass != originalStudent.IdSchoolclass)
                            {
                                originalStudent.IdSchoolclass = student.IdSchoolclass;
                                isChanged = true;
                            }
                            if (student.Email != originalStudent.Email)
                            {
                                originalStudent.Email = student.Email;

                                var studentDuplicateResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Students/getDuplicateEmailStudent/{originalStudent.Email}");

                                if (studentDuplicateResponse.IsSuccessStatusCode)
                                {
                                    isDuplicate = await studentDuplicateResponse.Content.ReadAsAsync<bool>();

                                    if (isDuplicate)
                                    {
                                        MessageBox.Show($"Attention le mail {originalStudent.Email} existe déjà en base !", "Mail dupliqué", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                isChanged = true;
                            }
                            if (student.PhoneNumber != originalStudent.PhoneNumber)
                            {
                                originalStudent.PhoneNumber = student.PhoneNumber;
                                isChanged = true;
                            }

                            if (isChanged && !isDuplicate)
                            {
                                var json = JsonConvert.SerializeObject(originalStudent);
                                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                                var putResponse = await _httpClient.PutAsync($"https://localhost:7026/api/Students/updateStudent/{originalStudent.Id}", stringContent);
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
                    buttonText = "L'étudiant a été ajouté";
                    if (countAdded > 1)
                    {
                        buttonText = "Les étudiants ont été ajoutés";
                    }
                }
                else if (isUpdated && !isAdded)
                {
                    buttonText = "L'étudiant a été modifié";
                    if (countUpdated > 1)
                    {
                        buttonText = "Les étudiants ont été modifiés";
                    }
                }
                else if (isAdded && isUpdated)
                {
                    buttonText = "Les étudiants ont été ajoutés/modifiés";
                }
                buttonText += " avec succès !";

                MessageBox.Show(buttonText, "Enregistrement étudiants", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Aucun enregistrement effectué, veuillez réessayer", "Enregistrement étudiant", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void Delete_Student(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (studentsListGrid.SelectedItems.Count > 0)
                {
                    if (studentsListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        studentsListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isDeleted = false;
                    int countDeleted = 0;

                    MessageBoxResult messageBoxResult = MessageBox.Show("Etes-vous sûr de vouloir confirmer la suppression ?", "Suppression définitive", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    switch (messageBoxResult)
                    {
                        case MessageBoxResult.Yes:
                            List<Students> selectedStudents = new List<Students>(studentsListGrid.SelectedItems.Cast<Students>());

                            foreach (Students student in selectedStudents)
                            {
                                if(student.Id != 0)
                                {
                                    var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Students/deleteStudent/{student.Id}");
                                    if (response.IsSuccessStatusCode)
                                    {
                                        allStudents.Remove(student);
                                        isDeleted = true;
                                        countDeleted++;
                                    }
                                }
                                else
                                {
                                    allStudents.Remove(student);
                                    isDeleted = true;
                                    countDeleted++;
                                }
                            }
                            if (isDeleted)
                            {
                                string messageText = "L'étudiant a été supprimé";
                                if (countDeleted > 1)
                                {
                                    messageText = "Les étudiants ont été supprimés";
                                }
                                messageText += " avec succès !";
                                MessageBox.Show(messageText, "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Aucun étudiant supprimé, veuillez réessayer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Vous devez sélectionner des étudiants à supprimer", "Suppression définitive", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private async void Archive_Student(object sender, RoutedEventArgs e)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                if (studentsListGrid.SelectedItems.Count > 0)
                {
                    if (studentsListGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
                    {
                        studentsListGrid.SelectedItems.Remove(CollectionView.NewItemPlaceholder);
                    }

                    bool isSoftDeleted = false;
                    int countSoftDeleted = 0;

                    List<Students> selectedStudents = new List<Students>(studentsListGrid.SelectedItems.Cast<Students>());

                    foreach (Students student in selectedStudents)
                    {
                        var response = await _httpClient.DeleteAsync($"https://localhost:7026/api/Students/softDeleteStudent/{student.Id}");
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            bool isDeletedStatus = bool.Parse(responseContent);
                            student.IsDeleted = isDeletedStatus;
                            int index = allStudents.IndexOf(student);
                            allStudents.Remove(student);
                            allStudents.Insert(index, student);
                            isSoftDeleted = true;
                            countSoftDeleted++;
                        }
                    }
                    if (isSoftDeleted)
                    {
                        string messageText = "L'étudiant a été archivé/désarchivé";
                        if (countSoftDeleted > 1)
                        {
                            messageText = "Les étudiants ont été archivés/désarchivés";
                        }
                        messageText += " avec succès !";
                        MessageBox.Show(messageText, "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucun étudiant archivé/désarchivé, veuillez réessayer", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez sélectionner des étudiants à archiver/désarchiver", "Archivage/Désarchivage", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private async void CSVImport(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers CSV (*.csv)|*.csv|Tous les fichiers (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                List<string[]> CSVdata = CSVFileImport(filePath);

                if(CSVdata.Count > 0)
                {
                    for (int i = 0; i < CSVdata.Count; i++)
                    {
                        DateTime BirthDate;
                        DateTime.TryParse(CSVdata[i][2], out BirthDate);

                        int? IdSchoolclass = null;
                        int.TryParse(CSVdata[i][3], out int Id);
                        IdSchoolclass = Id;
                        bool isSchoolclassExist = false;

                        if (IdSchoolclass != null)
                        {
                            using (HttpClient _httpClient = new HttpClient())
                            {
                                var schoolclassResponse = await _httpClient.GetAsync($"https://localhost:7026/api/Schoolclasses/getSchoolclassId/{IdSchoolclass}");

                                if (schoolclassResponse.IsSuccessStatusCode)
                                {
                                    isSchoolclassExist = await schoolclassResponse.Content.ReadAsAsync<bool>();
                                }
                            }
                        }

                        if (!isSchoolclassExist)
                        {
                            IdSchoolclass = null;
                        }

                            Students student = new Students()
                        {
                            FirstName = CSVdata[i][0],
                            LastName = CSVdata[i][1],
                            BirthDate = BirthDate,
                            IdSchoolclass = IdSchoolclass,
                            Email = CSVdata[i][4],
                            PhoneNumber = CSVdata[i][5],
                        };
                        allStudents.Add(student);
                    }
                    studentsListGrid.ItemsSource = allStudents;
                }
            }
        }

        private List<string[]> CSVFileImport(string filePath)
        {
            List<string[]> CSVdata = new List<string[]>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        CSVdata.Add(values);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors de l'importation du fichier CSV : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return CSVdata;
        }
    }
}
