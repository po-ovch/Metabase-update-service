using System.Net.Http.Headers;
using EntityLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_for_metabase.Model;

namespace Service_for_metabase.Controllers
{
    [Authorize]
    public class MetabaseController : Controller
    {
        private readonly MetabaseContext _metabaseContext;

        public MetabaseController(MetabaseContext metabaseContext)
        {
            _metabaseContext = metabaseContext;
        }
        
        [HttpGet("update")]
        public async Task<IActionResult> Update()
        {
            var updatedProperties = new List<MetabaseProperty>();
            var updatedSystems = new List<MetabaseSystem>();
            
            using var client = SpecialHttpClient.GetHttpClient();
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", TokenGenerator.GetToken(User.Claims));

            var dbServicesUrls = (await _metabaseContext.DBInfo.ToListAsync())
                .Select(info => info.DBServiceHost)
                .Where(host => host is not null);
            foreach (var dbServiceUrl in dbServicesUrls)
            {
                updatedProperties.AddRange(await UpdateProperties(client, dbServiceUrl!));
                updatedSystems.AddRange(await UpdateSystems(client, dbServiceUrl!));
            }

            ViewBag.PropertiesTableModel = TableModel<MetabaseProperty>.BuildModel(updatedProperties);
            ViewBag.SystemsTableModel = TableModel<MetabaseSystem>.BuildModel(updatedSystems);

            return View("ShowUpdate");
        }

        [HttpGet("/properties")]
        public async Task<IActionResult> GetProperties()
        {
            var properties = await _metabaseContext.PropertiesInfo.ToListAsync();
            ViewBag.TableModel = TableModel<MetabaseProperty>.BuildModel(properties);
            return View("ShowInfo");
        }
        
        [HttpGet("/systems")]
        public async Task<IActionResult> GetSystems()
        {
            var systems = await _metabaseContext.SystemInfo.ToListAsync();
            ViewBag.TableModel = TableModel<MetabaseSystem>.BuildModel(systems);
            return View("ShowInfo");
        }

        private async Task<List<MetabaseProperty>> UpdateProperties(HttpClient client, string dbServiceUrl)
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
                var modifyingProperty  = _metabaseContext.PropertiesInfo.Update(foundProp!).Entity;
                modifyingProperty.Modify(prop);
            }
            await _metabaseContext.SaveChangesAsync();

            var newProperties = new List<MetabaseProperty>();
            newProperties.AddRange(propsToAdd);
            newProperties.AddRange(propsToEdit);
            return newProperties;
        }
        
        private async Task<List<MetabaseSystem>> UpdateSystems(HttpClient client, string dbServiceUrl)
        {
            var dbUrl =  dbServiceUrl + "/systems";
            var dbSystems = await (await client.GetAsync(dbUrl))
                .Content
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
                var modifyingSystem  = _metabaseContext.SystemInfo.Update(foundSys!).Entity;
                modifyingSystem.Modify(system);
            }
            await _metabaseContext.SaveChangesAsync();

            var newSystems = new List<MetabaseSystem>();
            newSystems.AddRange(systemsToAdd);
            newSystems.AddRange(systemsToEdit);
            return newSystems;
        }
    }
}
