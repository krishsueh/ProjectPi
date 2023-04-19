namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAppointmentTimeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "AppointmentTimeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Appointments", "AppointmentTimeId");
        }
    }
}
