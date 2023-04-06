namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBase64Counselor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Counselors", "LicenseBase64", c => c.String());
            AddColumn("dbo.Counselors", "PhotoBase64", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Counselors", "PhotoBase64");
            DropColumn("dbo.Counselors", "LicenseBase64");
        }
    }
}
