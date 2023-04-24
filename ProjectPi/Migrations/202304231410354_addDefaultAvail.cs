namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDefaultAvail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Timetables", "DefaultAvail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Timetables", "DefaultAvail");
        }
    }
}
