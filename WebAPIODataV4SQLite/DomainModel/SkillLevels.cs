using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.OData.Builder;

namespace WebAPIODataV4SQLite.DomainModel
{
    public class SkillLevels
    {
        [Key]
        public int Id { get; set; }

        [Contained]
        public List<SkillLevel> Levels { get; set; }
    }

    public class SkillLevel
    {
        [Key]
        public int Level { get; set; }

        public string Description { get; set; }

        [Contained]
        public virtual PlayerStats PlayerStats { get; set; }
    }
}