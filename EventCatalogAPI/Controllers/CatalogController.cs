using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventCatalogAPI.Data;
using EventCatalogAPI.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EventCatalogAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly IConfiguration _configuration;

        public CatalogController(CatalogContext catalogContext, 
            IConfiguration Configuration)
        {
            _catalogContext = catalogContext;
            _configuration = Configuration;
        }

//Get for Catagories

        [HttpGet]
        [Route("[action]")]

        public async Task<IActionResult> catagories()
        {
           var Events =await _catalogContext.Catagories.ToListAsync();
            return Ok(Events);
        }

// Get for subcatagories
        [HttpGet]
        [Route("[action]")]

        public async Task<IActionResult> subcatagories()
        {
            var Events = await _catalogContext.SubCatagories.ToListAsync();
            return Ok(Events);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Events(
             [FromQuery] int pagesize = 5,
             [FromQuery]int pageIndex = 0
            )
        {
            var totalEvents = await
                _catalogContext.Events.LongCountAsync();

            var eventsOnPage = await _catalogContext.Events
                 .OrderBy(c => c.EventName)
                 .Skip(pageIndex * pagesize)
                 .Take(pagesize)
                 .ToListAsync();

            eventsOnPage = ChangeUrlPlaceholder(eventsOnPage);
            return Ok(eventsOnPage);
        }

        [HttpGet]
        [Route("[action]/EventName/{EventName:minlength(1)}")]
        public async Task<IActionResult> Events(
            string EventName,
             [FromQuery] int pagesize = 5,
             [FromQuery]int pageIndex = 0
            )
        {
            var totalEvents = await
                _catalogContext.Events.LongCountAsync();

            var eventsOnPage = await _catalogContext.Events
                .Where(c => c.EventName.StartsWith(EventName))
                 .OrderBy(c => c.EventName)
                 .Skip(pageIndex * pagesize)
                 .Take(pagesize)
                 .ToListAsync();

            eventsOnPage = ChangeUrlPlaceholder(eventsOnPage);
            return Ok(eventsOnPage);
        }

      
        [HttpGet]
        [Route("Events/{id:int}")]

     public async Task<IActionResult> GetEventBtId(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            // this method return one item
            var Events = await _catalogContext.Events
                  .SingleOrDefaultAsync(c => c.EventID == id);
            if (Events != null)
            {
                Events.EventImageUrl = Events.EventImageUrl
                    .Replace("http://externalcatalogbaseurltobereplaced",
                _configuration["ExternalCatalogBaseurl"]);
                return Ok(Events);
            }
            return NotFound();
        }

        private List<Event> ChangeUrlPlaceholder(
            List<Event> Events)
        {
            Events.ForEach(
                x => x.EventImageUrl =
                x.EventImageUrl.Replace("http://externalcatalogbaseurltobereplaced",
                _configuration["ExternalCatalogBaseurl"]));
               return Events;
        }

    }
}