using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text;

namespace ApplicationWebPointeuse.Controllers
{
    public class TerminalController : Controller
    {
        public IActionResult Index()
        {
            bool applicationStatus = GetWinFormsApplicationStatus();
            ViewBag.ApplicationStatus = applicationStatus;

            return View();
        }

        public bool GetWinFormsApplicationStatus()
        {
            string serverIP = "127.0.0.1";
            int serverPort = 1234;

            try
            {
                using (TcpClient client = new TcpClient(serverIP, serverPort))
                {
                    NetworkStream stream = client.GetStream();

                    string request = "Running or not";
                    byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                    stream.Write(requestBytes, 0, requestBytes.Length);

                    byte[] buffer = new byte[1];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    bool response = BitConverter.ToBoolean(buffer, 0);

                    stream.Close();

                    return response;
                }
            }
            catch (SocketException ex)
            {
                return false;
            }
        }
    }
}
