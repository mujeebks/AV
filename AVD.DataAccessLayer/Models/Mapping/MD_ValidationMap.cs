using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_ValidationMap : EntityTypeConfiguration<MD_Validation>
    {
        public MD_ValidationMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.ValueType)
                .HasMaxLength(50);

            this.Property(t => t.Value)
                .HasMaxLength(200);

            this.Property(t => t.ErrorMessage)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("MD_Validation");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.EntityTypeID).HasColumnName("EntityTypeID");
            this.Property(t => t.RelationShipID).HasColumnName("RelationShipID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ValueType).HasColumnName("ValueType");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage");
        }
    }
}
