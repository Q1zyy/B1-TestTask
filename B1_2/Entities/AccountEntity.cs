namespace B1_2.Entities;

public class AccountEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;          
    public int AccountClassEntityId { get; set; }
    public AccountClassEntity? AccountClass { get; set; }
    public decimal OpeningDebit { get; set; }
    public decimal OpeningCredit { get; set; }
    public decimal TurnoverDebit { get; set; }
    public decimal TurnoverCredit { get; set; }
    public decimal ClosingDebit { get; set; }
    public decimal ClosingCredit { get; set; }
    public int UploadedFileId { get; set; }
    public UploadedFileEntity? UploadedFile { get; set; }
}
