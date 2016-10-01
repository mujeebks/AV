using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class CurrencyMap : EntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            // Primary Key
            this.HasKey(t => t.CurrencyId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CurrencyName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Symbol)
                .IsRequired()
                .HasMaxLength(4);

            // Table & Column Mappings
            this.ToTable("Currencies");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.CurrencyName).HasColumnName("CurrencyName");
            this.Property(t => t.Symbol).HasColumnName("Symbol");
        }
    }
}
