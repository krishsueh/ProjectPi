namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPiTable4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "InitDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.OrderRecords", "InitDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Carts", "InitDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "Guid", c => c.Guid(nullable: false));
            AddColumn("dbo.Users", "InitDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Products", "InitDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Counselors", "Guid", c => c.Guid(nullable: false));
            AddColumn("dbo.Counselors", "InitDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Features", "InitDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.MainFields", "InitDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ChatRooms", "InitDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ChatRooms", "InitDate", c => c.DateTime());
            DropColumn("dbo.MainFields", "InitDate");
            DropColumn("dbo.Features", "InitDate");
            DropColumn("dbo.Counselors", "InitDate");
            DropColumn("dbo.Counselors", "Guid");
            DropColumn("dbo.Products", "InitDate");
            DropColumn("dbo.Users", "InitDate");
            DropColumn("dbo.Users", "Guid");
            DropColumn("dbo.Carts", "InitDate");
            DropColumn("dbo.OrderRecords", "InitDate");
            DropColumn("dbo.Appointments", "InitDate");
        }
    }
}
