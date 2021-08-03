using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Administration;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        Models.Site _oSite = new Models.Site();
        List<Models.Site> _oSites = new List<Models.Site>();

        [HttpGet]
        public async Task<List<Models.Site>> GetSite()
        {
            _oSites = new List<Models.Site>();
            ServerManager iisManager = new ServerManager();
            int id = 1;
            foreach (var site in iisManager.Sites)
            {
                _oSite = new Models.Site();
                _oSite.Id = id;
                id = id + 1;
                _oSite.Name = site.Name;
                _oSites.Add(_oSite);

            }
                return _oSites;
        }
    }
}
