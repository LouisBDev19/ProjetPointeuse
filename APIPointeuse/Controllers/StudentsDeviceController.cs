using APIPointeuse.Data;
using APIPointeuse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsDeviceController : ControllerBase
    {

        private readonly DataContext _dataContext;

        public StudentsDeviceController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: api/<StudentsDeviceController>
        [HttpGet]
        [Route("getStudentsDeviceList")]
        public async Task<ActionResult<List<StudentsDevice>>> Get()
        {
            var studentdevice = await _dataContext.StudentsDevice
                                                  .Include(sc => sc.Students)
                                                  .Where(sc => !sc.IsDeleted)
                                                  .ToListAsync();

            return Ok(studentdevice);
        }

        // GET: api/<StudentsDeviceController>
        [HttpGet]
        [Route("getAllStudentsDeviceList")]
        public async Task<ActionResult<List<StudentsDevice>>> GetAll()
        {
            var studentdevice = await _dataContext.StudentsDevice
                                                  .Include(sc => sc.Students)
                                                  .ToListAsync();

            return Ok(studentdevice);
        }

        // GET api/<StudentsDeviceController>/5
        [HttpGet]
        [Route("getStudentDevice/{id}")]
        public async Task<ActionResult<StudentsDevice>> Get(int id)
        {
            var studentdevice = await _dataContext.StudentsDevice
                                                  .Include(sc => sc.Students)
                                                  .Where(sc => !sc.IsDeleted)
                                                  .FirstOrDefaultAsync(sc => sc.Id == id);
            if (studentdevice == null)
            {
                return NotFound();
            }

            return Ok(studentdevice);
        }

        // GET api/<StudentsDeviceController>/5
        [HttpGet]
        [Route("getAllStudentDevice/{id}")]
        public async Task<ActionResult<StudentsDevice>> GetAll(int id)
        {
            var studentdevice = await _dataContext.StudentsDevice
                                                  .Include(sc => sc.Students)
                                                  .FirstOrDefaultAsync(sc => sc.Id == id);
            if (studentdevice == null)
            {
                return NotFound();
            }

            return Ok(studentdevice);
        }

        // POST api/<StudentsDeviceController>
        [HttpPost]
        [Route("addStudentDevice")]
        public async Task<ActionResult<List<StudentsDevice>>> Add(StudentsDevice studentdevice)
        {
            _dataContext.StudentsDevice.Add(studentdevice);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.StudentsDevice.ToListAsync());
        }

        // PUT api/<StudentsDeviceController>/5
        [HttpPut]
        [Route("updateStudentDevice")]
        public async Task<ActionResult<List<StudentsDevice>>> Update(StudentsDevice request)
        {
            var dbStudentDevice = await _dataContext.StudentsDevice.FindAsync(request.Id);
            if (dbStudentDevice == null)
            {
                return BadRequest("Aucun appareil d'étudiant trouvé.");
            }
            dbStudentDevice.IdStudent = request.IdStudent;
            dbStudentDevice.System = request.System;
            dbStudentDevice.RegistrationDate = request.RegistrationDate;

            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.StudentsDevice.ToListAsync());
        }

        // DELETE api/<StudentsDeviceController>/5
        [HttpDelete]
        [Route("deleteStudentDevice/{id}")]
        public async Task<ActionResult<StudentsDevice>> Delete(int id)
        {
            var dbStudentDevice = await _dataContext.StudentsDevice.FindAsync(id);
            if (dbStudentDevice == null)
            {
                return BadRequest("Aucun appareil d'étudiant trouvé.");
            }

            _dataContext.StudentsDevice.Remove(dbStudentDevice);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.StudentsDevice.ToListAsync());
        }

        // DELETE api/<StudentsDeviceController>/5
        [HttpDelete]
        [Route("softDeleteStudentDevice/{id}")]
        public async Task<ActionResult<StudentsDevice>> SoftDelete(int id)
        {
            var dbStudentDevice = await _dataContext.StudentsDevice.FindAsync(id);
            if (dbStudentDevice == null)
            {
                return BadRequest("Aucun appareil d'étudiant trouvé.");
            }

            dbStudentDevice.IsDeleted = true;
            _dataContext.Entry(dbStudentDevice).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.StudentsDevice.ToListAsync());
        }
    }
}
