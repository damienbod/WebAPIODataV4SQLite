using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using WebApiContrib.Tracing.Slab;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite.Controllers
{
	[SlabLoggingFilterAttribute]
    [ODataRoutePrefix("EventData")]
    public class EventDataController : ODataController
    {
        readonly SqliteContext _sqliteContext;

        public EventDataController(SqliteContext sqliteContext)
        {
            _sqliteContext = sqliteContext;
        }

		[SlabLoggingFilter]
        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get()
        {
            return Ok(_sqliteContext.EventDataEntities.AsQueryable());
        }

        [HttpPost]
        [ODataRoute("")]
        public async Task<IHttpActionResult> Post(EventData eventData)
        {
            if (eventData !=null && !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _sqliteContext.EventDataEntities.Add(eventData);
            await _sqliteContext.SaveChangesAsync();

            return Created(eventData);
        }

        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get([FromODataUri] int key)
        {
            return Ok(_sqliteContext.EventDataEntities.Find(key));
        }

        [HttpPut]
        [ODataRoute("")]
        public async Task<IHttpActionResult> Put([FromODataUri] int key, EventData eventData)
        {
            if (eventData != null && !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_sqliteContext.EventDataEntities.Any(t => t.EventDataId == eventData.EventDataId && t.EventDataId == key))
            {
                return Content(HttpStatusCode.NotFound, "NotFound");
            }

            _sqliteContext.EventDataEntities.AddOrUpdate(eventData);
            await _sqliteContext.SaveChangesAsync();

            return Updated(eventData);
        }

        [HttpPut]
        [ODataRoute("")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<EventData> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_sqliteContext.EventDataEntities.Any(t =>  t.EventDataId == key))
            {
                return Content(HttpStatusCode.NotFound, "NotFound");
            }

            var eventData = _sqliteContext.EventDataEntities.Single(t => t.EventDataId == key);
            delta.Patch(eventData);
            await _sqliteContext.SaveChangesAsync();

            return Updated(eventData);
        }


        [HttpDelete]
        [ODataRoute("")]
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var entity = _sqliteContext.EventDataEntities.FirstOrDefault(t => t.EventDataId == key);
            if (entity == null)
            {
                return Content(HttpStatusCode.NotFound, "NotFound");
            }

            _sqliteContext.EventDataEntities.Remove(entity);
            await _sqliteContext.SaveChangesAsync();

            return Content(HttpStatusCode.NoContent, "Deleted");
        }

        protected override void Dispose(bool disposing)
        {
            _sqliteContext.Dispose();
            base.Dispose(disposing);
        }
    }
}
