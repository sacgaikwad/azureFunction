using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SqlFunction
{
    public static class SqlDbFunction
    {
        [FunctionName("SqlDbFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            List<Course> courses = new List<Course>();

            try
            {
                string connectionString = Environment.GetEnvironmentVariable("SQLAZURESQLCONNSTR_DbConnection");
                //"Server=tcp:azuretutorialdemo.database.windows.net,1433;Initial Catalog=azuretutorial;Persist Security Info=False;User ID=azuredemo;Password=sachin123!@#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    String sql = "SELECT CourseId,CourseName,Rating FROM [dbo].[Course]";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Course course = new Course();
                                course.CourseId = reader["CourseId"].ToString();
                                course.CourseName = reader["CourseName"].ToString();
                                course.Rating = reader["Rating"].ToString();
                                courses.Add(course);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }

            log.LogInformation("Exiting GetCourses");

            return new OkObjectResult(courses);
        }
    }
}
