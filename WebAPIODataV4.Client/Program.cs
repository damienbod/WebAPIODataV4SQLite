using System;
using System.Linq;
using Microsoft.OData.Client;
using WebAPIODataV4.Client.Default;
using WebAPIODataV4.Client.WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new DamienbodContext(new Uri("http://localhost:59145/odatabatching/"));
            context.Format.UseJson();

            var eventDataItems = context.EventData.ToList();
            var animalsItems = context.AnimalType.ToList();

            // Some test data for the batch
            var newObjectEventData = new EventData
            {
                AnimalTypeId = animalsItems.First().Key,
                Factor = 56,
                FixChange = 14.0,
                StringTestId = "batchtestdatafromodataclient"
            };
            newObjectEventData.AnimalType = animalsItems.First();

            var dataToUpdate = eventDataItems.FirstOrDefault();
            dataToUpdate.Factor = 99;
            dataToUpdate.FixChange = 99;

            context.AddToEventData(newObjectEventData);
            context.UpdateObject(dataToUpdate);

            DataServiceResponse response = context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
            if (!response.IsBatchResponse)
            {
                Console.WriteLine("There was an error with the batch request");
            }

            int i = 0;
            foreach (OperationResponse individualResponse in response)
            {
                Console.WriteLine("Operation {0} status code = {1}", i++, individualResponse.StatusCode);
            }

            Console.ReadLine();
        }
    }
}
