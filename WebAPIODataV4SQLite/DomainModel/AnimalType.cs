using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebAPIODataV4SQLite.DomainModel
{
    [DataContract(Name = "AnimalType")]
    public class AnimalType
    {
        public AnimalType()
        {
            EventDataValues = new List<EventData>();
        }

        [DataMember(Name = "Key")]
        [Key]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double MeanCost { get; set; }

        [DataMember(Name = "EventData")]
        public virtual ICollection<EventData> EventDataValues { get; set; }
    }
}
