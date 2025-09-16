namespace B1_2.Entities;

public class BankExpensesEntity
{
    public int Id { get; set; }
    public int FileId { get; set; }
    public string? Account { get; set; }
    public decimal? SaldoInActive { get; set; }
    public decimal? SaldoInPassive { get; set; }
    public decimal? TurnoverDebit { get; set; }
    public decimal? TurnoverCredit { get; set; }
    public decimal? SaldoOutActive { get; set; }
    public decimal? SaldoOutPassive { get; set; }
}
