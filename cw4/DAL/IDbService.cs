using System.Collections.Generic;
using cw4.Models;

namespace cw4.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public IEnumerable<Student> GetStudents(string IndexNumber);
        public Enrollment GetEnrollment(string indexNumber);
    }
}