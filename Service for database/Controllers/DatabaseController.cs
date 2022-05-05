using EntityLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Service_for_database.Model;

namespace Service_for_database.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly DatabaseContext _dbContext;

        public DatabaseController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("/check")]
        public async Task<IActionResult> Check()
        {
            try
            {
                await ExecuteSqlQuery();
            }
            catch (Exception exception)
            {
                return Problem(
                    detail: exception.Message,
                    title: "Data base connection problem.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            return Ok();
        }
        
        [Authorize]
        [HttpGet("properties")]
        public async Task<ICollection<MetabaseProperty>?> GetProperties()
        {
            List<MetabaseProperty> propertiesList;
            try
            {
                await ExecuteSqlQuery();

                // Получение данных из View.
                propertiesList = await _dbContext.PropertiesInfo.ToListAsync();
            } catch (Exception)
            {
                return null;
            }

            return propertiesList;
        }
        
        [Authorize]
        [HttpGet("systems")]
        public async Task<ICollection<MetabaseSystem>?> GetSystems()
        {
            List<MetabaseSystem> systemsList;
            try
            {
                await ExecuteSqlQuery();

                // Получение данных из View.
                systemsList = await _dbContext.SystemInfo.ToListAsync();
            } catch (Exception)
            {
                return null;
            }

            return systemsList;
        }

        private async Task ExecuteSqlQuery()
        {
            // Скрипт создает или обновляет View.
            string sqlScript;
            using (var reader = new StreamReader("DatabaseQuery.sql"))
            {
                sqlScript = await reader.ReadToEndAsync();
            }

            var quires = sqlScript.Split("GO").Where(str => str.Trim() != "");

            foreach (var query in quires)
            {
                await using var sqlConnection = new SqlConnection(Program.DbConnectionString);
                await using var command = sqlConnection.CreateCommand();
                command.CommandText = query;
                sqlConnection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}