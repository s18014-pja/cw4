using System;
using System.Data.SQLite;

namespace cw4.Models
{
    public class Student
    {
        public int IdEnrollment { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        
        public string BirthDate { get; set; }
        
    }
}