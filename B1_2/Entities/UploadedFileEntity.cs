namespace B1_2.Entities;

public class UploadedFileEntity
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; } = DateTime.Now;
    public int BankId { get; set; }
    public BankEntity? Bank { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public ICollection<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
}
