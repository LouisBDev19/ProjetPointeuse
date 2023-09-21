using APIPointeuse.Data;
using APIPointeuse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolclassesController : ControllerBase
    {

        private readonly DataContext _dataContext;

        public SchoolclassesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: api/<SchoolclassesController>
        [HttpGet]
        [Route("getSchoolclassesList")]
        public async Task<ActionResult<List<Schoolclasses>>> Get()
        {
            var schoolclass = await _dataContext.Schoolclasses
                                                .Include(sc => sc.Cycles)
                                                .Include(sc => sc.Sections)
                                                .Include(sc => sc.Subsections)
                                                .Where(sc => !sc.IsDeleted)
                                                .ToListAsync();

            return Ok(schoolclass);
        }

        // GET: api/<SchoolclassesController>
        [HttpGet]
        [Route("getAllSchoolclassesList")]
        public async Task<ActionResult<List<Schoolclasses>>> GetAll()
        {
            var schoolclass = await _dataContext.Schoolclasses
                                                .Include(sc => sc.Cycles)
                                                .Include(sc => sc.Sections)
                                                .Include(sc => sc.Subsections)
                                                .ToListAsync();

            return Ok(schoolclass);
        }

        // GET api/<SchoolclassesController>/5
        [HttpGet]
        [Route("getSchoolclass/{id}")]
        public async Task<ActionResult<Schoolclasses>> Get(int id)
        {
            var schoolclass = await _dataContext.Schoolclasses
                .Include(sc => sc.Cycles)
                .Include(sc => sc.Sections)
                .Include(sc => sc.Subsections)
                .Include(sc => sc.Students)
                .Include(sc => sc.Periods)
                .Where(sc => !sc.IsDeleted)
                .Where(sc => sc.Id == id)
                .Select(sc => new
                {
                    SchoolClass = sc,
                    Students = sc.Students
                                      .Where(s => !s.IsDeleted)
                                      .ToList(),
                    SortedPeriods = sc.Periods
                                      .Where(p => !p.IsDeleted)
                                      .OrderBy(p => p.BeginningPeriod)
                                      .ToList()
                })
                .FirstOrDefaultAsync();

            if (schoolclass == null)
            {
                return BadRequest("Aucune classe trouvée.");
            }

            // Vous pouvez maintenant accéder à la classe avec les périodes triées dans l'ordre souhaité.
            var schoolClassWithSortedPeriods = schoolclass.SchoolClass;
            schoolClassWithSortedPeriods.Students = schoolclass.Students;
            schoolClassWithSortedPeriods.Periods = schoolclass.SortedPeriods;

            return Ok(schoolClassWithSortedPeriods);
        }

        // GET api/<SchoolclassesController>/5
        [HttpGet]
        [Route("getAllSchoolclass/{id}")]
        public async Task<ActionResult<Schoolclasses>> GetAll(int id)
        {
            var schoolclass = await _dataContext.Schoolclasses
                                                .Include(sc => sc.Cycles)
                                                .Include(sc => sc.Sections)
                                                .Include(sc => sc.Subsections)
                                                .Include(sc => sc.Students)
                                                .Include(sc => sc.Periods)
                                                .FirstOrDefaultAsync(sc => sc.Id == id);
            if (schoolclass == null)
            {
                return BadRequest("Aucune classe trouvée.");
            }
            return Ok(schoolclass);
        }

        // GET api/<SchoolclassesController>/5
        [HttpGet]
        [Route("getAllSchoolclassNoIncludes/{id}")]
        public async Task<ActionResult<Schoolclasses>> GetAllNoIncludes(int id)
        {
            var schoolclass = await _dataContext.Schoolclasses
                                                .FirstOrDefaultAsync(sc => sc.Id == id);
            if (schoolclass == null)
            {
                return BadRequest("Aucune classe trouvée.");
            }
            return Ok(schoolclass);
        }

        [HttpGet]
        [Route("Cycle/{id}")]
        public async Task<ActionResult<List<Schoolclasses>>> GetByCycleId(int id)
        {
            var schoolclasses = await _dataContext.Schoolclasses
                                                    .Include(sc => sc.Cycles)
                                                    .Include(sc => sc.Sections)
                                                    .Include(sc => sc.Subsections)
                                                    .Include(sc => sc.Students)
                                                    .Where(sc => !sc.IsDeleted && sc.IdCycle == id)
                                                    .Select(sc => new Schoolclasses
                                                    {
                                                        Id = sc.Id,
                                                        Cycles = sc.Cycles,
                                                        Sections = sc.Sections,
                                                        Subsections = sc.Subsections,
                                                        Students = sc.Students.Where(s => !s.IsDeleted).ToList()
                                                    })
                                                    .ToListAsync();

            if (schoolclasses == null || !schoolclasses.Any())
            {
                return BadRequest("Aucune classe trouvée dans ce cycle.");
            }

            return Ok(schoolclasses);
        }

        // GET api/<SchoolclassesController>/Cycle/5
        [HttpGet]
        [Route("Cycle/getSchoolclass/{id}")]
        public async Task<bool> CycleIsSchoolclass(int id)
        {
            var schoolclasses = await _dataContext.Schoolclasses
                                                    .Include(sc => sc.Cycles)
                                                    .Include(sc => sc.Sections)
                                                    .Include(sc => sc.Subsections)
                                                    .Include(sc => sc.Students)
                                                    .Where(sc => !sc.IsDeleted && sc.IdCycle == id)
                                                    .FirstOrDefaultAsync();

            return schoolclasses != null;
        }

        // GET api/<SchoolclassesController>/Section/5
        [HttpGet]
        [Route("Section/{id}")]
        public async Task<ActionResult<List<Schoolclasses>>> GetBySectionId(int id)
        {
            var schoolclasses = await _dataContext.Schoolclasses
                                                    .Include(sc => sc.Cycles)
                                                    .Include(sc => sc.Sections)
                                                    .Include(sc => sc.Subsections)
                                                    .Include(sc => sc.Students)
                                                    .Where(sc => !sc.IsDeleted && sc.IdSection == id)
                                                    .Select(sc => new Schoolclasses
                                                    {
                                                        Id = sc.Id,
                                                        Cycles = sc.Cycles,
                                                        Sections = sc.Sections,
                                                        Subsections = sc.Subsections,
                                                        Students = sc.Students.Where(s => !s.IsDeleted).ToList()
                                                    })
                                                    .ToListAsync();

            if (schoolclasses == null)
            {
                return BadRequest("Aucune classe trouvée dans cette section.");
            }
            return Ok(schoolclasses);
        }

        // GET api/<SchoolclassesController>/Section/5
        [HttpGet]
        [Route("Section/getSchoolclass/{id}")]
        public async Task<bool> SectionIsSchoolclass(int id)
        {
            var schoolclasses = await _dataContext.Schoolclasses
                                                    .Include(sc => sc.Cycles)
                                                    .Include(sc => sc.Sections)
                                                    .Include(sc => sc.Subsections)
                                                    .Include(sc => sc.Students)
                                                    .Where(sc => !sc.IsDeleted && sc.IdSection == id)
                                                    .FirstOrDefaultAsync();

            return schoolclasses != null;
        }

        // GET api/<SchoolclassesController>/Subsection/5
        [HttpGet]
        [Route("Subsection/{id}")]
        public async Task<ActionResult<List<Schoolclasses>>> GetBySubsectionId(int id)
        {
            var schoolclasses = await _dataContext.Schoolclasses
                                                    .Include(sc => sc.Cycles)
                                                    .Include(sc => sc.Sections)
                                                    .Include(sc => sc.Subsections)
                                                    .Include(sc => sc.Students)
                                                    .Where(sc => !sc.IsDeleted && sc.IdSubsection == id)
                                                    .Select(sc => new Schoolclasses
                                                    {
                                                        Id = sc.Id,
                                                        Cycles = sc.Cycles,
                                                        Sections = sc.Sections,
                                                        Subsections = sc.Subsections,
                                                        Students = sc.Students.Where(s => !s.IsDeleted).ToList()
                                                    })
                                                    .ToListAsync();

            if (schoolclasses == null)
            {
                return BadRequest("Aucune classe trouvée dans cette sous-section.");
            }
            return Ok(schoolclasses);
        }

        // GET api/<SchoolclassesController>/Subsection/5
        [HttpGet]
        [Route("Subsection/getSchoolclass/{id}")]
        public async Task<bool> SubsectionIsSchoolclass(int id)
        {
            var schoolclasses = await _dataContext.Schoolclasses
                                                    .Include(sc => sc.Cycles)
                                                    .Include(sc => sc.Sections)
                                                    .Include(sc => sc.Subsections)
                                                    .Include(sc => sc.Students)
                                                    .Where(sc => !sc.IsDeleted && sc.IdSubsection == id)
                                                    .FirstOrDefaultAsync();

            return schoolclasses != null;
        }


        // POST api/<SchoolclassesController>
        [HttpPost]
        [Route("addSchoolclass")]
        public async Task<ActionResult<List<Schoolclasses>>> Add(Schoolclasses schoolclass)
        {
            _dataContext.Schoolclasses.Add(schoolclass);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Schoolclasses.OrderByDescending(sc => sc.Id).FirstOrDefaultAsync());
        }

        // PUT api/<SchoolclassesController>/5
        [HttpPut]
        [Route("updateSchoolclass/{id}")]
        public async Task<ActionResult<List<Schoolclasses>>> Update(int id, Schoolclasses request)
        {
            var dbSchoolclass = await _dataContext.Schoolclasses.FindAsync(id);
            if (dbSchoolclass == null)
            {
                return BadRequest("Aucune classe trouvée.");
            }
            dbSchoolclass.IdCycle = request.IdCycle;
            dbSchoolclass.IdSection = request.IdSection;
            dbSchoolclass.IdSubsection = request.IdSubsection;
            dbSchoolclass.IsDeleted = request.IsDeleted;

            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Schoolclasses.ToListAsync());
        }

        // DELETE api/<SchoolclassesController>/5
        [HttpDelete]
        [Route("deleteSchoolclass/{id}")]
        public async Task<ActionResult<Schoolclasses>> Delete(int id)
        {
            var dbSchoolclass = await _dataContext.Schoolclasses.FindAsync(id);
            if (dbSchoolclass == null)
            {
                return BadRequest("Aucune classe trouvée.");
            }

            _dataContext.Schoolclasses.Remove(dbSchoolclass);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Schoolclasses.ToListAsync());
        }

        // DELETE api/<SchoolclassesController>/5
        [HttpDelete]
        [Route("softDeleteSchoolclass/{id}")]
        public async Task<ActionResult<Schoolclasses>> SoftDelete(int id)
        {
            var dbSchoolclass = await _dataContext.Schoolclasses.FindAsync(id);
            if (dbSchoolclass == null)
            {
                return BadRequest("Aucune classe trouvée.");
            }

            bool softDeleteStatus = dbSchoolclass.IsDeleted ? false : true;

            dbSchoolclass.IsDeleted = softDeleteStatus;
            _dataContext.Entry(dbSchoolclass).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();

            return Ok(softDeleteStatus);
        }
    }
}
