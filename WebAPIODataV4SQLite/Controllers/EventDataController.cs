using System.Linq;
using System.Web.Http;
using System.Web.OData;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite.Controllers
{
    public class EventDataController : ODataController
    {
        readonly SqliteContext _sqliteContext;

        public EventDataController(SqliteContext sqliteContext)
        {
            _sqliteContext = sqliteContext;
        }

        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get()
        {
            return Ok(_sqliteContext.EventDataEntities.AsQueryable());
        }

        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get([FromODataUri] int key)
        {
            return Ok(_sqliteContext.EventDataEntities.Find(key));
        }

        protected override void Dispose(bool disposing)
        {
            _sqliteContext.Dispose();
            base.Dispose(disposing);
        }
    }
}
