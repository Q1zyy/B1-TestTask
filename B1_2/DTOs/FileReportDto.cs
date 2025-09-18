namespace B1_2.DTOs;

public class FileReportDto
{
    public string BankName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

    public List<AccountClassDto> Classes { get; set; } = new();
}
