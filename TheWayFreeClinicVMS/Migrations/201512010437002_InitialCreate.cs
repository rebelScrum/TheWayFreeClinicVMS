namespace TheWayFreeClinicVMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Availability",
                c => new
                    {
                        avID = c.Int(nullable: false, identity: true),
                        volID = c.Int(nullable: false),
                        avDay = c.Int(nullable: false),
                        avFrom = c.DateTime(nullable: false),
                        avUntil = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.avID)
                .ForeignKey("dbo.Volunteer", t => t.volID, cascadeDelete: true)
                .Index(t => t.volID);
            
            CreateTable(
                "dbo.Volunteer",
                c => new
                    {
                        volID = c.Int(nullable: false, identity: true),
                        volFirstName = c.String(nullable: false, maxLength: 25),
                        volLastName = c.String(nullable: false, maxLength: 30),
                        volMiddleName = c.String(maxLength: 30),
                        volDOB = c.DateTime(nullable: false),
                        volEmail = c.String(nullable: false, maxLength: 50),
                        volPhone = c.String(nullable: false, maxLength: 15),
                        volStreet1 = c.String(nullable: false, maxLength: 30),
                        volStreet2 = c.String(maxLength: 30),
                        volCity = c.String(nullable: false, maxLength: 25),
                        volState = c.String(nullable: false, maxLength: 2),
                        volZip = c.String(nullable: false, maxLength: 5),
                        volStartDate = c.DateTime(nullable: false),
                        volActive = c.Boolean(nullable: false),
                        spcID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.volID)
                .ForeignKey("dbo.Specialty", t => t.spcID, cascadeDelete: true)
                .Index(t => t.spcID);
            
            CreateTable(
                "dbo.Contract",
                c => new
                    {
                        ctrNum = c.Int(nullable: false),
                        volID = c.Int(nullable: false),
                        pgrID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ctrNum, t.volID, t.pgrID })
                .ForeignKey("dbo.Pagroup", t => t.pgrID, cascadeDelete: true)
                .ForeignKey("dbo.Volunteer", t => t.volID, cascadeDelete: true)
                .Index(t => t.volID)
                .Index(t => t.pgrID);
            
            CreateTable(
                "dbo.Pagroup",
                c => new
                    {
                        pgrID = c.Int(nullable: false, identity: true),
                        pgrName = c.String(nullable: false, maxLength: 50),
                        pgrOfcFirstName = c.String(maxLength: 25),
                        pgrOfcLastName = c.String(maxLength: 30),
                        pgrPhone = c.String(nullable: false, maxLength: 15),
                        pgrStreet1 = c.String(nullable: false, maxLength: 30),
                        pgrStreet2 = c.String(maxLength: 30),
                        pgrCity = c.String(nullable: false, maxLength: 25),
                        pgrState = c.String(nullable: false, maxLength: 2),
                        pgrZip = c.String(nullable: false, maxLength: 5),
                    })
                .PrimaryKey(t => t.pgrID);
            
            CreateTable(
                "dbo.Econtact",
                c => new
                    {
                        volID = c.Int(nullable: false),
                        ecFirstName = c.String(nullable: false, maxLength: 25),
                        ecLastName = c.String(nullable: false, maxLength: 30),
                        ecPhone = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.volID)
                .ForeignKey("dbo.Volunteer", t => t.volID)
                .Index(t => t.volID);
            
            CreateTable(
                "dbo.Job",
                c => new
                    {
                        volID = c.Int(nullable: false),
                        empID = c.Int(nullable: false),
                        jobTitle = c.String(nullable: false, maxLength: 30),
                        jobStartDate = c.DateTime(nullable: false),
                        jobEndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.volID, t.empID })
                .ForeignKey("dbo.Employer", t => t.empID, cascadeDelete: true)
                .ForeignKey("dbo.Volunteer", t => t.volID, cascadeDelete: true)
                .Index(t => t.volID)
                .Index(t => t.empID);
            
            CreateTable(
                "dbo.Employer",
                c => new
                    {
                        empID = c.Int(nullable: false, identity: true),
                        empName = c.String(nullable: false, maxLength: 30),
                        empPhone = c.String(nullable: false, maxLength: 15),
                        empStreet1 = c.String(nullable: false, maxLength: 30),
                        empStreet2 = c.String(maxLength: 30),
                        empCity = c.String(nullable: false, maxLength: 25),
                        empState = c.String(nullable: false, maxLength: 2),
                        empZip = c.String(nullable: false, maxLength: 5),
                    })
                .PrimaryKey(t => t.empID);
            
            CreateTable(
                "dbo.License",
                c => new
                    {
                        volID = c.Int(nullable: false),
                        lcDate = c.DateTime(nullable: false),
                        lcClear = c.Boolean(nullable: false),
                        lcExpire = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.volID)
                .ForeignKey("dbo.Volunteer", t => t.volID)
                .Index(t => t.volID);
            
            CreateTable(
                "dbo.Speak",
                c => new
                    {
                        lngID = c.Int(nullable: false),
                        volID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.lngID, t.volID })
                .ForeignKey("dbo.Language", t => t.lngID, cascadeDelete: true)
                .ForeignKey("dbo.Volunteer", t => t.volID, cascadeDelete: true)
                .Index(t => t.lngID)
                .Index(t => t.volID);
            
            CreateTable(
                "dbo.Language",
                c => new
                    {
                        lngID = c.Int(nullable: false, identity: true),
                        lngName = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.lngID);
            
            CreateTable(
                "dbo.Specialty",
                c => new
                    {
                        spcID = c.Int(nullable: false, identity: true),
                        spcName = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.spcID);
            
            CreateTable(
                "dbo.Worktime",
                c => new
                    {
                        wrkID = c.Int(nullable: false),
                        volID = c.Int(nullable: false),
                        wrkDate = c.DateTime(nullable: false),
                        wrkStartTime = c.DateTime(nullable: false),
                        wrkEndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.wrkID, t.volID })
                .ForeignKey("dbo.Volunteer", t => t.volID, cascadeDelete: true)
                .Index(t => t.volID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Availability", "volID", "dbo.Volunteer");
            DropForeignKey("dbo.Worktime", "volID", "dbo.Volunteer");
            DropForeignKey("dbo.Volunteer", "spcID", "dbo.Specialty");
            DropForeignKey("dbo.Speak", "volID", "dbo.Volunteer");
            DropForeignKey("dbo.Speak", "lngID", "dbo.Language");
            DropForeignKey("dbo.License", "volID", "dbo.Volunteer");
            DropForeignKey("dbo.Job", "volID", "dbo.Volunteer");
            DropForeignKey("dbo.Job", "empID", "dbo.Employer");
            DropForeignKey("dbo.Econtact", "volID", "dbo.Volunteer");
            DropForeignKey("dbo.Contract", "volID", "dbo.Volunteer");
            DropForeignKey("dbo.Contract", "pgrID", "dbo.Pagroup");
            DropIndex("dbo.Worktime", new[] { "volID" });
            DropIndex("dbo.Speak", new[] { "volID" });
            DropIndex("dbo.Speak", new[] { "lngID" });
            DropIndex("dbo.License", new[] { "volID" });
            DropIndex("dbo.Job", new[] { "empID" });
            DropIndex("dbo.Job", new[] { "volID" });
            DropIndex("dbo.Econtact", new[] { "volID" });
            DropIndex("dbo.Contract", new[] { "pgrID" });
            DropIndex("dbo.Contract", new[] { "volID" });
            DropIndex("dbo.Volunteer", new[] { "spcID" });
            DropIndex("dbo.Availability", new[] { "volID" });
            DropTable("dbo.Worktime");
            DropTable("dbo.Specialty");
            DropTable("dbo.Language");
            DropTable("dbo.Speak");
            DropTable("dbo.License");
            DropTable("dbo.Employer");
            DropTable("dbo.Job");
            DropTable("dbo.Econtact");
            DropTable("dbo.Pagroup");
            DropTable("dbo.Contract");
            DropTable("dbo.Volunteer");
            DropTable("dbo.Availability");
        }
    }
}
