using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class ErrorOccuranceMap : EntityTypeConfiguration<ErrorOccurance>
    {
        public ErrorOccuranceMap()
        {
            // Table & Column Mappings
            this.ToTable("ErrorOccurances");

            this.HasRequired(t => t.ErrorMessage).WithMany(t => t.ErrorOccurances).HasForeignKey(t => t.ErrorMessageId);
        }
    }
}
