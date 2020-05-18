using System;

namespace cw5.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int IdEnrollment { get; set; }
        public int  Semester { get; set; }
        public int IdStudy { get; set; }
        public DateTime StartDate { get; set; }
    }
}