﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite.Controllers
{

    public class SkillLevelsController : ODataController
    {
        private readonly SqliteContext _sqliteContext;

        public SkillLevelsController(SqliteContext sqliteContext)
        {
            _sqliteContext = sqliteContext;
        }

        [EnableQuery(PageSize = 20)]
        public IHttpActionResult Get()
        {
            return Ok(GetFixedSkillLevels());
        }

        [EnableQuery(PageSize = 20)]
        [ODataRoute("SkillLevels/Levels({key})")]
        public IHttpActionResult GetPlayerStats([FromODataUri] int key)
        {
            return Ok(GetFixedSkillLevels().Levels.FirstOrDefault(t => t.Level == key));
        }
        
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        [ODataRoute("SkillLevels/Levels({key})/PlayerStats")]
        public IHttpActionResult GetPlayserStats([FromODataUri] int key)
        {
            return Ok(_sqliteContext.PlayerStatsEntities.Where(t => t.SkillLevel == key));
        }

        private SkillLevels GetFixedSkillLevels()
        {
            return new SkillLevels
            {
                Id =1,
                Levels = new List<SkillLevel>
                {
                    new SkillLevel {Description = "Legend", Level = 1},
                    new SkillLevel {Description = "Master", Level = 2},
                    new SkillLevel {Description = "Senior", Level = 3},
                    new SkillLevel {Description = "Intermediate", Level = 4},
                    new SkillLevel {Description = "Junior", Level = 5},
                    new SkillLevel {Description = "Novice", Level = 6}
                }
            };
        }
    }
}
