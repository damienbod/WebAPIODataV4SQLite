using System.Collections.Generic;

namespace WebAPIODataV4SQLite.DomainModel
{
    public class AnimalType
    {
        public AnimalType()
        {
            EventDataValues = new List<EventData>();
        }
 
        public long Id { get; set; }
        public string Name { get; set; }
        public double MeanCost { get; set; }
        
        public virtual ICollection<EventData> EventDataValues { get; set; }
    }
}
