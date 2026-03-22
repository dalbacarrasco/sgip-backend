using Domain.Loans.Entities;
using Domain.Loans.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("loans");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .HasColumnName("id");

            builder.Property(l => l.ClientId)
                .HasColumnName("client_id")
                .IsRequired();

            builder.OwnsOne(l => l.Amount, money =>
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

            builder.OwnsOne(l => l.InterestRate, rate =>
            {
                rate.Property(r => r.AnnualRate)
                    .HasColumnName("annual_rate")
                    .HasPrecision(5, 4)
                    .IsRequired();
            });

            builder.Property(l => l.TermInMonths)
                .HasColumnName("term_in_months")
                .IsRequired();

            builder.Property(l => l.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(l => l.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(l => l.DisbursementDate)
                .HasColumnName("disbursement_date");

            builder.Property(l => l.RejectionReason)
                .HasColumnName("rejection_reason")
                .HasMaxLength(500);

            builder.Property(l => l.RowVersion)
                .HasColumnName("row_version")
                .IsConcurrencyToken();

            builder.Property(l => l.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            builder.Property(l => l.CreatedAt)
                .HasColumnName("created_at");

            builder.Property(l => l.UpdatedAt)
                .HasColumnName("updated_at");

            builder.HasQueryFilter(l => !l.IsDeleted);

            builder.HasMany(l => l.PaymentSchedule)
                .WithOne()
                .HasForeignKey(p => p.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(l => l.ClientId)
                .HasDatabaseName("ix_loans_client_id");

            builder.HasIndex(l => l.Status)
                .HasDatabaseName("ix_loans_status");
        }
    }
}