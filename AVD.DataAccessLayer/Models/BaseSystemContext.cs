using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AVD.DataAccessLayer.Models.Mapping;

namespace AVD.DataAccessLayer.Models
{

    /* Use TrackerEnabledDbContext to extend Entity Framework functionality to store changes in database. 
     * This is very useful for auditing purpose. It stores WHO changed WHAT and WHEN. 
     * It will let you choose which tables and columns you want to track with the help of attributes
    */

    public partial class BaseSystemContext : TrackerEnabledDbContext.TrackerContext
    {
        static BaseSystemContext()
        {
            Database.SetInitializer<BaseSystemContext>(null);
        }

        public BaseSystemContext()
            : base("Name=BaseSystemContext")
        {
        }

        public DbSet<AM_LOG> AM_LOG { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<MD_Attribute> MD_Attribute { get; set; }
        public DbSet<MD_AttributeType> MD_AttributeType { get; set; }
        public DbSet<MD_EntityType> MD_EntityType { get; set; }
        public DbSet<MD_EntityType_Feature> MD_EntityType_Feature { get; set; }
        public DbSet<MD_EntityType_Hierarchy> MD_EntityType_Hierarchy { get; set; }
        public DbSet<MD_Feature> MD_Feature { get; set; }
        public DbSet<MD_MetadataVersion> MD_MetadataVersion { get; set; }
        public DbSet<MD_Module> MD_Module { get; set; }
        public DbSet<MD_Option> MD_Option { get; set; }
        public DbSet<MD_TreeLevel> MD_TreeLevel { get; set; }
        public DbSet<MD_TreeNode> MD_TreeNode { get; set; }
        public DbSet<MD_Validation> MD_Validation { get; set; }
        public DbSet<UM_User> UM_User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AM_LOGMap());
            modelBuilder.Configurations.Add(new ErrorMap());
            modelBuilder.Configurations.Add(new MD_AttributeMap());
            modelBuilder.Configurations.Add(new MD_AttributeTypeMap());
            modelBuilder.Configurations.Add(new MD_EntityTypeMap());
            modelBuilder.Configurations.Add(new MD_EntityType_FeatureMap());
            modelBuilder.Configurations.Add(new MD_EntityType_HierarchyMap());
            modelBuilder.Configurations.Add(new MD_FeatureMap());
            modelBuilder.Configurations.Add(new MD_MetadataVersionMap());
            modelBuilder.Configurations.Add(new MD_ModuleMap());
            modelBuilder.Configurations.Add(new MD_OptionMap());
            modelBuilder.Configurations.Add(new MD_TreeLevelMap());
            modelBuilder.Configurations.Add(new MD_TreeNodeMap());
            modelBuilder.Configurations.Add(new MD_ValidationMap());
            modelBuilder.Configurations.Add(new UM_UserMap());
        }
    }
}
