using EntityLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_for_metabase.Models;

namespace Service_for_metabase.Controllers
{
    public class MetabaseController : Controller
    {
        private static ICollection<string> _dbServicesUrls = new List<string>();
        private readonly MetabaseContext _metabaseContext;

        public MetabaseController(MetabaseContext metabaseContext)
        {
            _metabaseContext = metabaseContext;
        }

        /*
        public async Task<IActionResult> Index()
        {
            return View( (await _metabaseContext.PropertiesInfo.ToListAsync()).Distinct());
        }*/

        [Route("register")]
        public ActionResult Register([FromQuery(Name = "host")] string dbServiceUrl)
        {
            _dbServicesUrls.Add(dbServiceUrl);
            return Ok();
        }
        
        [Route("update")]
        public async Task<IActionResult> Update()
        {
            using var client = SpecialHttpClient.GetHttpClient();

            foreach (var dbServiceUrl in _dbServicesUrls)
            {
                var dbUrl = "https://" + dbServiceUrl + "/Database/properties";
                var dbProperties = await (await client.GetAsync(dbUrl))
                    .Content
                    .ReadFromJsonAsync<List<MetabaseProperty>>();

                var metabaseProperties = await _metabaseContext.PropertiesInfo.ToListAsync();

                var propsToAdd = dbProperties
                    ?.Where(prop => !metabaseProperties.Contains(prop));

                _metabaseContext.PropertiesInfo.AddRange(propsToAdd!);
                await _metabaseContext.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
