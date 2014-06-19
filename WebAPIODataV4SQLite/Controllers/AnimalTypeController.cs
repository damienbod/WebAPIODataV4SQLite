using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite.Controllers
{
    public class AnimalTypeController : ODataController
    {
        readonly SqliteContext _sqliteContext;

        public AnimalTypeController(SqliteContext sqliteContext)
        {
            _sqliteContext = sqliteContext;
        }

        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get()
        {
            return Ok(_sqliteContext.AnimalTypeEntities.AsQueryable());
        }

        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get([FromODataUri] int key)
        {
            return Ok(_sqliteContext.AnimalTypeEntities.Find(key));
        }

        [HttpGet]
        [ODataRoute("AnimalType({key})/EventData")]
        [EnableQuery(PageSize = 20)]      
        public IHttpActionResult GetEventData([FromODataUri] int key)
        {
            return Ok(_sqliteContext.AnimalTypeEntities.Find(key).EventDataValues);
        }

        [HttpPost]
        [ODataRoute("AnimalType")]
        public async Task<IHttpActionResult> CreateEventData(AnimalType animalType)
        {
            if (animalType != null && !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _sqliteContext.AnimalTypeEntities.Add(animalType);
            await _sqliteContext.SaveChangesAsync();

            return Created(animalType);
        }

        [HttpPut]
        [ODataRoute("AnimalType")]
        public async Task<IHttpActionResult> Put([FromODataUri] int key, AnimalType animalType)
        {
            if (animalType != null && !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_sqliteContext.AnimalTypeEntities.Any(t => t.Id == animalType.Id && t.Id == key))
            {
                return Content(HttpStatusCode.NotFound, "NotFound");
            }

            _sqliteContext.AnimalTypeEntities.AddOrUpdate(animalType);
            await _sqliteContext.SaveChangesAsync();

            return Updated(animalType);
        }

        [HttpPut]
        [ODataRoute("AnimalType")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<AnimalType> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_sqliteContext.AnimalTypeEntities.Any(t => t.Id == key))
            {
                return Content(HttpStatusCode.NotFound, "NotFound");
            }

            var animalType = _sqliteContext.AnimalTypeEntities.Single(t => t.Id == key);
            delta.Patch(animalType);
            await _sqliteContext.SaveChangesAsync();

            return Updated(animalType);
        }

        [HttpDelete]
        [ODataRoute("AnimalType")]
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var entity = _sqliteContext.AnimalTypeEntities.FirstOrDefault(t => t.Id == key);

            if (entity == null)
            {
                return Content(HttpStatusCode.NotFound, "NotFound");
            }

            if (entity.EventDataValues.Any())
            {
                return BadRequest("The entity has child EventData entites still, remove these first");
            }

            _sqliteContext.AnimalTypeEntities.Remove(entity);
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
