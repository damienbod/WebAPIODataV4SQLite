using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData.Batch;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite.App_Start
{
	public class ODataBatchHandlerSingleTransaction : DefaultODataBatchHandler
	{
		public ODataBatchHandlerSingleTransaction(HttpServer httpServer): base(httpServer)
        {
        }

		public async override Task<IList<ODataBatchResponseItem>> ExecuteRequestMessagesAsync(
		   IEnumerable<ODataBatchRequestItem> requests,
		   CancellationToken cancellation)
		{
			if (requests == null)
			{
				throw new ArgumentNullException("requests");
			}

			IList<ODataBatchResponseItem> responses = new List<ODataBatchResponseItem>();
			try
			{
				foreach (ODataBatchRequestItem request in requests)
				{
					var operation = request as OperationRequestItem;
					if (operation != null)
					{
						responses.Add(await request.SendRequestAsync(Invoker, cancellation));
					}
					else
					{
						await ExecuteChangeSet((ChangeSetRequestItem)request, responses, cancellation);
					}
				}
			}
			catch
			{
				foreach (ODataBatchResponseItem response in responses)
				{
					if (response != null)
					{
						response.Dispose();
					}
				}
				throw;
			}

			return responses;
		}

		private async Task ExecuteChangeSet(ChangeSetRequestItem changeSet, IList<ODataBatchResponseItem> responses, CancellationToken cancellation)
		{
			ChangeSetResponseItem changeSetResponse;

			using (var context = new SqliteContext())
			{
				foreach (HttpRequestMessage request in changeSet.Requests)
				{
					request.SetContext(context);
				}

				using (DbContextTransaction transaction = context.Database.BeginTransaction())
				{
					changeSetResponse = (ChangeSetResponseItem)await changeSet.SendRequestAsync(Invoker, cancellation);
					responses.Add(changeSetResponse);

					if (changeSetResponse.Responses.All(r => r.IsSuccessStatusCode))
					{
						transaction.Commit();
					}
					else
					{
						transaction.Rollback();
					}
				}
			}
		}
	}
}