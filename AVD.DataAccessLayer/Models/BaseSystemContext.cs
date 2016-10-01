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

        public DbSet<AM_LOG> AmLog { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<MD_Attribute> MdAttribute { get; set; }
        public DbSet<MD_AttributeType> MdAttributeType { get; set; }
        public DbSet<MD_EntityType> MdEntityType { get; set; }
        public DbSet<MD_EntityType_Feature> MdEntityTypeFeature { get; set; }
        public DbSet<MD_EntityType_Hierarchy> MdEntityTypeHierarchy { get; set; }
        public DbSet<MD_Feature> MdFeature { get; set; }
        public DbSet<MD_MetadataVersion> MdMetadataVersion { get; set; }
        public DbSet<MD_Module> MdModule { get; set; }
        public DbSet<MD_Option> MdOption { get; set; }
        public DbSet<MD_TreeLevel> MdTreeLevel { get; set; }
        public DbSet<MD_TreeNode> MdTreeNode { get; set; }
        public DbSet<MD_Validation> MdValidation { get; set; }
        public DbSet<User> UmUser { get; set; }

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
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}
