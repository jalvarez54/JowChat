namespace Cdf54.Ja.SignalR.Chat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UseGravatar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UseGravatar", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UseGravatar");
        }
    }
}
