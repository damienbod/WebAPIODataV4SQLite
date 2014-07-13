using System;

namespace WebAPIODataV4SQLite.DomainModel
{
    public class EventData
    {
        public long EventDataId { get; set; }
        public int Factor { get; set; }
        public string StringTestId { get; set; }
        public double FixChange { get; set; }

        public long AnimalTypeId { get; set; }
        public virtual AnimalType AnimalType { get; set; }
    }

	public  class AdmDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public DateTimeOffset? AcceptDate { get; set; }
	}
}