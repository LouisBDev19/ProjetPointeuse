using APIPointeuse.Models;
using ApplicationWebPointeuse.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApplicationWebPointeuse.Controllers
{
    public class SignaturesController : Controller
    {
        private readonly ILogger<SignaturesController> _logger;
        private readonly HttpClient _httpClient;

        public SignaturesController(ILogger<SignaturesController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.STUDENTS_MODEL + Constants.GET_STUDENTS);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var studentDataList = JsonConvert.DeserializeObject<List<Students>>(content);
                    ViewData["Students"] = studentDataList;
            }
            else
            {
                ViewData["Students"] = null;
            }

            return View();
        }
    }
}
