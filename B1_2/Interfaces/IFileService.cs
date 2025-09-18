using B1_2.DTOs;

namespace B1_2.Interfaces;

public interface IFileService
{
    public Task ParseExcelFileAsync(IFormFile formFile, CancellationToken cancellationToken);

    public Task<List<UploadedFilesResponse>> GetUploadedFilesAsync(CancellationToken cancellationToken);

    public Task<FileReportDto?> GetFileReportAsync(int fileId, CancellationToken cancellationToken);

    public Task<byte[]?> ExportFileToMarkdownAsync(int fileId, CancellationToken cancellationToken);
}
