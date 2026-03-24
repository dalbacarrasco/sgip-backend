namespace Domain.Transactions.Enums
{
    public enum TransactionType
    {
        LoanDisbursement,  // desembolso de préstamo
        InstallmentPayment, // pago de cuota
        PortfolioInvestment, // inversión en portafolio
        Withdrawal,         // retiro de fondos
        Transfer            // transferencia entre cuentas
    }
}