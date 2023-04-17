namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class amendDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BackEndMangers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Sex = c.String(nullable: false, maxLength: 50),
                        BirthDate = c.DateTime(nullable: false),
                        Account = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 100),
                        AdminAccess = c.Int(nullable: false),
                        Guid = c.Guid(nullable: false),
                        InitDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Appointments", "ZoomLink", c => c.String());
            AddColumn("dbo.MainFields", "FieldImg", c => c.String());
            AddColumn("dbo.ChatRooms", "UserRead", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChatRooms", "CounselorRead", c => c.Boolean(nullable: false));
            DropColumn("dbo.Counselors", "LicenseBase64");
            DropColumn("dbo.Counselors", "PhotoBase64");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Counselors", "PhotoBase64", c => c.String());
            AddColumn("dbo.Counselors", "LicenseBase64", c => c.String());
            DropColumn("dbo.ChatRooms", "CounselorRead");
            DropColumn("dbo.ChatRooms", "UserRead");
            DropColumn("dbo.MainFields", "FieldImg");
            DropColumn("dbo.Appointments", "ZoomLink");
            DropTable("dbo.BackEndMangers");
        }
    }
}
