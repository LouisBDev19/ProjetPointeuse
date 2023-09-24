using AForge.Video.DirectShow;
using APIPointeuse.Models;
using BorneDesktopPointeuse.Common;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ZXing;
using ZXing.Windows.Compatibility;

namespace BorneDesktopPointeuse
{
    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
        private Thread listenerThread;
        public Form1()
        {
            InitializeComponent();

            listenerThread = new Thread(StartListening);
            listenerThread.Start();
        }

        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        private void Form1_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (filterInfoCollection.Count > 0)
            {
                videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[0].MonikerString);
                videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
                videoCaptureDevice.Start();
                timer1.Start();
            }
            else
            {
                pictureBox.Visible = false;
                txtQRCode.Visible = false;
                lblMessage.BackColor = Color.Red;
                lblMessage.ForeColor = Color.White;
                lblMessage.Text = "Aucune caméra trouvée. Borne indisponible, veuillez contacter un administrateur";
            }
        }

        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.WaitForStop();
            }

            tcpListener.Stop();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                Result qrCodeContent = barcodeReader.Decode((Bitmap)pictureBox.Image);
                if (qrCodeContent != null)
                {
                    string encodedCipherText = System.Net.WebUtility.UrlEncode(qrCodeContent.ToString());
                    using (HttpClient _httpClient = new HttpClient())
                    {
                        var response = await _httpClient.GetAsync("https://localhost:7026/api/EncryptionDecryption/decrypt?cipherText=" + encodedCipherText);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            ArrivalDateTime arrivalData = Methods.ExplodeStringToArrivalDateTime(content.ToString(), '|');

                            if (arrivalData != null)
                            {
                                var json = JsonConvert.SerializeObject(arrivalData);
                                var arrivalDateTimeContent = new StringContent(json, Encoding.UTF8, "application/json");
                                var arrivalDateTimeResponse = await _httpClient.PostAsync("https://localhost:7026/api/ArrivalDateTime/addArrivalDateTime", arrivalDateTimeContent);
                                if (arrivalDateTimeResponse.IsSuccessStatusCode)
                                {
                                    txtQRCode.Text = "Successful!";
                                }
                                else
                                {
                                    txtQRCode.Text = "Expired!";
                                }
                            }
                        }
                    }
                }
            }
        }

        private void StartListening()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 1234);
                tcpListener.Start();

                while (true)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();

                    Thread clientThread = new Thread(HandleClientCommunication);
                    clientThread.Start(client);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Erreur de socket : " + ex.Message);
            }
        }

        private void HandleClientCommunication(object clientObject)
        {
            TcpClient client = (TcpClient)clientObject;
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            bool response = true;

            byte[] responseBytes = BitConverter.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);

            stream.Close();
            client.Close();
        }
    }
}