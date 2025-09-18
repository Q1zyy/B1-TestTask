namespace B1_2.Entities;

public class AccountClassEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; 
    public ICollection<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
}
