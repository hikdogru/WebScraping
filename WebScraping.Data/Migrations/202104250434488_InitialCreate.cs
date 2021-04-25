namespace WebScraping.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookNodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Publisher = c.String(),
                        Name = c.String(),
                        Author = c.String(),
                        Price = c.String(),
                        Image = c.String(),
                        Detail = c.String(),
                        ItemCount = c.Int(nullable: false),
                        WebsiteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Websites", t => t.WebsiteId, cascadeDelete: true)
                .Index(t => t.WebsiteId);
            
            CreateTable(
                "dbo.Websites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LogoUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Author = c.String(),
                        Publisher = c.String(),
                        Price = c.String(),
                        Image = c.String(),
                        BookDetailUrl = c.String(),
                        CategoryType = c.String(),
                        WebsiteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Websites", t => t.WebsiteId, cascadeDelete: true)
                .Index(t => t.WebsiteId);
            
            CreateTable(
                "dbo.WebsiteUrls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        UrlType = c.String(),
                        WebsiteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Websites", t => t.WebsiteId, cascadeDelete: true)
                .Index(t => t.WebsiteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookNodes", "WebsiteId", "dbo.Websites");
            DropForeignKey("dbo.WebsiteUrls", "WebsiteId", "dbo.Websites");
            DropForeignKey("dbo.Books", "WebsiteId", "dbo.Websites");
            DropIndex("dbo.WebsiteUrls", new[] { "WebsiteId" });
            DropIndex("dbo.Books", new[] { "WebsiteId" });
            DropIndex("dbo.BookNodes", new[] { "WebsiteId" });
            DropTable("dbo.WebsiteUrls");
            DropTable("dbo.Books");
            DropTable("dbo.Websites");
            DropTable("dbo.BookNodes");
        }
    }
}
