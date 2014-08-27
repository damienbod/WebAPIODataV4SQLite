using System;
using System.Linq;
using Microsoft.OData.Client;
using WebAPIODataV4.Client.Damienbod;
using WebAPIODataV4.Client.WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4.Client
{
    class Program
    {
        static void Main(string[] args)
        {
			var context = new SqliteContext(new Uri("http://localhost.fiddler:59145/odata/"));
            context.Format.UseJson();

			// Call some basic Get
            var eventDataItems = context.EventData.ToList();
            var animalsItems = context.AnimalType.ToList();
	        var skillLevels = context.SkillLevels.Expand("Levels").GetValue();
	        var players = context.Player.Expand(c => c.PlayerStats).Where(u => u.PlayerStats.SkillLevel == 2).ToList();

            // Create a new entity
            var newObjectEventData = new EventData
            {
                AnimalTypeId = animalsItems.First().Key,
                Factor = 55,
                FixChange = 55.0,
                StringTestId = "_ok_testdatafromodataclient",
                AnimalType = animalsItems.First()
            };

			var secondNewObjectEventData = new EventData
			{
				AnimalTypeId = 200,
				Factor = 44,
				FixChange = 44.0,
				StringTestId = "_nok_testdatafromodataclient",
			};

			// Update a new entity
            var dataToUpdate = eventDataItems.FirstOrDefault();
            dataToUpdate.Factor = -1;
            dataToUpdate.FixChange = 97;

			context.AddToEventData(newObjectEventData);
			context.UpdateObject(dataToUpdate);
			context.AddToEventData(secondNewObjectEventData);

			context.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;

			// Add the data to the server
			DataServiceResponse response = context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

            foreach (OperationResponse individualResponse in response)
            {
                Console.WriteLine(" status code = {0}", individualResponse.StatusCode);
            }

            Console.ReadLine();
        }
    }
}
