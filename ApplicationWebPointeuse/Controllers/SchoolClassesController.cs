using APIPointeuse.Models;
using ApplicationWebPointeuse.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApplicationWebPointeuse.Controllers
{
    public class SchoolClassesController : Controller
    {

        private readonly ILogger<SchoolClassesController> _logger;
        private readonly HttpClient _httpClient;

        public SchoolClassesController(ILogger<SchoolClassesController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.SCHOOLCLASSES_MODEL + Constants.GET_LIST_SCHOOLCLASSES);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var schoolclasses = JsonConvert.DeserializeObject<List<Schoolclasses>>(content);
                ViewData["Schoolclasses"] = schoolclasses;
            }
            else
            {
                ViewData["Schoolclasses"] = new List<Schoolclasses>();
            }

            return View();
        }

        public async Task<IActionResult> DisplaySchoolclass(int id)
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.SCHOOLCLASSES_MODEL + Constants.GET_ID_SCHOOLCLASS + id);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var schoolclass = JsonConvert.DeserializeObject<Schoolclasses>(content);
                ViewData["Schoolclass"] = schoolclass;
            }
            else
            {
                ViewData["Schoolclass"] = null;
            }

            return View();
        }

        public async Task<IActionResult> Cycles()
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.CYCLES_MODEL + Constants.GET_LIST_CYCLES);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cycles = JsonConvert.DeserializeObject<List<Cycles>>(content);
                ViewData["Cycles"] = cycles;
            }
            else
            {
                ViewData["Cycles"] = new List<Cycles>();
            }

            return View();
        }

        public async Task<IActionResult> DisplayCycle(int id)
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.SCHOOLCLASSES_MODEL + Constants.GET_CYCLE_SCHOOLCLASSES + id);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var schoolclasses = JsonConvert.DeserializeObject<List<Schoolclasses>>(content);
                ViewData["Schoolclasses"] = schoolclasses;
            }
            else
            {
                ViewData["Schoolclasses"] = null;
            }

            return View();
        }

        public async Task<IActionResult> Sections()
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.SECTIONS_MODEL + Constants.GET_LIST_SECTIONS);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sections = JsonConvert.DeserializeObject<List<Sections>>(content);
                ViewData["Sections"] = sections;
            }
            else
            {
                ViewData["Sections"] = new List<Sections>();
            }

            return View();
        }

        public async Task<IActionResult> DisplaySection(int id)
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.SCHOOLCLASSES_MODEL + Constants.GET_SECTION_SCHOOLCLASSES + id);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var schoolclasses = JsonConvert.DeserializeObject<List<Schoolclasses>>(content);
                ViewData["Schoolclasses"] = schoolclasses;
            }
            else
            {
                ViewData["Schoolclasses"] = null;
            }

            return View();
        }

        public async Task<IActionResult> Subsections()
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.SUBSECTIONS_MODEL + Constants.GET_LIST_SUBSECTIONS);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var subsections = JsonConvert.DeserializeObject<List<Subsections>>(content);
                ViewData["Subsections"] = subsections;
            }
            else
            {
                ViewData["Subsections"] = new List<Subsections>();
            }

            return View();
        }

        public async Task<IActionResult> DisplaySubsection(int id)
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.SCHOOLCLASSES_MODEL + Constants.GET_SUBSECTION_SCHOOLCLASSES + id);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var schoolclasses = JsonConvert.DeserializeObject<List<Schoolclasses>>(content);
                ViewData["Schoolclasses"] = schoolclasses;
            }
            else
            {
                ViewData["Schoolclasses"] = null;
            }

            return View();
        }
    }
}
