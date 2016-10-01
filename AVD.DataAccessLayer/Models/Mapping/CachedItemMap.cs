using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class CachedItemMap : EntityTypeConfiguration<CachedItem>
    {
        public CachedItemMap()
        {
            // Primary Key
            this.HasKey(t => t.CachedItemId);

            // Properties
            this.Property(t => t.CacheKey)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.Value)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("CachedItems");
            this.Property(t => t.CachedItemId).HasColumnName("CachedItemId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.CacheKey).HasColumnName("CacheKey");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.ElapsedMinutes).HasColumnName("ElapsedMinutes");
            this.Property(t => t.ExpiryMinutes).HasColumnName("ExpiryMinutes");

            // Relationships
            this.HasOptional(t => t.User)
                .WithMany(t => t.CachedItems)
                .HasForeignKey(d => d.UserId);

        }
    }
}
