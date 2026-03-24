namespace Domain.Loans.Enums
{
    public enum LoanStatus
    {
        Draft,           // borrador, recién creado
        PendingApproval, // enviado, esperando revisión
        Evaluating,      // el sistema está calculando el score
        Approved,        // aprobado
        Rejected,        // rechazado
        Disbursed,       // dinero enviado al cliente
        Active,          // préstamo en curso, cliente pagando
    }
}