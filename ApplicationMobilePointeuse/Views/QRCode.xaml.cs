using ApplicationMobilePointeuse.Common;
using ApplicationMobilePointeuse.Models;
using Newtonsoft.Json;
using QRCoder;

namespace ApplicationMobilePointeuse.Views
{
    public partial class QRCode : ContentPage
    {
        private HttpClient httpClient;
        private readonly DevHttpsConnectionHelper devSslHelper;
        private const double TargetLatitude = 45.8189;
        private const double TargetLongitude = 1.2720;
        private const double RadiusMeters = 60;

        public QRCode()
        {
            InitializeComponent();
            devSslHelper = new DevHttpsConnectionHelper(sslPort: 7026);
            httpClient = devSslHelper.HttpClient;
        }

        private async void GetQRCode(object sender, EventArgs e)
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLocationAsync();
                    if (location != null)
                    {
                        bool isWithinRadius = Methods.IsWithinRadius(location.Latitude, location.Longitude, TargetLatitude, TargetLongitude, RadiusMeters);
                        if (isWithinRadius)
                        {
                            ImageSource qrCode = await GenerateQRCodeAsync();
                            if (qrCode != null)
                            {
                                MyQRCode.Source = qrCode;
                                QRCodeButton.IsVisible = false;
                                QRCodeText.Text = "QR Code généré avec succès ! Veuillez désormais le scanner";
                            }
                        }
                        else
                        {
                            await DisplayAlert("Erreur", "Rapprochez vous de l'école 3iL pour générer le QR code.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Impossible d'obtenir la localisation actuelle.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Erreur", "La permission de localisation n'a pas été accordée.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la récupération de la localisation : " + ex.Message, "OK");
            }
        }

        private async Task<ImageSource> GenerateQRCodeAsync()
        {
            try
            {
                var studentResponse = await httpClient.GetAsync(devSslHelper.DevServerRootUrl + "/api/Students/getRandomStudent");

                if (studentResponse.IsSuccessStatusCode)
                {
                    string studentContent = await studentResponse.Content.ReadAsStringAsync();
                    var student = JsonConvert.DeserializeObject<Students>(studentContent);

                    DateTime expirationDate = DateTime.Now.AddMinutes(2);
                    string qrCodeContent = student.Id + "|" + expirationDate;
                    var response = await httpClient.GetAsync(devSslHelper.DevServerRootUrl + $"/api/EncryptionDecryption/encrypt?plainText={qrCodeContent}");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.L);
                        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                        byte[] qrCodeBytes = qrCode.GetGraphic(20);
                        QRCodeInfos.Text = student.StudentName + " | Expire le : " + expirationDate.ToString("MM/dd/yyyy HH:mm:ss");
                        return ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Échec de la requête HTTP pour générer le QR code.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Erreur", "Échec de la requête HTTP pour récupérer l'élève connecté.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la génération du QR code : " + ex.Message, "OK");
            }

            return null;
        }

        private void ResetDisplay()
        {
            MyQRCode.Source = "qrcode";
            QRCodeButton.IsVisible = true;
            QRCodeText.Text = "Vous pouvez générer un QR Code en appuyant sur le bouton ci-dessous";
            QRCodeInfos.Text = "";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ResetDisplay();
        }
    }
}