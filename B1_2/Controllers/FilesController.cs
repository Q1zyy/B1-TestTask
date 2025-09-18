using B1_2.DTOs;
using B1_2.Interfaces;
using B1_2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace B1_2.Controllers;

[ApiController] // делает контроллер API (автоматическая валидация, JSON по умолчанию)
[Route("api/files")] // базовый маршрут для всех методов контроллера
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService; // сервис для работы с файлами

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")] // POST api/files/upload
    public async Task<IActionResult> UploadFileAsync([FromForm] UploadFileDto file, CancellationToken cancellationToken)
    {
        await _fileService.ParseExcelFileAsync(file.FormFile, cancellationToken); // вызов сервиса для парсинга Excel
        return Ok(); // возвращаем статус 200
    }

    [HttpGet] // GET api/files
    public async Task<IActionResult> GetUploadedFilesAsync(CancellationToken cancellationToken)
    {
        var result = await _fileService.GetUploadedFilesAsync(cancellationToken); // список загруженных файлов
        return Ok(result); // возвращаем JSON со списком
    }

    [HttpGet("{fileId:int}")] // GET api/files/{fileId}
    public async Task<IActionResult> GetFileReport([FromRoute] int fileId, CancellationToken cancellationToken)
    {
        var report = await _fileService.GetFileReportAsync(fileId, cancellationToken); // получаем отчет по файлу

        if (report == null)
        {
            return NotFound(); // если файла нет — 404
        }
        return Ok(report); // возвращаем отчет в JSON
    }

    [HttpGet("{fileId}/md")] // GET api/files/{fileId}/md
    public async Task<IActionResult> ExportToCsvAsync([FromRoute] int fileId, CancellationToken cancellationToken)
    {
        var bytes = await _fileService.ExportFileToMarkdownAsync(fileId, cancellationToken); // экспорт отчета в Markdown

        if (bytes == null)
        {
            return NotFound(); // если файла нет — 404
        }

        return File(bytes, "text/markdown", $"{fileId}_export.md"); // скачивание .md файла
    }
}
