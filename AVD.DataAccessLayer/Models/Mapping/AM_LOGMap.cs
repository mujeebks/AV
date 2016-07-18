using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class AM_LOGMap : EntityTypeConfiguration<AM_LOG>
    {
        public AM_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ActionName)
                .IsRequired();

            this.Property(t => t.ActionDescription)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("AM_LOG");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.ActionName).HasColumnName("ActionName");
            this.Property(t => t.ActionDescription).HasColumnName("ActionDescription");
            this.Property(t => t.ActionDate).HasColumnName("ActionDate");
        }
    }
}
