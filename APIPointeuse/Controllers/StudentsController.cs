using APIPointeuse.Data;
using APIPointeuse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private readonly DataContext _dataContext;

        public StudentsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        // GET: api/<StudentsController>
        [HttpGet]
        [Route("getStudentsList")]
        public async Task<ActionResult<List<Students>>> Get()
        {
            var students = await _dataContext.Students
                                             .Include(s => s.Schoolclasses.Cycles)
                                             .Include(s => s.Schoolclasses.Sections)
                                             .Include(s => s.Schoolclasses.Subsections)
                                             .Include(s => s.ArrivalDateTime)
                                             .Where(s => !s.IsDeleted)
                                             .ToListAsync();
            return Ok(students);
        }

        // GET: api/<StudentsController>
        [HttpGet]
        [Route("getAllStudentsList")]
        public async Task<ActionResult<List<Students>>> GetAll()
        {
            var students = await _dataContext.Students
                                             .Include(s => s.Schoolclasses.Cycles)
                                             .Include(s => s.Schoolclasses.Sections)
                                             .Include(s => s.Schoolclasses.Subsections)
                                             .Include(s => s.ArrivalDateTime)
                                             .ToListAsync();
            return Ok(students);
        }

        // GET: api/<StudentsController>
        [HttpGet]
        [Route("getStudent/{id}")]
        public async Task<ActionResult<List<Students>>> Get(int id)
        {
            var student = await _dataContext.Students
                .Include(s => s.Schoolclasses.Cycles)
                .Include(s => s.Schoolclasses.Sections)
                .Include(s => s.Schoolclasses.Subsections)
                .Include(s => s.Schoolclasses.Periods)
                .Include(s => s.ArrivalDateTime)
                .Where(s => !s.IsDeleted)
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    Student = s,
                    SortedPeriods = s.Schoolclasses.Periods
                                      .Where(p => !p.IsDeleted)
                                      .OrderBy(p => p.BeginningPeriod)
                                      .ToList()
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return BadRequest("Aucun étudiant trouvé.");
            }

            var studentWithSortedPeriods = student.Student;
            studentWithSortedPeriods.Schoolclasses.Periods = student.SortedPeriods;

            var uniqueDates = new List<Dictionary<string, DateTime?>>();

            foreach (var group in studentWithSortedPeriods.ArrivalDateTime.GroupBy(ad => ad.ArrivalSavedDate.Date))
            {
                var morningArrivals = group.Where(ad => ad.ArrivalSavedDate.Hour < 12)
                                            .OrderBy(ad => ad.ArrivalSavedDate.TimeOfDay)
                                            .Take(1);

                var afternoonArrivals = group.Where(ad => ad.ArrivalSavedDate.Hour >= 12)
                                                .OrderBy(ad => ad.ArrivalSavedDate.TimeOfDay)
                                                .Take(1);

                uniqueDates.Add(new Dictionary<string, DateTime?>
                {
                    { "morning", morningArrivals.FirstOrDefault()?.ArrivalSavedDate },
                    { "afternoon", afternoonArrivals.FirstOrDefault()?.ArrivalSavedDate }
                });
            }

            var response = new
            {
                student = studentWithSortedPeriods,
                uniqueDates
            };

            return Ok(response);
        }

        // GET: api/<StudentsController>
        [HttpGet]
        [Route("getStudents")]
        public async Task<ActionResult<List<Students>>> GetArrivalDateTimeStudents()
        {
            var students = await _dataContext.Students
                .Include(s => s.Schoolclasses.Cycles)
                .Include(s => s.Schoolclasses.Sections)
                .Include(s => s.Schoolclasses.Subsections)
                .Include(s => s.Schoolclasses.Periods)
                .Include(s => s.ArrivalDateTime)
                .Where(s => !s.IsDeleted)
                .ToListAsync();

            if (students == null || students.Count == 0)
            {
                return BadRequest("Aucun étudiant trouvé.");
            }

            var studentsData = new List<object>();

            foreach (var student in students)
            {
                var studentWithSortedPeriods = student;
                studentWithSortedPeriods.Schoolclasses.Periods = student.Schoolclasses.Periods
                    .Where(p => !p.IsDeleted)
                    .OrderBy(p => p.BeginningPeriod)
                    .ToList();

                student.UniqueDates = new List<Dictionary<string, DateTime?>>();

                foreach (var group in studentWithSortedPeriods.ArrivalDateTime.GroupBy(ad => ad.ArrivalSavedDate.Date))
                {
                    var morningArrivals = group.Where(ad => ad.ArrivalSavedDate.Hour < 12)
                                                .OrderBy(ad => ad.ArrivalSavedDate.TimeOfDay)
                                                .Take(1);

                    var afternoonArrivals = group.Where(ad => ad.ArrivalSavedDate.Hour >= 12)
                                                  .OrderBy(ad => ad.ArrivalSavedDate.TimeOfDay)
                                                  .Take(1);

                    student.UniqueDates.Add(new Dictionary<string, DateTime?>
            {
                { "morning", morningArrivals.FirstOrDefault()?.ArrivalSavedDate },
                { "afternoon", afternoonArrivals.FirstOrDefault()?.ArrivalSavedDate }
            });
                }
            }

            return Ok(students);
        }

        // GET: api/<StudentsController>
        [HttpGet]
        [Route("getAllStudent/{id}")]
        public async Task<ActionResult<List<Students>>> GetAll(int id)
        {
            var student = await _dataContext.Students
                                            .Include(s => s.Schoolclasses.Cycles)
                                            .Include(s => s.Schoolclasses.Sections)
                                            .Include(s => s.Schoolclasses.Subsections)
                                            .Include(s => s.Schoolclasses.Periods)
                                            .Include(s => s.ArrivalDateTime)
                                            .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return BadRequest("Aucun étudiant trouvé.");
            }

            var uniqueDates = new List<Dictionary<string, DateTime?>>();

            foreach (var group in student.ArrivalDateTime.GroupBy(ad => ad.ArrivalSavedDate.Date))
            {
                var morningArrivals = group.Where(ad => ad.ArrivalSavedDate.Hour < 12)
                                           .OrderBy(ad => ad.ArrivalSavedDate.TimeOfDay)
                                           .Take(1);

                var afternoonArrivals = group.Where(ad => ad.ArrivalSavedDate.Hour >= 12)
                                              .OrderBy(ad => ad.ArrivalSavedDate.TimeOfDay)
                                              .Take(1);

                uniqueDates.Add(new Dictionary<string, DateTime?>
                {
                    { "morning", morningArrivals.FirstOrDefault()?.ArrivalSavedDate },
                    { "afternoon", afternoonArrivals.FirstOrDefault()?.ArrivalSavedDate }
                });
            }

            var response = new
            {
                student,
                uniqueDates
            };

            return Ok(response);
        }

        // GET: api/<StudentsController>
        [HttpGet]
        [Route("getDuplicateEmailStudent/{email}")]
        public async Task<bool> GetDuplicateEmailStudent(string email)
        {
            var student = await _dataContext.Students
                                             .FirstOrDefaultAsync(s => s.Email == email);

            return student != null;
        }

        // GET api/<StudentsController>
        [HttpGet]
        [Route("getSchoolclass/{id}")]
        public async Task<bool> StudentsHasSchoolclass(int id)
        {
            var students = await _dataContext.Students
                                                    .Where(s => s.IdSchoolclass == id)
                                                    .FirstOrDefaultAsync();

            return students != null;
        }

        // POST api/<StudentsController>
        [HttpPost]
        [Route("addStudent")]
        public async Task<ActionResult<List<Students>>> Add(Students student)
        {
            _dataContext.Students.Add(student);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Students.OrderByDescending(s => s.Id).FirstOrDefaultAsync());
        }

        // PUT api/<StudentsController>
        [HttpPut]
        [Route("updateStudent/{id}")]
        public async Task<ActionResult<List<Students>>> Update(int id, Students request)
        {
            var dbStudent = await _dataContext.Students.FindAsync(id);
            if (dbStudent == null)
            {
                return BadRequest("Aucun étudiant trouvé.");
            }
            dbStudent.BirthDate = request.BirthDate;
            dbStudent.FirstName = request.FirstName;
            dbStudent.LastName = request.LastName;
            dbStudent.Email = request.Email;
            dbStudent.PhoneNumber = request.PhoneNumber;
            dbStudent.Signature = request.Signature;
            dbStudent.IdSchoolclass = request.IdSchoolclass;

            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Students.ToListAsync());
        }

        // DELETE api/<StudentsController>/5
        [HttpDelete]
        [Route("deleteStudent/{id}")]
        public async Task<ActionResult<Students>> Delete(int id)
        {
            var dbStudent = await _dataContext.Students.FindAsync(id);
            if (dbStudent == null)
            {
                return BadRequest("Aucun étudiant trouvé.");
            }

            _dataContext.Students.Remove(dbStudent);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Students.ToListAsync());
        }

        // DELETE api/<StudentsController>/5
        [HttpDelete]
        [Route("softDeleteStudent/{id}")]
        public async Task<ActionResult<Students>> SoftDelete(int id)
        {
            var dbStudent = await _dataContext.Students.FindAsync(id);
            if (dbStudent == null)
            {
                return BadRequest("Aucun étudiant trouvé.");
            }

            bool softDeleteStatus = dbStudent.IsDeleted ? false : true;

            dbStudent.IsDeleted = softDeleteStatus;
            _dataContext.Entry(dbStudent).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();

            return Ok(softDeleteStatus);
        }
    }
}
