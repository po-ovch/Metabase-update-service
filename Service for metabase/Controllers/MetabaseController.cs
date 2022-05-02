using System.Net;
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
        
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet("update")]
        public async Task<IActionResult> Update()
        {
            var updatedProperties = new List<MetabaseProperty>();
            var updatedSystems = new List<MetabaseSystem>();
            var unsuccessfulUpdatesUrls = new List<string>();
            List<MetabaseDb> databases;

            using var client = SpecialHttpClient.GetHttpClient();
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", TokenGenerator.GetToken(User.Claims));
            
            try
            {
                databases = await _metabaseContext.DBInfo.ToListAsync();
            }
            catch
            {
                return NotFound();
            }

            var dbServicesUrls = databases
                .Select(info => info.DBServiceHost)
                .Where(host => host is not null);
            foreach (var dbServiceUrl in dbServicesUrls)
            {
                try
                {
                    updatedProperties.AddRange(await UpdateProperties(client, dbServiceUrl!));
                    updatedSystems.AddRange(await UpdateSystems(client, dbServiceUrl!));
                }
                catch (Exception)
                {
                    unsuccessfulUpdatesUrls.Add(dbServiceUrl!);
                }
            }

            ViewBag.PropertiesTableModel = TableModel<MetabaseProperty>.BuildModel(updatedProperties);
            ViewBag.SystemsTableModel = TableModel<MetabaseSystem>.BuildModel(updatedSystems);
            ViewBag.UnsuccessfulUpdates = databases
                .Where(db => unsuccessfulUpdatesUrls.Contains(db.DBServiceHost!))
                .Select(db => db.Name);

            return View("ShowUpdate");
        }

        [HttpGet("/activeServices")]
        public async Task<IActionResult> GetActivatedServices()
        {
            var activeServices = new List<string>();
            var databases = await _metabaseContext.DBInfo.ToListAsync();
            const string checkEndpoint = "/check";
            
            using var client = SpecialHttpClient.GetHttpClient();
            var servicesUrls = databases
                .Select(db => db.DBServiceHost)
                .Where(url => url != null);
            foreach (var serviceUrl in servicesUrls)
            {
                try {
                    var responseCode = (await client.GetAsync(serviceUrl + checkEndpoint)).StatusCode;
                    if (responseCode is HttpStatusCode.OK)
                    {
                        activeServices.Add(serviceUrl);
                    }
                }
                catch
                {
                    // ignored
                }
            }

            ViewBag.TableModel = TableModel<DatabaseModel>.BuildModel(databases
                .Where(db => activeServices.Contains(db.DBServiceHost))
                .Select(DatabaseModel.FromMetabaseDb));
            return View("ShowActiveServices");
        }

        [HttpGet("/properties")]
        public async Task<IActionResult> GetProperties()
        {
            List<MetabaseProperty> properties;
            try
            {
                properties = await _metabaseContext.PropertiesInfo.ToListAsync();
            }
            catch
            {
                return NotFound();
            }

            ViewBag.TableModel = TableModel<MetabaseProperty>.BuildModel(properties);
            return View("ShowInfo");
        }
        
        [HttpGet("/systems")]
        public async Task<IActionResult> GetSystems()
        {
            List<MetabaseSystem> systems;
            try
            {
                systems = await _metabaseContext.SystemInfo.ToListAsync();
            }
            catch
            {
                return NotFound();
            }

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
