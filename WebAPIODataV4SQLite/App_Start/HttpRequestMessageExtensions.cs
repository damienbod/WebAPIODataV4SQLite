using System.Net.Http;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite.App_Start
{
	public static class HttpRequestMessageExtensions
	{
		private const string DbContext = "Batch_DbContext";

		public static void SetContext(this HttpRequestMessage request,SqliteContext context)
		{
			request.Properties[DbContext] = context;
		}

		
		public static SqliteContext GetContext(this HttpRequestMessage request)
		{
			object sqliteContext;
			if (request.Properties.TryGetValue(DbContext, out sqliteContext))
			{
				return (SqliteContext)sqliteContext;
			}
			else
			{
				var context = new SqliteContext();
				SetContext(request, context);

				request.RegisterForDispose(context);
				return context;
			}
		}
	}
}