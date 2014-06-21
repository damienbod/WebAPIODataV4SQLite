using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite.Controllers
{
    public class PlayerController : ODataController
    {
        readonly SqliteContext _sqliteContext;

        public PlayerController(SqliteContext sqliteContext)
        {
            _sqliteContext = sqliteContext;
        }

        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get()
        {
            return Ok(_sqliteContext.PlayerEntities.AsQueryable());
        }

        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get([FromODataUri] int key)
        {
            return Ok(_sqliteContext.PlayerEntities.Find(key));
        }

        [EnableQuery(PageSize = 20)]
        [ODataRoute("Player({key})/PlayerStats")]
        public IHttpActionResult GetPlayserStats([FromODataUri] int key)
        {
            return Ok(_sqliteContext.PlayerEntities.Find(key).PlayerStats);
        }
    }
}
