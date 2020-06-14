using System.Collections.Generic;
using cw5.DTOs.Requests;
using cw5.Models;

namespace cw5.Services
{
    public interface IStudentsDbService
    {
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(string indexNumber);
        public void DeleteStudent(string indexNumber);
        // public Enrollment GetEnrollment(string indexNumber);
        public bool ChangePassword(string indexNumber, ChangePasswordRequests request);
        public Enrollment EnrollStudent(EnrollStudentRequests request);
        public Enrollment PromoteStudents(PromoteStudentsRequest request);

    }
}