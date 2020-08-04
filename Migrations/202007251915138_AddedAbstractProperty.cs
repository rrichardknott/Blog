namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAbstractProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Abstract", c => c.String());
            AddColumn("dbo.BlogPosts", "Abstract", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BlogPosts", "Abstract");
            DropColumn("dbo.Comments", "Abstract");
        }
    }
}
