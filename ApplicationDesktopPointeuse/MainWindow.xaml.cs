using APIPointeuse.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ApplicationDesktopPointeuse
{
    public partial class MainWindow : Window
    {
        private async Task<string> AuthenticateAsync(string username, string password)
        {
            try
            {
                using (HttpClient _httpClient = new HttpClient())
                {
                    User user = new User
                    {
                        Username = username,
                        Password = password,
                        Role = "Admin"
                    };

                    var json = JsonConvert.SerializeObject(user);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("https://localhost:7026/api/Login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResponse = await response.Content.ReadAsStringAsync();
                        var token = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);
                        return token.Token; // Stockez le token dans un endroit sécurisé (par exemple, SecureString).
                    }
                    else
                    {
                        MessageBox.Show("L'authentification a échoué. Vérifiez vos informations d'identification.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors de l'authentification : {ex.Message}");
            }

            return null;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            string token = await AuthenticateAsync(username, password);

            if (!string.IsNullOrEmpty(token))
            {
                SecureString secureToken = new NetworkCredential("", token).SecurePassword;

                TokenManager.SetToken(secureToken);

                ChoicesWindow choicesWindow = new ChoicesWindow();
                choicesWindow.Show();

                // Fermez cette fenêtre (MainWindow) si nécessaire
                Close();
            }
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
    }
}
