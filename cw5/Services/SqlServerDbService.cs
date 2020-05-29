using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using cw5.DTOs.Requests;
using cw5.Models;

namespace cw5.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private static List<Student> _students = new List<Student>();
        private const string ConnectionString = "Server=localhost;Database=APBD;User Id=SA;Password=Trelek_123";
        // private const string  ConnectionString = "Data Source=db-mssql;Initial Catalog=pgago;Integrated Security=True";
        
        public IEnumerable<Student> GetStudents()
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"select * from Student";
                _students.Clear();
                connection.Open();
                var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString()),
                        IndexNumber = dr["IndexNumber"].ToString(),
                        IdEnrollment = Convert.ToInt32(dr["IdEnrollment"])
                    };
                    _students.Add(st);
                }
            }
            return _students;
        }

        public Student GetStudent(string indexNumber)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"select * from Student where IndexNumber = @indexNumber";
                command.Parameters.AddWithValue("@indexNumber", indexNumber);
                _students.Clear();
                connection.Open();
                var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString()),
                        IndexNumber = dr["IndexNumber"].ToString(),
                        IdEnrollment = Convert.ToInt32(dr["IdEnrollment"]),
                        Password = dr["Password"].ToString()
                    };
                    return st;
                }
            }
            return null;
        }

        public Enrollment GetEnrollment(string indexNumber)
        {
            var enrollment = new Enrollment();
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText =
                    $"select Enrollment.IdEnrollment, Semester, IdStudy, StartDate from Enrollment join Student on Student.IdEnrollment = Enrollment.IdEnrollment where IndexNumber = @indexNumber";
                command.Parameters.AddWithValue("@indexNumber", indexNumber);
                connection.Open();
                var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    enrollment.IdEnrollment = Convert.ToInt32(dr["IdEnrollment"]);
                    enrollment.Semester = Convert.ToInt32(dr["Semester"]);
                    enrollment.IdStudy = Convert.ToInt32(dr["IdStudy"]);
                    enrollment.StartDate = DateTime.Parse(dr["StartDate"].ToString()!);
                }
            }
            return null;
        }

        public Enrollment EnrollStudent(EnrollStudentRequests request)
        {
            Enrollment enrollment = null;
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                var tran = connection.BeginTransaction();
                command.Transaction = tran;
                try
                {
                    command.CommandText = $"select IdStudy from Studies where name=@name";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@name", request.Studies);
                    var dr = command.ExecuteReader();
                    int idStudy;
                    if (!dr.HasRows)
                    {
                        dr.Close();
                        tran.Rollback();
                        return null;
                    }
                    else
                    {
                        dr.Read();
                        idStudy = Convert.ToInt32(dr["IdStudy"]);
                        dr.Close();
                    }
                    
                    var idEnrollment = 0;
                    var startDate = DateTime.Today;
                    
                    command.CommandText = $"select IdEnrollment from Enrollment where IdStudy=@idStudy and Semester=1";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@idStudy", idStudy);
                    dr = command.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        dr.Close();
                        command.CommandText = $"select max(IdEnrollment) from Enrollment";
                        dr = command.ExecuteReader();
                        if (dr.Read())
                        {
                            idEnrollment = Convert.ToInt32(dr[0]);
                            idEnrollment++;
                        }
                        dr.Close();
                        command.CommandText = $"insert into Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(@idEnrollment, @semester, @idStudy, @startDate)";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@idEnrollment", idEnrollment);
                        command.Parameters.AddWithValue("@semester", 1);
                        command.Parameters.AddWithValue("@idStudy", idStudy);
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        if (dr.Read())
                        {
                            idEnrollment = Convert.ToInt32(dr[0]);
                        }
                        dr.Close();
                    }
                    
                    command.CommandText = $"select FirstName from Student where IndexNumber=@indexNumber";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@indexNumber", request.IndexNumber);
                    dr = command.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Close();
                        tran.Rollback();
                        return null;
                    }
                    dr.Close();

                    command.CommandText = $"insert into Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES(@indexNumber, @firstName, @lastName, @birthDate, @idEnrollment)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@indexNumber", request.IndexNumber);
                    command.Parameters.AddWithValue("@firstName", request.FirstName);
                    command.Parameters.AddWithValue("@lastName", request.LastName);
                    command.Parameters.AddWithValue("@birthDate", DateTime.ParseExact(request.BirthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue("@idEnrollment", idEnrollment);
                    command.ExecuteNonQuery();

                    command.CommandText = $"select StartDate from Enrollment where IdEnrollment=@idEnrollment";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@idEnrollment", idEnrollment);
                    dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        startDate = (DateTime)dr["StartDate"];
                    }
                    dr.Close();
                    enrollment = new Enrollment
                    {
                        IdEnrollment = idEnrollment,
                        Semester = 1,
                        IdStudy = idStudy,
                        StartDate = startDate
                    };

                    tran.Commit();
                } catch(SqlException exc)
                {
                    Console.WriteLine(exc);
                    tran.Rollback();
                }
            }
            return enrollment;
        }

        public Enrollment PromoteStudents(PromoteStudentsRequest request)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"PromoteStudents";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Name", request.Studies);
                command.Parameters.AddWithValue("@Semester", request.Semester);
                connection.Open();
                try
                {
                    var dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        var enrollment = new Enrollment
                        {
                            IdEnrollment = Convert.ToInt32(dr["IdEnrollment"]),
                            Semester = Convert.ToInt32(dr["Semester"]),
                            IdStudy = Convert.ToInt32(dr["IdStudy"]),
                            StartDate = DateTime.Parse(dr["StartDate"].ToString()!)
                        };
                        dr.Close();
                        return enrollment;
                    }

                }
                catch (SqlException exc)
                {
                    Console.WriteLine("PromoteStudents: błędny parametr");
                }
            }
            return null;
        }
    }
}