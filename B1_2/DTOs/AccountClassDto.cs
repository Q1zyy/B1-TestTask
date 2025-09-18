namespace B1_2.DTOs;

public class AccountClassDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<AccountRowDto> Accounts { get; set; } = new();
}
