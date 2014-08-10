using System;
using System.Runtime.Serialization;

namespace WebAPIODataV4SQLite.DomainModel
{
	[DataContract(Name = "EventData")]
    public class EventData
    {
		[DataMember]
        public long EventDataId { get; set; }
		[DataMember]
        public int Factor { get; set; }
		[DataMember]
        public string StringTestId { get; set; }
		[DataMember]
        public double FixChange { get; set; }

		[DataMember]
        public long AnimalTypeId { get; set; }
		[DataMember]
        public virtual AnimalType AnimalType { get; set; }
    }

	public  class AdmDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public DateTimeOffset? AcceptDate { get; set; }
	}
}