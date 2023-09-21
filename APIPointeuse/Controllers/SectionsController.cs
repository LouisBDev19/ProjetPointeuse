using APIPointeuse.Data;
using APIPointeuse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {

        private readonly DataContext _dataContext;

        public SectionsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: api/<SectionsController>
        [HttpGet]
        [Route("getSectionsList")]
        public async Task<ActionResult<List<Sections>>> Get()
        {
            return Ok(await _dataContext.Sections
                                        .Where(s => !s.IsDeleted)
                                        .ToListAsync());
        }

        // GET: api/<SectionsController>
        [HttpGet]
        [Route("getAllSectionsList")]
        public async Task<ActionResult<List<Sections>>> GetAll()
        {
            return Ok(await _dataContext.Sections
                                        .ToListAsync());
        }

        // GET api/<SectionsController>/5
        [HttpGet]
        [Route("getSection/{id}")]
        public async Task<ActionResult<Sections>> Get(int id)
        {
            var section = await _dataContext.Sections
                                            .Where(s => !s.IsDeleted)
                                            .FirstOrDefaultAsync(s => s.Id == id);
            if (section == null)
            {
                return BadRequest("Aucune section trouvée.");
            }
            return Ok(section);
        }

        // GET api/<SectionsController>/5
        [HttpGet]
        [Route("getAllSection/{id}")]
        public async Task<ActionResult<Sections>> GetAll(int id)
        {
            var section = await _dataContext.Sections
                                            .FirstOrDefaultAsync(s => s.Id == id);
            if (section == null)
            {
                return BadRequest("Aucune section trouvée.");
            }
            return Ok(section);
        }

        // POST api/<SectionsController>
        [HttpPost]
        [Route("addSection")]
        public async Task<ActionResult<List<Sections>>> Add(Sections section)
        {
            _dataContext.Sections.Add(section);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Sections.OrderByDescending(s => s.Id).FirstOrDefaultAsync());
        }

        // PUT api/<SectionsController>/5
        [HttpPut]
        [Route("updateSection/{id}")]
        public async Task<ActionResult<List<Sections>>> Update(int id, Sections request)
        {
            var dbSection = await _dataContext.Sections.FindAsync(id);
            if (dbSection == null)
            {
                return BadRequest("Aucune section trouvée.");
            }
            dbSection.Name = request.Name;

            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Sections.ToListAsync());
        }

        // DELETE api/<SectionsController>/5
        [HttpDelete]
        [Route("deleteSection/{id}")]
        public async Task<ActionResult<Sections>> Delete(int id)
        {
            var dbSection = await _dataContext.Sections.FindAsync(id);
            if (dbSection == null)
            {
                return BadRequest("Aucune section trouvée.");
            }

            _dataContext.Sections.Remove(dbSection);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Sections.ToListAsync());
        }

        // DELETE api/<SectionsController>/5
        [HttpDelete]
        [Route("softDeleteSection/{id}")]
        public async Task<ActionResult<Sections>> SoftDelete(int id)
        {
            var dbSection = await _dataContext.Sections.FindAsync(id);
            if (dbSection == null)
            {
                return BadRequest("Aucune section trouvée.");
            }

            bool softDeleteStatus = dbSection.IsDeleted ? false : true;

            dbSection.IsDeleted = softDeleteStatus;
            _dataContext.Entry(dbSection).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();

            return Ok(softDeleteStatus);
        }
    }
}
