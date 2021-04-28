namespace WebScraping.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class New : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Websites", "WebsiteUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Websites", "WebsiteUrl");
        }
    }
}
