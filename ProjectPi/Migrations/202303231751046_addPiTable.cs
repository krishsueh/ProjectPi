namespace ProjectPi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPiTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ReserveStatus = c.String(maxLength: 50),
                        AppointmentTime = c.DateTime(),
                        CounsellingRecord = c.String(),
                        RecordDate = c.DateTime(),
                        Star = c.Int(),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderRecords", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.OrderRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNum = c.String(maxLength: 50),
                        OrderDate = c.DateTime(nullable: false),
                        UserName = c.String(maxLength: 50),
                        CounselorName = c.String(maxLength: 50),
                        Field = c.String(maxLength: 50),
                        Item = c.String(maxLength: 50),
                        Quantity = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                        OrderStatus = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UersId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UersId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.UersId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Sex = c.String(nullable: false, maxLength: 50),
                        BirthDate = c.DateTime(nullable: false),
                        Account = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CounselorId = c.Int(nullable: false),
                        FieldId = c.Int(nullable: false),
                        Item = c.String(maxLength: 50),
                        Quantity = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                        Availability = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MainFields", t => t.FieldId, cascadeDelete: true)
                .ForeignKey("dbo.Counselors", t => t.CounselorId, cascadeDelete: true)
                .Index(t => t.CounselorId)
                .Index(t => t.FieldId);
            
            CreateTable(
                "dbo.Counselors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Account = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 100),
                        CertNumber = c.String(nullable: false, maxLength: 50),
                        LicenseImg = c.String(nullable: false, maxLength: 50),
                        Photo = c.String(maxLength: 50),
                        SellingPoint = c.String(maxLength: 50),
                        SelfIntroduction = c.String(maxLength: 300),
                        VideoLink = c.String(maxLength: 100),
                        IsVideoOpen = c.String(maxLength: 50),
                        Validation = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Features",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CounselorId = c.Int(nullable: false),
                        FieldId = c.Int(nullable: false),
                        Feature1 = c.String(nullable: false, maxLength: 25),
                        Feature2 = c.String(nullable: false, maxLength: 25),
                        Feature3 = c.String(nullable: false, maxLength: 25),
                        Feature4 = c.String(maxLength: 25),
                        Feature5 = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Counselors", t => t.CounselorId, cascadeDelete: true)
                .ForeignKey("dbo.MainFields", t => t.FieldId, cascadeDelete: true)
                .Index(t => t.CounselorId)
                .Index(t => t.FieldId);
            
            CreateTable(
                "dbo.MainFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Field = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChatRooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CounselorId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Type = c.String(maxLength: 10),
                        Content = c.String(),
                        InitDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "CounselorId", "dbo.Counselors");
            DropForeignKey("dbo.Products", "FieldId", "dbo.MainFields");
            DropForeignKey("dbo.Features", "FieldId", "dbo.MainFields");
            DropForeignKey("dbo.Features", "CounselorId", "dbo.Counselors");
            DropForeignKey("dbo.Carts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Carts", "UersId", "dbo.Users");
            DropForeignKey("dbo.Appointments", "OrderId", "dbo.OrderRecords");
            DropIndex("dbo.Features", new[] { "FieldId" });
            DropIndex("dbo.Features", new[] { "CounselorId" });
            DropIndex("dbo.Products", new[] { "FieldId" });
            DropIndex("dbo.Products", new[] { "CounselorId" });
            DropIndex("dbo.Carts", new[] { "ProductId" });
            DropIndex("dbo.Carts", new[] { "UersId" });
            DropIndex("dbo.Appointments", new[] { "OrderId" });
            DropTable("dbo.ChatRooms");
            DropTable("dbo.MainFields");
            DropTable("dbo.Features");
            DropTable("dbo.Counselors");
            DropTable("dbo.Products");
            DropTable("dbo.Users");
            DropTable("dbo.Carts");
            DropTable("dbo.OrderRecords");
            DropTable("dbo.Appointments");
        }
    }
}
