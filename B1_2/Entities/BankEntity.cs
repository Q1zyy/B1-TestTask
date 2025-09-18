namespace B1_2.Entities;

public class BankEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<UploadedFileEntity> Files { get; set; } = new List<UploadedFileEntity>();
}
