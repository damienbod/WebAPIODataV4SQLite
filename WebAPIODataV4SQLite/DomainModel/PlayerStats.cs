using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIODataV4SQLite.DomainModel
{
    public class PlayerStats
    {
        public long PlayerStatsId { get; set; }
        public int SkillLevel { get; set; }
        public string HighScore { get; set; }

        [Key, ForeignKey("Player")]
        public long PlayerId { get; set; }
        public virtual Player Player { get; set; }
    }
}
