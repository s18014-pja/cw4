using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using cw4.Models;

namespace cw4.DAL
{
    public class RealDbService : IDbService
    {
        private static List<Student> _students;
        private static string _dbfilename = "apbd.sqlite3";
        // private static string _dbpath = "Data Source=./apbd.sqlite3;Version=3";
        private static string _dbpath = AppContext.BaseDirectory.Substring(0,
            AppContext.BaseDirectory.LastIndexOf("bin", StringComparison.Ordinal));
        private static string _fulldbpath = $"Data Source={_dbpath}{_dbfilename};Version=3";
        
        static RealDbService()
        {
            _students = new List<Student>();
        }
        public IEnumerable<Student> GetStudents()
        {
            using (var connection = new SQLiteConnection(_fulldbpath))
            {
                using var command = new SQLiteCommand
                {
                    Connection = connection, CommandText = $"select * from Student"
                };
                
                _students.Clear();
                connection.Open();
                var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        IndexNumber = dr["IndexNumber"].ToString(),
                        IdEnrollment = Convert.ToInt32(dr["IdEnrollment"].ToString())
                    };
                    _students.Add(st);
                }
            }

            return _students;
        }
        
        public IEnumerable<Student> GetStudents(string indexNumber)
        {
            using (var connection = new SQLiteConnection(_fulldbpath))
            {
                 /*using var command = new SQLiteCommand
                {
                    Connection = connection, CommandText = $"select * from Student where IndexNumber = '{indexNumber}'"
                };*/
                
                using var command = new SQLiteCommand
                {
                    Connection = connection, CommandText = $"select * from Student where IndexNumber = @indexNumber"
                };
                command.Parameters.AddWithValue("indexNumber", indexNumber);
                // Console.Write(command.CommandText);
                
                _students.Clear();
                connection.Open();
                var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        IndexNumber = dr["IndexNumber"].ToString(),
                        IdEnrollment = Convert.ToInt32(dr["IdEnrollment"].ToString())
                    };
                    _students.Add(st);
                }
            }
            return _students;
        }

        public Enrollment GetEnrollment(string indexNumber)
        {
            var en = new Enrollment();
            using (var connection = new SQLiteConnection(_fulldbpath))
            {
                using var command = new SQLiteCommand
                {
                    Connection = connection, CommandText = $"select Enrollment.IdEnrollment, Semester, IdStudy, StartDate from Enrollment join Student on Student.IdEnrollment = Enrollment.IdEnrollment where IndexNumber = @indexNumber"
                };
                command.Parameters.AddWithValue("indexNumber", indexNumber);
                
                connection.Open();
                var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    en.IdEnrollment = Convert.ToInt32(dr["IdEnrollment"].ToString());
                    en.Semester = Convert.ToInt32(dr["Semester"].ToString());
                    en.IdStudy = Convert.ToInt32(dr["IdStudy"].ToString());
                    en.StartDate = dr["StartDate"].ToString();
                }
            }
            return en;
        }
    }
}