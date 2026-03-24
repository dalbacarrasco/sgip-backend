using Domain.Transactions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.ClientId)
                .HasColumnName("client_id")
                .IsRequired();

            builder.Property(t => t.IdempotencyKey)
                .HasColumnName("idempotency_key")
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsOne(t => t.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(t => t.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.ReferenceId)
                .HasColumnName("reference_id");

            builder.Property(t => t.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            builder.Property(t => t.Metadata)
                .HasColumnName("metadata")
                .HasColumnType("jsonb");

            builder.Property(t => t.FailureReason)
                .HasColumnName("failure_reason")
                .HasMaxLength(500);

            builder.Property(t => t.RowVersion)
                .HasColumnName("row_version")
                .IsConcurrencyToken();

            builder.Property(t => t.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at");

            builder.Property(t => t.UpdatedAt)
                .HasColumnName("updated_at");

            builder.HasQueryFilter(t => !t.IsDeleted);

            builder.HasIndex(t => t.IdempotencyKey)
                .IsUnique()
                .HasDatabaseName("ix_transactions_idempotency_key");

            builder.HasIndex(t => t.ClientId)
                .HasDatabaseName("ix_transactions_client_id");
        }
    }
}