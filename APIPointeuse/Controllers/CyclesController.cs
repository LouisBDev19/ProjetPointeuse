using APIPointeuse.Data;
using APIPointeuse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CyclesController : ControllerBase
    {

        private readonly DataContext _dataContext;

        public CyclesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: api/<CyclesController>
        [HttpGet]
        [Route("getCyclesList")]
        public async Task<ActionResult<List<Cycles>>> Get()
        {
            return Ok(await _dataContext.Cycles
                                        .Where(c => !c.IsDeleted)
                                        .ToListAsync());
        }

        // GET: api/<CyclesController>
        [HttpGet]
        [Route("getAllCyclesList")]
        public async Task<ActionResult<List<Cycles>>> GetAll()
        {
            return Ok(await _dataContext.Cycles
                                        .ToListAsync());
        }

        // GET api/<CyclesController>/5
        [HttpGet]
        [Route("getCycle/{id}")]
        public async Task<ActionResult<Cycles>> Get(int id)
        {
            var cycle = await _dataContext.Cycles
                                          .Where(c => !c.IsDeleted)
                                          .FirstOrDefaultAsync(c => c.Id == id);

            if (cycle == null)
            {
                return BadRequest("Aucun cycle trouvé.");
            }
            return Ok(cycle);
        }

        // GET api/<CyclesController>/5
        [HttpGet]
        [Route("getAllCycle/{id}")]
        public async Task<ActionResult<Cycles>> GetAll(int id)
        {
            var cycle = await _dataContext.Cycles
                                          .FirstOrDefaultAsync(c => c.Id == id);

            if (cycle == null)
            {
                return BadRequest("Aucun cycle trouvé.");
            }
            return Ok(cycle);
        }

        // POST api/<CyclesController>
        [HttpPost]
        [Route("addCycle")]
        public async Task<ActionResult<List<Cycles>>> Add(Cycles cycle)
        {
            _dataContext.Cycles.Add(cycle);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Cycles.OrderByDescending(s => s.Id).FirstOrDefaultAsync());
        }

        // PUT api/<CyclesController>/5
        [HttpPut]
        [Route("updateCycle/{id}")]
        public async Task<ActionResult<List<Cycles>>> Update(int id, Cycles request)
        {
            var dbCycle = await _dataContext.Cycles.FindAsync(id);
            if (dbCycle == null)
            {
                return BadRequest("Aucun cycle trouvé.");
            }
            dbCycle.Name = request.Name;

            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Cycles.ToListAsync());
        }

        // DELETE api/<CyclesController>/5
        [HttpDelete]
        [Route("deleteCycle/{id}")]
        public async Task<ActionResult<Cycles>> Delete(int id)
        {
            var dbCycle = await _dataContext.Cycles.FindAsync(id);
            if (dbCycle == null)
            {
                return BadRequest("Aucun cycle trouvé.");
            }

            _dataContext.Cycles.Remove(dbCycle);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Cycles.ToListAsync());
        }

        // DELETE api/<CyclesController>/5
        [HttpDelete]
        [Route("softDeleteCycle/{id}")]
        public async Task<ActionResult<Cycles>> SoftDelete(int id)
        {
            var dbCycle = await _dataContext.Cycles.FindAsync(id);
            if (dbCycle == null)
            {
                return BadRequest("Aucun cycle trouvé.");
            }

            bool softDeleteStatus = dbCycle.IsDeleted ? false : true;

            dbCycle.IsDeleted = softDeleteStatus;
            _dataContext.Entry(dbCycle).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();

            return Ok(softDeleteStatus);
        }
    }
}
