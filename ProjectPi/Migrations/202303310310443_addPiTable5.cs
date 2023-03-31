namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPiTable5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Counselors", "LicenseBase64", c => c.String(nullable: false));
            AddColumn("dbo.Counselors", "PhotoBase64", c => c.String());
            AlterColumn("dbo.Products", "Availability", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Counselors", "IsVideoOpen", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Counselors", "IsVideoOpen", c => c.String(maxLength: 50));
            AlterColumn("dbo.Products", "Availability", c => c.String(maxLength: 50));
            DropColumn("dbo.Counselors", "PhotoBase64");
            DropColumn("dbo.Counselors", "LicenseBase64");
        }
    }
}
