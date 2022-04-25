using EntityLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Service_for_database.Models;

namespace Service_for_database.Controllers
{
    [Route("[controller]")]
    public class DatabaseController : Controller
    {
        private readonly DatabaseContext _dbContext;

        public DatabaseController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [Route("register")]
        public async Task<HttpResponseMessage> Register()
        {
            using var client = SpecialHttpClient.GetHttpClient();
		
            return await client.PostAsync($"{Program.MetabaseHost}/register?host={Request.Host}", new StringContent("noContent"));
        }
        
        [HttpGet("properties")]
        public async Task<ICollection<MetabaseProperty>> SqlQuery()
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

            // Получение данных из View.
            var propertiesList = await _dbContext.MetaProperties.ToListAsync();
            return propertiesList;
        }
        
        [HttpGet("show")]
        public async Task<IActionResult> DatabaseProperties()
        {
            return null;
        }
    }
}