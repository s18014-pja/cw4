using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cw5.DTOs.Requests;
using cw5.Models;
using cw5.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace cw5.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDbService _dbService;
        public IConfiguration Configuration { get; set; }

        public StudentsController(IStudentsDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
        }
        
        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            Console.WriteLine(request.Login);
            Console.WriteLine(request.Haslo);
            var student = _dbService.GetStudent(request.Login);
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, student.IndexNumber),
                new Claim(ClaimTypes.Name, student.FirstName),
                new Claim(ClaimTypes.Surname, student.LastName),
                new Claim(ClaimTypes.Role, "employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "s18014",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken=Guid.NewGuid()
            });
        }
        
        [HttpGet]
        [Authorize]
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
        
        /*[HttpGet("{indexNumber}/{enrollment}")]
        public IActionResult GetStudent(string indexNumber, string enrollment)
        {
            return enrollment.Equals("enrollment") ? Ok(_dbService.GetEnrollment(indexNumber)) : Ok(BadRequest("Błędny parametr"));
        }*/
        
        [HttpDelete("{indexNumber}")]
        public IActionResult DeleteStudent(string indexNumber)
        {
            _dbService.DeleteStudent(indexNumber);
            return Ok("Deleted");
        }
        
        [HttpPost("{indexNumber}/{password}")]
        public IActionResult ChangePassword(string indexNumber, string password, ChangePasswordRequests request)
        {
            if (!password.Equals("password")) return Ok(BadRequest("Błędny parametr"));
            var response = _dbService.ChangePassword(indexNumber, request);
            return response == true ? Ok("Password changed") : Ok(BadRequest("Wrong old password"));
        }
    }
}