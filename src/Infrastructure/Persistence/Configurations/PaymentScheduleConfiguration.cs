using Domain.Loans.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PaymentScheduleConfiguration : IEntityTypeConfiguration<PaymentScheduleItem>
    {
        public void Configure(EntityTypeBuilder<PaymentScheduleItem> builder)
        {
            builder.ToTable("payment_schedule_items");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.LoanId)
                .HasColumnName("loan_id")
                .IsRequired();

            builder.Property(p => p.InstallmentNumber)
                .HasColumnName("installment_number")
                .IsRequired();

            builder.Property(p => p.NominalDueDate)
                .HasColumnName("nominal_due_date")
                .IsRequired();

            builder.Property(p => p.DueDate)
                .HasColumnName("due_date")
                .IsRequired();

            builder.OwnsOne(p => p.TotalAmount, m =>
            {
                m.Property(x => x.Amount).HasColumnName("total_amount").HasPrecision(18, 2);
                m.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3);
            });

            builder.OwnsOne(p => p.PrincipalAmount, m =>
            {
                m.Property(x => x.Amount).HasColumnName("principal_amount").HasPrecision(18, 2);
                m.Ignore(x => x.Currency);
            });

            builder.OwnsOne(p => p.InterestAmount, m =>
            {
                m.Property(x => x.Amount).HasColumnName("interest_amount").HasPrecision(18, 2);
                m.Ignore(x => x.Currency);
            });

            builder.OwnsOne(p => p.RemainingBalance, m =>
            {
                m.Property(x => x.Amount).HasColumnName("remaining_balance").HasPrecision(18, 2);
                m.Ignore(x => x.Currency);
            });

            builder.Property(p => p.EffectiveRate)
                .HasColumnName("effective_rate")
                .HasPrecision(10, 8);

            builder.Property(p => p.ActualDays)
                .HasColumnName("actual_days");

            builder.Property(p => p.Status)
                .HasColumnName("status")
                .HasConversion<string>();

            builder.Property(p => p.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at");

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasIndex(p => p.LoanId)
                .HasDatabaseName("ix_payment_schedule_loan_id");
        }
    }
}