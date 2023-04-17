namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIdsToOrderRecords : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRecords", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.OrderRecords", "CounselorId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRecords", "CounselorId");
            DropColumn("dbo.OrderRecords", "UserId");
        }
    }
}
