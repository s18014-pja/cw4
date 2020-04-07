using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using cw4.DAL;
using cw4.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw4.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }
        
        [HttpGet]
        public IActionResult GetStudent()
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            return Ok(_dbService.GetStudents(indexNumber));
        }
        
        [HttpGet("{indexNumber}/{enrollment}")]
        public IActionResult GetStudent(string indexNumber, string enrollment)
        {
            return enrollment.Equals("enrollment") ? Ok(_dbService.GetEnrollment(indexNumber)) : Ok("Błędny parametr");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //... add
            //... generate
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult PutSomething(int id)
        {
            return Ok("Aktualizacja dokończona");
            // connection.Execute("insert into Person (FirstName, LastName) values (@FirstName, @LastName)", person);
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteSomething(int id)
        {
            return Ok("Usuwanie ukończone");
        }
        
    }
}