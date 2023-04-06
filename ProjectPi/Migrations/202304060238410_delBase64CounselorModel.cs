namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delBase64CounselorModel : DbMigration
    {
        public override void Up()
        {
       
        }
        
        public override void Down()
        {
            AddColumn("dbo.Counselors", "PhotoBase64", c => c.String());
            AddColumn("dbo.Counselors", "LicenseBase64", c => c.String(nullable: false));
        }
    }
}
