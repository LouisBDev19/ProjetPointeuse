using APIPointeuse.Models;
using ApplicationWebPointeuse.Common;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Web.WebPages;

namespace ApplicationWebPointeuse.Controllers
{
    public class StudentsController : Controller
    {

        private readonly ILogger<StudentsController> _logger;
        private readonly HttpClient _httpClient;

        public StudentsController(ILogger<StudentsController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.STUDENTS_MODEL + Constants.GET_LIST_STUDENTS);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var students = JsonConvert.DeserializeObject<List<Students>>(content);
                ViewData["Students"] = students;
            }
            else
            {
                ViewData["Students"] = null;
            }

            return View();
        }

        public async Task<IActionResult> DisplayStudent(int id)
        {
            var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.STUDENTS_MODEL + Constants.GET_ID_STUDENT + id);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeAnonymousType(content, new { student = new Students(), uniqueDates = new List<Dictionary<string, DateTime?>>() });
                ViewData["Student"] = result.student;
                ViewData["UniqueDates"] = result.uniqueDates;
            }
            else
            {
                ViewData["Student"] = null;
                ViewData["UniqueDates"] = null;
            }

            return View();
        }

        public async Task<IActionResult> DisplayStudentPDF(string id)
        {
            var idArray = id.Split(',');

            Document document = new Document();
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            PdfPageEvent pdfPageEvent = new PdfPageEvent();
            writer.PageEvent = pdfPageEvent;

            string studentInfos = "";

            foreach (var studentId in idArray)
            {
                var response = await _httpClient.GetAsync(Constants.API_GLOBAL_URI + Constants.STUDENTS_MODEL + Constants.GET_ID_STUDENT + studentId);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeAnonymousType(content, new { student = new Students(), uniqueDates = new List<Dictionary<string, DateTime?>>() });

                studentInfos = result.student.FirstName + " " + result.student.LastName;
                if (result.student.IdSchoolclass != null)
                {
                    studentInfos = result.student.FirstName + " " + result.student.LastName + " | " + result.student.Schoolclasses.Cycles.Name + " " + result.student.Schoolclasses.Sections.Name + " " + result.student.Schoolclasses.Subsections.Name;
                }

                Paragraph studentTitle = new Paragraph(studentInfos);
                studentTitle.Alignment = Element.ALIGN_CENTER;

                PdfPTable table = new PdfPTable(3);

                PdfPCell cell1 = new PdfPCell(new Phrase("Jour"));
                table.AddCell(cell1);

                PdfPCell cell2 = new PdfPCell(new Phrase("Matin"));
                table.AddCell(cell2);

                PdfPCell cell3 = new PdfPCell(new Phrase("Après-midi"));
                table.AddCell(cell3);

                Methods methods = new Methods();

                var uniqueDates = result.uniqueDates;

                if (result.student.Schoolclasses?.Periods?.Count > 0)
                {
                    var datesPeriod = result.student.Schoolclasses.Periods;

                    methods.datesPeriods(uniqueDates, datesPeriod, table);
                }
                else
                {
                    methods.datesPeriods(uniqueDates, null, table);
                }

                document.Add(studentTitle);
                document.Add(Chunk.NEWLINE);
                document.Add(table);
                document.NewPage();
            }

            document.Close();

            byte[] pdfBytes = memoryStream.ToArray();
            return File(pdfBytes, "application/pdf", "students.pdf");
        }
    }
}
