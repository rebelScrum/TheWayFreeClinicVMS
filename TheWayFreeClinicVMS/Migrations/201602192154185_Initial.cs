namespace TheWayFreeClinicVMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
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
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Volunteer_volID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Volunteer", t => t.Volunteer_volID)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Volunteer_volID);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Contract",
                c => new
                    {
                        contrID = c.Int(nullable: false, identity: true),
                        ctrNum = c.String(maxLength: 30),
                        volID = c.Int(nullable: false),
                        pgrID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.contrID)
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
                        ecID = c.Int(nullable: false, identity: true),
                        volID = c.Int(nullable: false),
                        ecFirstName = c.String(nullable: false, maxLength: 25),
                        ecLastName = c.String(nullable: false, maxLength: 30),
                        ecPhone = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.ecID)
                .ForeignKey("dbo.Volunteer", t => t.volID, cascadeDelete: true)
                .Index(t => t.volID);
            
            CreateTable(
                "dbo.Job",
                c => new
                    {
                        jobID = c.Int(nullable: false, identity: true),
                        volID = c.Int(nullable: false),
                        empID = c.Int(nullable: false),
                        jobTitle = c.String(nullable: false, maxLength: 30),
                        jobStartDate = c.DateTime(nullable: false),
                        jobEndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.jobID)
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
                        licenceID = c.Int(nullable: false, identity: true),
                        volID = c.Int(nullable: false),
                        lcNum = c.Int(nullable: false),
                        lcDate = c.DateTime(nullable: false),
                        lcClear = c.Boolean(nullable: false),
                        lcExpire = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.licenceID)
                .ForeignKey("dbo.Volunteer", t => t.volID, cascadeDelete: true)
                .Index(t => t.volID);
            
            CreateTable(
                "dbo.Speak",
                c => new
                    {
                        speakID = c.Int(nullable: false, identity: true),
                        lngID = c.Int(nullable: false),
                        volID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.speakID)
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
                        wrkID = c.Int(nullable: false, identity: true),
                        volID = c.Int(nullable: false),
                        wrkDate = c.DateTime(nullable: false),
                        wrkStartTime = c.DateTime(nullable: false),
                        wrkEndTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.wrkID)
                .ForeignKey("dbo.Volunteer", t => t.volID, cascadeDelete: true)
                .Index(t => t.volID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
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
            DropForeignKey("dbo.AspNetUsers", "Volunteer_volID", "dbo.Volunteer");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Worktime", new[] { "volID" });
            DropIndex("dbo.Speak", new[] { "volID" });
            DropIndex("dbo.Speak", new[] { "lngID" });
            DropIndex("dbo.License", new[] { "volID" });
            DropIndex("dbo.Job", new[] { "empID" });
            DropIndex("dbo.Job", new[] { "volID" });
            DropIndex("dbo.Econtact", new[] { "volID" });
            DropIndex("dbo.Contract", new[] { "pgrID" });
            DropIndex("dbo.Contract", new[] { "volID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Volunteer_volID" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Volunteer", new[] { "spcID" });
            DropIndex("dbo.Availability", new[] { "volID" });
            DropTable("dbo.AspNetRoles");
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
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Volunteer");
            DropTable("dbo.Availability");
        }
    }
}
