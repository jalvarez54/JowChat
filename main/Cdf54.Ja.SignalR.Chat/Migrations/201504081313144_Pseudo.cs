namespace Cdf54.Ja.SignalR.Chat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pseudo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Pseudo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Pseudo");
        }
    }
}
