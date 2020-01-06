﻿namespace MyWallet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_Country : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Country",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Country");
        }
    }
}
