using cw5.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw5.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDbService _dbService;

        public StudentsController(IStudentsDbService dbService)
        {
            _dbService = dbService;
        }
        
        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            var response = _dbService.GetStudent(indexNumber);
            return response == null ? Ok(BadRequest("Błędny parametr")) : Ok(response);
        }
        
        [HttpGet("{indexNumber}/{enrollment}")]
        public IActionResult GetStudent(string indexNumber, string enrollment)
        {
            return enrollment.Equals("enrollment") ? Ok(_dbService.GetEnrollment(indexNumber)) : Ok(BadRequest("Błędny parametr"));
        }
    }
}