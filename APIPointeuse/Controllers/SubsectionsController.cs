using APIPointeuse.Data;
using APIPointeuse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubsectionsController : ControllerBase
    {

        private readonly DataContext _dataContext;

        public SubsectionsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: api/<SubsectionsController>
        [HttpGet]
        [Route("getSubsectionsList")]
        public async Task<ActionResult<List<Subsections>>> Get()
        {
            return Ok(await _dataContext.Subsections
                                        .Where(sb => !sb.IsDeleted)
                                        .ToListAsync());
        }

        // GET: api/<SubsectionsController>
        [HttpGet]
        [Route("getAllSubsectionsList")]
        public async Task<ActionResult<List<Subsections>>> GetAll()
        {
            return Ok(await _dataContext.Subsections
                                        .ToListAsync());
        }

        // GET api/<SubsectionsController>/5
        [HttpGet]
        [Route("getSubsection/{id}")]
        public async Task<ActionResult<Subsections>> Get(int id)
        {
            var subsection = await _dataContext.Subsections
                                               .Where(sb => !sb.IsDeleted)
                                               .FirstOrDefaultAsync(sb => sb.Id == id);
            if (subsection == null)
            {
                return BadRequest("Aucune sous-section trouvée.");
            }
            return Ok(subsection);
        }

        // GET api/<SubsectionsController>/5
        [HttpGet]
        [Route("getAllSubsection/{id}")]
        public async Task<ActionResult<Subsections>> GetAll(int id)
        {
            var subsection = await _dataContext.Subsections
                                               .FirstOrDefaultAsync(sb => sb.Id == id);
            if (subsection == null)
            {
                return BadRequest("Aucune sous-section trouvée.");
            }
            return Ok(subsection);
        }

        // POST api/<SubsectionsController>
        [HttpPost]
        [Route("addSubsection")]
        public async Task<ActionResult<List<Subsections>>> Add(Subsections subsection)
        {
            _dataContext.Subsections.Add(subsection);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Subsections.OrderByDescending(s => s.Id).FirstOrDefaultAsync());
        }

        // PUT api/<SubsectionsController>/5
        [HttpPut]
        [Route("updateSubsection/{id}")]
        public async Task<ActionResult<List<Subsections>>> Update(int id, Subsections request)
        {
            var dbSubsection = await _dataContext.Subsections.FindAsync(id);
            if (dbSubsection == null)
            {
                return BadRequest("Aucune sous-section trouvée.");
            }
            dbSubsection.Name = request.Name;

            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Subsections.ToListAsync());
        }

        // DELETE api/<SubsectionsController>/5
        [HttpDelete]
        [Route("deleteSubsection/{id}")]
        public async Task<ActionResult<Subsections>> Delete(int id)
        {
            var dbSubsection = await _dataContext.Subsections.FindAsync(id);
            if (dbSubsection == null)
            {
                return BadRequest("Aucune sous-section trouvée.");
            }

            _dataContext.Subsections.Remove(dbSubsection);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Subsections.ToListAsync());
        }

        // DELETE api/<SubsectionsController>/5
        [HttpDelete]
        [Route("softDeleteSubsection/{id}")]
        public async Task<ActionResult<Subsections>> SoftDelete(int id)
        {
            var dbSubsection = await _dataContext.Subsections.FindAsync(id);
            if (dbSubsection == null)
            {
                return BadRequest("Aucune sous-section trouvée.");
            }

            bool softDeleteStatus = dbSubsection.IsDeleted ? false : true;

            dbSubsection.IsDeleted = softDeleteStatus;
            _dataContext.Entry(dbSubsection).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();

            return Ok(softDeleteStatus);
        }
    }
}
