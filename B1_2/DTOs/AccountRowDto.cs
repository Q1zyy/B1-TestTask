namespace B1_2.DTOs;

public class AccountRowDto
{
    public string Code { get; set; } = string.Empty;
    public decimal OpeningDebit { get; set; }
    public decimal OpeningCredit { get; set; }
    public decimal TurnoverDebit { get; set; }
    public decimal TurnoverCredit { get; set; }
    public decimal ClosingDebit { get; set; }
    public decimal ClosingCredit { get; set; }
}
