namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTimetableModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Timetables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CounselorId = c.Int(nullable: false),
                        WeekDay = c.String(maxLength: 10),
                        Date = c.DateTime(nullable: false),
                        Time = c.String(maxLength: 5),
                        Availability = c.Boolean(nullable: false),
                        InitDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Counselors", t => t.CounselorId, cascadeDelete: true)
                .Index(t => t.CounselorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Timetables", "CounselorId", "dbo.Counselors");
            DropIndex("dbo.Timetables", new[] { "CounselorId" });
            DropTable("dbo.Timetables");
        }
    }
}
