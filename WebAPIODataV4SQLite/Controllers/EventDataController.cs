using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.OData;
using System.Web.OData.Routing;
using Microsoft.Ajax.Utilities;
using WebApiContrib.Tracing.Slab;
using WebAPIODataV4SQLite.App_Start;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite.Controllers
{
	[SlabLoggingFilterAttribute]
    [ODataRoutePrefix("EventData")]
    public class EventDataController : ODataController
    {

		[SlabLoggingFilter]
        [EnableQuery(PageSize = 20)]		
        public IHttpActionResult Get()
        {
			// We want to force xml for the odata feed
			//Request.Headers.Accept.Clear();
			//Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            return Ok( Request.GetContext().EventDataEntities.AsQueryable());
        }

        [HttpPost]
        [ODataRoute("")]
        public async Task<IHttpActionResult> Post(EventData eventData)
        {
            if (eventData !=null && !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			Request.GetContext().EventDataEntities.Add(eventData);
			await Request.GetContext().SaveChangesAsync();

            return Created(eventData);
        }

        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get([FromODataUri] int key)
        {
			return Ok(Request.GetContext().EventDataEntities.Find(key));
        }

        [HttpPut]
        [ODataRoute("")]
        public async Task<IHttpActionResult> Put([FromODataUri] int key, EventData eventData)
        {
            if (eventData != null && !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			if (!Request.GetContext().EventDataEntities.Any(t => t.EventDataId == eventData.EventDataId && t.EventDataId == key))
            {
                return Content(HttpStatusCode.NotFound, "NotFound");
            }

			Request.GetContext().EventDataEntities.AddOrUpdate(eventData);
			await Request.GetContext().SaveChangesAsync();

            return Updated(eventData);
        }

        [HttpPatch]
        [ODataRoute("")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<EventData> delta)
        {
	        //return BadRequest("TESTING BATCHING ROLLBACK");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			if (!Request.GetContext().EventDataEntities.Any(t => t.EventDataId == key))
            {
                return Content(HttpStatusCode.NotFound, "NotFound");
            }

			var eventData = Request.GetContext().EventDataEntities.Single(t => t.EventDataId == key);
            delta.Patch(eventData);
			await Request.GetContext().SaveChangesAsync();

            return Updated(eventData);
        }

        [HttpDelete]
        [ODataRoute("")]
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
			var entity = Request.GetContext().EventDataEntities.FirstOrDefault(t => t.EventDataId == key);
            if (entity == null)
            {
                return Content(HttpStatusCode.NotFound, "NotFound");
            }

			Request.GetContext().EventDataEntities.Remove(entity);
			await Request.GetContext().SaveChangesAsync();

            return Content(HttpStatusCode.NoContent, "Deleted");
        }

        protected override void Dispose(bool disposing)
        {
			Request.GetContext().Dispose();
            base.Dispose(disposing);
        }
    }
}
