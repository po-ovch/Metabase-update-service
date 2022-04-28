using EntityLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Service_for_metabase.Controllers
{
    public class MetabaseController : Controller
    {
        private static ICollection<string> _dbServicesUrls = new List<string>();
        private readonly DatabaseContext _metabaseContext;

        public MetabaseController(DatabaseContext metabaseContext)
        {
            _metabaseContext = metabaseContext;
        }

        [HttpPost("register")]
        public ActionResult Register([FromQuery(Name = "host")] string dbServiceUrl)
        {
            _dbServicesUrls.Add("https://" + dbServiceUrl);
            return Ok();
        }
        
        [HttpPut("update")]
        public async Task<IActionResult> Update()
        {
            using var client = SpecialHttpClient.GetHttpClient();

            foreach (var dbServiceUrl in _dbServicesUrls)
            {
                await UpdateProperties(client, dbServiceUrl);
                await UpdateSystems(client, dbServiceUrl);
            }

            return Ok();
        }

        private async Task UpdateProperties(HttpClient client, string dbServiceUrl)
        {
            var dbUrl =  dbServiceUrl + "/properties";
            var dbProperties = await (await client.GetAsync(dbUrl))
                .Content
                .ReadFromJsonAsync<List<MetabaseProperty>>();
            
            if (dbProperties is null)
            {
                throw new Exception();
            }
            
            var metabaseProperties = await _metabaseContext.PropertiesInfo.ToListAsync();
            var propsToAdd = dbProperties
                .Where(prop => !metabaseProperties.Contains(prop));
            _metabaseContext.PropertiesInfo.AddRange(propsToAdd);
            await _metabaseContext.SaveChangesAsync();
            
            var updMetabaseProperties = await _metabaseContext.PropertiesInfo.ToListAsync();
            var propsToEdit = dbProperties
                .Where(prop => !updMetabaseProperties.Contains(prop, new FullPropertyEqualityComparer()));
            foreach (var prop in propsToEdit)
            {
                var foundProp = updMetabaseProperties.Find(mProp =>
                    mProp.DBID == prop.DBID && mProp.PropId == prop.PropId);
                var modifyingProperty  = _metabaseContext.PropertiesInfo.Update(foundProp).Entity;
                modifyingProperty.Modify(prop);
            }
            await _metabaseContext.SaveChangesAsync();
        }
        
        private async Task UpdateSystems(HttpClient client, string dbServiceUrl)
        {
            var dbUrl =  dbServiceUrl + "/systems";
            var dbSystemsI = (await client.GetAsync(dbUrl))
                .Content;
            var dbSystems    = await dbSystemsI
                .ReadFromJsonAsync<List<MetabaseSystem>>();

            if (dbSystems is null)
            {
                throw new Exception();
            }
            
            var metabaseSystems = await _metabaseContext.SystemInfo.ToListAsync();
            var systemsToAdd = dbSystems
                .Where(sys => !metabaseSystems.Contains(sys));
            _metabaseContext.SystemInfo.AddRange(systemsToAdd);
            await _metabaseContext.SaveChangesAsync();
            
            var updMetabaseSystems = await _metabaseContext.SystemInfo.ToListAsync();
            var systemsToEdit = dbSystems
                .Where(sys => !updMetabaseSystems.Contains(sys, new FullSystemEqualityComparer()));
            foreach (var system in systemsToEdit)
            {
                var foundSys = updMetabaseSystems.Find(mProp =>
                    mProp.DBID == system.DBID && mProp.SystemId == system.SystemId);
                var modifyingSystem  = _metabaseContext.SystemInfo.Update(foundSys).Entity;
                modifyingSystem.Modify(system);
            }
            await _metabaseContext.SaveChangesAsync();
        }
    }
}
