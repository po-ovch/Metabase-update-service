using EntityLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Service_for_database.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly DatabaseContext _dbContext;

        public DatabaseController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpPost("register")]
        public async Task<HttpResponseMessage> Register()
        {
            using var client = SpecialHttpClient.GetHttpClient();
		
            return await client.PostAsync($"{Program.MetabaseHost}/register?host={Request.Host}", new StringContent("noContent"));
        }

        [HttpGet("properties")]
        public async Task<ICollection<MetabaseProperty>> GetProperties()
        {
            await ExecuteSqlQuery();

            // Получение данных из View.
            var propertiesList = await _dbContext.PropertiesInfo.ToListAsync();
            return propertiesList;
        }
        
        [HttpGet("systems")]
        public async Task<ICollection<MetabaseSystem>> GetSystems()
        {
            await ExecuteSqlQuery();

            // Получение данных из View.
            var propertiesList = await _dbContext.SystemInfo.ToListAsync();
            return propertiesList;
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
        
        [HttpGet("show")]
        public async Task<IActionResult> DatabaseProperties()
        {
            return null;
        }
    }
}