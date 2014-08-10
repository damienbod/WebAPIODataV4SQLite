using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite.Controllers
{
	public class AdmsController : ODataController
	{
		private readonly AdmDto _adm = new AdmDto();
		readonly SqliteContext _sqliteContext;

		public AdmsController(SqliteContext sqliteContext)
		{
			_sqliteContext = sqliteContext;
			_adm.AcceptDate = new DateTimeOffset(DateTime.UtcNow);
			_adm.Id = 1;
			_adm.Name = "damienbod";
		}

		[EnableQuery(PageSize = 20)]
		public IHttpActionResult Get()
		{
			return Ok(new List<AdmDto>{_adm});
		}

		[EnableQuery(PageSize = 20)]
		public IHttpActionResult Get([FromODataUri] int key)
		{
			//if (Request.Headers.Accept.Count == 0)
			//	Request.Headers.Add("Accept", "application/atom+xml");

			return Ok(_adm);
		}

		protected override void Dispose(bool disposing)
		{
			_sqliteContext.Dispose();
			base.Dispose(disposing);
		}
	}
}
