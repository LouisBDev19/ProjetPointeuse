using APIPointeuse.Data;
using APIPointeuse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodsController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public PeriodsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: api/<PeriodsController>
        [HttpGet]
        [Route("getPeriodsList")]
        public async Task<ActionResult<List<Periods>>> Get()
        {
            return Ok(await _dataContext.Periods
                                        .Where(c => !c.IsDeleted)
                                        .ToListAsync());
        }

        // GET: api/<PeriodsController>
        [HttpGet]
        [Route("getAllPeriodsList")]
        public async Task<ActionResult<List<Periods>>> GetAll()
        {
            return Ok(await _dataContext.Periods
                                        .ToListAsync());
        }

        // GET api/<PeriodsController>/5
        [HttpGet]
        [Route("getPeriod/{id}")]
        public async Task<ActionResult<Periods>> Get(int id)
        {
            var period = await _dataContext.Periods
                                          .Where(c => !c.IsDeleted)
                                          .FirstOrDefaultAsync(c => c.Id == id);

            if (period == null)
            {
                return BadRequest("Aucune période trouvée.");
            }
            return Ok(period);
        }

        // GET api/<PeriodsController>/5
        [HttpGet]
        [Route("getAllPeriod/{id}")]
        public async Task<ActionResult<Periods>> GetAll(int id)
        {
            var period = await _dataContext.Periods
                                          .FirstOrDefaultAsync(c => c.Id == id);

            if (period == null)
            {
                return BadRequest("Aucune période trouvée.");
            }
            return Ok(period);
        }

        // GET api/<PeriodsController>/5
        [HttpGet]
        [Route("getPeriods/{id}")]
        public async Task<ActionResult<Periods>> GetPeriodsBySchoolclassId(int id)
        {
            var period = await _dataContext.Periods
                                          .Where(c => c.IdSchoolclass == id)
                                          .OrderBy(c => c.BeginningPeriod)
                                          .ToListAsync();

            if (period == null)
            {
                return BadRequest("Aucune période trouvée.");
            }
            return Ok(period);
        }

        // GET api/<PeriodsController>/5
        [HttpGet]
        [Route("getValidatedPeriods/{idSchoolclass}/{id}")]
        public async Task<ActionResult<Periods>> GetValidatedPeriods(int idSchoolclass, int id)
        {
            var period = await _dataContext.Periods
                                          .Where(c => c.IdSchoolclass == idSchoolclass && !c.IsDeleted && c.Id != id)
                                          .ToListAsync();

            if (period == null)
            {
                return BadRequest("Aucune période trouvée.");
            }
            return Ok(period);
        }

        // POST api/<PeriodsController>
        [HttpPost]
        [Route("addPeriod")]
        public async Task<ActionResult<List<Periods>>> Add(Periods period)
        {
            _dataContext.Periods.Add(period);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Periods.OrderByDescending(p => p.Id).FirstOrDefaultAsync());
        }

        // PUT api/<PeriodsController>/5
        [HttpPut]
        [Route("updatePeriod/{id}")]
        public async Task<ActionResult<List<Periods>>> Update(int id, Periods request)
        {
            var dbPeriod = await _dataContext.Periods.FindAsync(id);
            if (dbPeriod == null)
            {
                return BadRequest("Aucune période trouvée.");
            }
            dbPeriod.BeginningPeriod = request.BeginningPeriod;
            dbPeriod.EndingPeriod = request.EndingPeriod;

            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Periods.ToListAsync());
        }

        // DELETE api/<PeriodsController>/5
        [HttpDelete]
        [Route("deletePeriod/{id}")]
        public async Task<ActionResult<Periods>> Delete(int id)
        {
            var dbPeriod = await _dataContext.Periods.FindAsync(id);
            if (dbPeriod == null)
            {
                return BadRequest("Aucune période trouvée.");
            }

            _dataContext.Periods.Remove(dbPeriod);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Periods.ToListAsync());
        }

        // DELETE api/<PeriodsController>/5
        [HttpDelete]
        [Route("softDeletePeriod/{id}")]
        public async Task<ActionResult<Periods>> SoftDelete(int id)
        {
            var dbPeriod = await _dataContext.Periods.FindAsync(id);
            if (dbPeriod == null)
            {
                return BadRequest("Aucune période trouvée.");
            }

            bool softDeleteStatus = dbPeriod.IsDeleted ? false : true;

            dbPeriod.IsDeleted = softDeleteStatus;
            _dataContext.Entry(dbPeriod).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();

            return Ok(softDeleteStatus);
        }
    }
}
