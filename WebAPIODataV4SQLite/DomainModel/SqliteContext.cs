﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WebAPIODataV4SQLite.DomainModel
{
    public class SqliteContext : DbContext
    {
        public SqliteContext()
        {
			// Required for XML serialization
	        //this.Configuration.ProxyCreationEnabled = false;

            // Turn off the Migrations, (NOT a code first Db)
            Database.SetInitializer<SqliteContext>(null);
        }

        public DbSet<EventData> EventDataEntities { get; set; }
        public DbSet<AnimalType> AnimalTypeEntities { get; set; }

        public DbSet<PlayerStats> PlayerStatsEntities { get; set; }
        public DbSet<Player> PlayerEntities { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Database does not pluralize table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}
