using System;

namespace cw5.Models
{
    public class Student
    {
        public int IdEnrollment { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }
    }
}