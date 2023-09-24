using APIPointeuse.Data;
using APIPointeuse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArrivalDateTimeController : ControllerBase
    {

        private readonly DataContext _dataContext;

        public ArrivalDateTimeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: api/<ArrivalDateTimeController>
        [HttpGet]
        [Route("getArrivalDateTimeList")]
        public async Task<ActionResult<List<ArrivalDateTime>>> Get()
        {
            var arrivalDateTimeList = await _dataContext.ArrivalDateTime
                                                        .Where(a => !a.IsDeleted)
                                                        .ToListAsync();

            return Ok(arrivalDateTimeList);
        }

        // GET: api/<ArrivalDateTimeController>
        [HttpGet]
        [Route("getAllArrivalDateTimeList")]
        public async Task<ActionResult<List<ArrivalDateTime>>> GetAll()
        {
            var arrivalDateTimeList = await _dataContext.ArrivalDateTime
                                                        .ToListAsync();

            return Ok(arrivalDateTimeList);
        }

        // GET api/<ArrivalDateTimeController>/5
        [HttpGet]
        [Route("getArrivalDateTime/{id}")]
        public async Task<ActionResult<ArrivalDateTime>> Get(int id)
        {
            var arrivalDateTime = await _dataContext.ArrivalDateTime
                                                    .Include(a => a.Students)
                                                    .Where(a => !a.IsDeleted)
                                                    .FirstOrDefaultAsync(a => a.Id == id);
            if (arrivalDateTime == null)
            {
                return NotFound();
            }

            return Ok(arrivalDateTime);
        }

        // GET api/<ArrivalDateTimeController>/5
        [HttpGet]
        [Route("getAllArrivalDateTime/{id}")]
        public async Task<ActionResult<ArrivalDateTime>> GetAll(int id)
        {
            var arrivalDateTime = await _dataContext.ArrivalDateTime
                                                    .Include(a => a.Students)
                                                    .FirstOrDefaultAsync(a => a.Id == id);
            if (arrivalDateTime == null)
            {
                return NotFound();
            }

            return Ok(arrivalDateTime);
        }

        // POST api/<ArrivalDateTimeController>
        [HttpPost]
        [Route("addArrivalDateTime")]
        public async Task<ActionResult<List<ArrivalDateTime>>> Add(ArrivalDateTime arrivalDateTime)
        {
            _dataContext.ArrivalDateTime.Add(arrivalDateTime);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.ArrivalDateTime.OrderByDescending(a => a.Id).FirstOrDefaultAsync());
        }

        // PUT api/<ArrivalDateTimeController>/5
        [HttpPut]
        [Route("updateArrivalDateTime")]
        public async Task<ActionResult<List<ArrivalDateTime>>> Update(ArrivalDateTime request)
        {
            var dbArrivalDateTime = await _dataContext.ArrivalDateTime.FindAsync(request.Id);
            if (dbArrivalDateTime == null)
            {
                return BadRequest("Aucun créneau horaire d'étudiant trouvé.");
            }
            dbArrivalDateTime.IdStudent = request.IdStudent;
            dbArrivalDateTime.ArrivalSavedDate = request.ArrivalSavedDate;

            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.ArrivalDateTime.ToListAsync());
        }

        // DELETE api/<ArrivalDateTimeController>/5
        [HttpDelete]
        [Route("deleteArrivalDateTime/{id}")]
        public async Task<ActionResult<ArrivalDateTime>> Delete(int id)
        {
            var dbArrivalDateTime = await _dataContext.ArrivalDateTime.FindAsync(id);
            if (dbArrivalDateTime == null)
            {
                return BadRequest("Aucun créneau horaire d'étudiant trouvé.");
            }

            _dataContext.ArrivalDateTime.Remove(dbArrivalDateTime);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.ArrivalDateTime.ToListAsync());
        }

        // DELETE api/<ArrivalDateTimeController>/5
        [HttpDelete]
        [Route("softDeleteArrivalDateTime/{id}")]
        public async Task<ActionResult<ArrivalDateTime>> SoftDelete(int id)
        {
            var dbArrivalDateTime = await _dataContext.ArrivalDateTime.FindAsync(id);
            if (dbArrivalDateTime == null)
            {
                return BadRequest("Aucun créneau horaire d'étudiant trouvé.");
            }

            dbArrivalDateTime.IsDeleted = true;
            _dataContext.Entry(dbArrivalDateTime).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.ArrivalDateTime.ToListAsync());
        }

    }
}
