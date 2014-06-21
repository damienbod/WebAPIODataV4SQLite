using System.ComponentModel.DataAnnotations;
using System.Web.OData.Builder;

namespace WebAPIODataV4SQLite.DomainModel
{
    public class Player
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [Contained]
        public virtual PlayerStats PlayerStats { get; set; }
    }
}

/*
CREATE TABLE "Player"
(
 "Id" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL  UNIQUE ,
 "Name" TEXT NOT NULL  DEFAULT Unknown,
 "Email" TEXT NOT NULL  DEFAULT Unknown
)

CREATE TABLE "PlayerStats"
(
 "PlayerStatsId" INTEGER PRIMARY KEY  NOT NULL ,
 "SkillLevel" int NOT NULL ,
 "HighScore" NVARCHAR(160) NOT NULL ,
 "PlayerId" INTEGER NOT NULL ,
 FOREIGN KEY ([PlayerId]) REFERENCES [Player] ([Id])
   ON DELETE NO ACTION ON UPDATE NO ACTION
)

http://localhost:59145/odata/Player(1)/PlayerStats

http://localhost:59145/odata/Player%281%29?$expand=PlayerStats
*/