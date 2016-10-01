using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class ErrorMessageMap : EntityTypeConfiguration<ErrorMessage>
    {
        public ErrorMessageMap()
        {
            // Primary Key
            this.HasKey(t => t.ErrorMessageId);

            // Properties
            this.Property(t => t.ExceptionFullName)
                .IsRequired()
                .HasMaxLength(2000);

            this.Property(t => t.ProviderCode)
                .HasMaxLength(100);

            this.Property(t => t.InternalDescription)
                .HasMaxLength(2000);

            this.Property(t => t.UserTitle)
                .HasMaxLength(500);

            this.Property(t => t.UserMessage)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("ErrorMessages");
            this.Property(t => t.ErrorMessageId).HasColumnName("ErrorMessageId");
            this.Property(t => t.ErrorTypeId).HasColumnName("ErrorTypeId");
            this.Property(t => t.ExceptionFullName).HasColumnName("ExceptionFullName");
            this.Property(t => t.ProviderId).HasColumnName("ProviderId");
            this.Property(t => t.ProviderCode).HasColumnName("ProviderCode");
            this.Property(t => t.ProviderMessage).HasColumnName("ProviderMessage");
            this.Property(t => t.InternalDescription).HasColumnName("InternalDescription");
            this.Property(t => t.UserTitle).HasColumnName("UserTitle");
            this.Property(t => t.UserMessage).HasColumnName("UserMessage");
            this.Property(t => t.IsInline).HasColumnName("IsInline");
            this.Property(t => t.IsSupportRequestEnabled).HasColumnName("IsSupportRequestEnabled");
            this.Property(t => t.Reviewed).HasColumnName("Reviewed");

        }
    }
}
