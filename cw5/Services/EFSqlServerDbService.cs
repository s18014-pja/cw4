using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using cw5.DTOs.Requests;
using cw5.Models;
using Microsoft.EntityFrameworkCore;

namespace cw5.Services
{
    public class EFSqlServerDbService : IStudentsDbService
    {
        
        private APBDContext _context; 
            
        public IEnumerable<Student> GetStudents()
        {
            _context = new APBDContext();
            return _context.Student.ToList();
        }

        public Student GetStudent(string indexNumber)
        {
            _context = new APBDContext();
            var student =  _context.Student
                .First(x => x.IndexNumber==indexNumber);
            return student;
        }
        
        public Student GetStudent(string indexNumber, APBDContext context)
        {
            var student =  context.Student
                .First(x => x.IndexNumber==indexNumber);
            return student;
        }

        public void DeleteStudent(string indexNumber)
        {
            _context = new APBDContext();
            var student = new Student
            {
                IndexNumber = indexNumber
            };
            _context.Remove(student);
            _context.SaveChanges();
        }

        /*public Enrollment GetEnrollment(string indexNumber)
        {
            _context = new APBDContext();
            var student =  _context.Student
                .First(x => x.IndexNumber == indexNumber);
            var enrollment = _context.Enrollment
                .First(x => x.IdEnrollment == student.IdEnrollment);
            return enrollment;
        }*/

        public Enrollment EnrollStudent(EnrollStudentRequests request)
        {
            _context = new APBDContext();

            Studies study;
            try
            {
                study = _context.Studies
                    .First(x => x.Name.Equals(request.Studies));
            }
            catch
            {
                return null;
            }
           
            Enrollment enrollment;
            try
            {
                enrollment = _context.Enrollment
                    .Where(x => x.IdStudy.Equals(study.IdStudy))
                    .First(x => x.Semester == 1);
            } catch
            {
                var newEnrollment = new Enrollment
                {
                    IdEnrollment = _context.Enrollment
                        .Max(x => x.IdEnrollment) + 1,
                    Semester = 1,
                    IdStudy = study.IdStudy,
                    StartDate = DateTime.Today
                };
                _context.Enrollment.Add(newEnrollment);
                enrollment = newEnrollment;
            }

            var idEnrollment = enrollment.IdEnrollment;
            var student = new Student
            {
                IndexNumber = request.IndexNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = DateTime.ParseExact(request.BirthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                IdEnrollment = idEnrollment,
                Password = "pass"
            };
            _context.Student.Add(student);
            
            _context.SaveChanges();
            return enrollment;
        }

        public Enrollment PromoteStudents(PromoteStudentsRequest request)
        {
            _context = new APBDContext();

            Studies study;
            Enrollment oldEnrollment;
            // Upewniam się, że w tabeli Enrollment istnieje wpis o podanej wartości Studies i Semester
            try
            {
                study = _context.Studies
                    .First(x => x.Name.Equals(request.Studies));
                oldEnrollment = _context.Enrollment
                    .Where(x => x.IdStudy.Equals(study.IdStudy))
                    .First(x => x.Semester == Convert.ToInt32(request.Semester));
            } catch
            {
                return null;
            }

            Enrollment enrollment;
            try
            {
                enrollment = _context.Enrollment
                    .Where(x => x.IdStudy.Equals(study.IdStudy))
                    .First(x => x.Semester == Convert.ToInt32(request.Semester) + 1);
            } catch
            {
                var newEnrollment = new Enrollment
                    {
                        IdEnrollment = _context.Enrollment
                            .Max(x => x.IdEnrollment) + 1,
                        Semester = Convert.ToInt32(request.Semester) + 1,
                        IdStudy = study.IdStudy,
                        StartDate = DateTime.Today
                    };
                _context.Enrollment.Add(newEnrollment);
                enrollment = newEnrollment;
            }

            var students = _context.Student
                .Where(x => x.IdEnrollment == oldEnrollment.IdEnrollment)
                .ToList();
            foreach (var s in students)
            {
                s.IdEnrollment = enrollment.IdEnrollment;
            }

            _context.SaveChanges();
            return enrollment;
        }

        public bool ChangePassword(string indexNumber, ChangePasswordRequests request)
        {
            _context = new APBDContext();
           
            Student student;
            try
            {
                student = GetStudent(indexNumber, _context);
                if (!student.Password.Equals(request.OldPassword))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            student.Password = request.NewPassword;
            _context.SaveChanges();
            
            /*var updatedStudent = new Student
            {
                IndexNumber = indexNumber,
                Password = request.NewPassword
            };
            
            _context.Attach(updatedStudent);
            _context.Entry(updatedStudent).Property("Password").IsModified = true;
            _context.SaveChanges();*/

            return true;
        }
        
    }
}