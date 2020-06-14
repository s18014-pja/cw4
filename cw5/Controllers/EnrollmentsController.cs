using System;
using cw5.Services;
using cw5.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cw5.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentsDbService _dbService;
        
        public EnrollmentsController(IStudentsDbService dbService)
        {
            _dbService = dbService;
        }
        
        [HttpPost]
        // [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequests request)
        {
            var response = _dbService.EnrollStudent(request);
            return response == null ? Ok(BadRequest("Błędny parametr")) : Ok(response);
        }
        
        [HttpPost("{promotions}")]
        // [Authorize(Roles = "employee")]
        public IActionResult PromoteStudents(string promotions, PromoteStudentsRequest request)
        {
            if (!promotions.Equals("promotions")) return Ok(BadRequest("Błędny parametr"));
            var response = _dbService.PromoteStudents(request);
            return response == null ? Ok(BadRequest("Błędny parametr")) : Ok(response);
        }
    }
}