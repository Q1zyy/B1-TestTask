using B1_2.DTOs;
using B1_2.Entities;
using B1_2.Infrastructure;
using B1_2.Interfaces;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Text;

namespace B1_2.Services;

public class FileService : IFileService
{
    private readonly AppDbContext _dbContext;

    public FileService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ParseExcelFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        // проверка на пустой файл
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("No file uploaded");
        }

        IWorkbook workbook;
        // открытие потока файла
        using var stream = file.OpenReadStream();

        if (Path.GetExtension(file.FileName).ToLower() == ".xls")
        {
            workbook = new HSSFWorkbook(stream); // старый Excel формат
        }
        else
        {
            workbook = new XSSFWorkbook(stream); // новый Excel формат
        }

        var name = Path.GetFileName(file.FileName);
        // берем первый лист
        var sheet = workbook.GetSheetAt(0); 

        int bankId = await ProcessBankNameAsync(sheet, cancellationToken); // обработка банка
        int fileId = await ProcessFileInfoAsync(name.ToString(), bankId, sheet, cancellationToken); // инфо о файле
        await ProcessTableAsync(sheet, fileId, cancellationToken); // обработка таблицы
    }

    private async Task<int> ProcessBankNameAsync(ISheet sheet, CancellationToken cancellationToken)
    {
        // банк из первой строки
        var bankName = sheet.GetRow(0).GetCell(0).ToString(); 

        // ищем банк по имени
        var exists = await _dbContext.Banks
            .FirstOrDefaultAsync(b => b.Name == bankName, cancellationToken);

        //если банка нет
        if (exists == null)
        {
            var bank = new BankEntity { Name = bankName };

            _dbContext.Add(bank); //добавляем новый банк в контекст
            await _dbContext.SaveChangesAsync(cancellationToken); //сохраняем изменения в БД

            return bank.Id; // возвращаем Id нового банка
        }

        return exists.Id; // возвращаем Id найденного банка
    }

    private async Task<int> ProcessFileInfoAsync(string fileName, int bankId, ISheet sheet, CancellationToken cancellationToken)
    {
        // проверяем, загружался ли уже этот файл
        var existed = await _dbContext.UploadedFiles
            .FirstOrDefaultAsync(u => u.FileName == fileName, cancellationToken);

        //если банка нет
        if (existed == null)
        {
            //парсим данные
            var line2 = sheet.GetRow(2).GetCell(0).ToString().Split(' ');
            var line6 = sheet.GetRow(5).GetCell(0).ToString();

            var uploadDate = DateTime.SpecifyKind(DateTime.Parse(line6), DateTimeKind.Utc);
            var periodStart = DateTime.SpecifyKind(DateTime.ParseExact(line2[3], "dd.MM.yyyy", CultureInfo.InvariantCulture), DateTimeKind.Utc);
            var periodEnd = DateTime.SpecifyKind(DateTime.ParseExact(line2[5], "dd.MM.yyyy", CultureInfo.InvariantCulture), DateTimeKind.Utc);

            var uploadedFile = new UploadedFileEntity
            {
                BankId = bankId,
                FileName = fileName,
                UploadDate = uploadDate,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
            };

            _dbContext.UploadedFiles.Add(uploadedFile); //добавляем запись о файле
            await _dbContext.SaveChangesAsync(cancellationToken); //сохраняем изменения в БД

            return uploadedFile.Id;
        }

        return existed.Id; // если файл есть возвращаем его Id
    }

    private async Task ProcessTableAsync(ISheet sheet, int fileId, CancellationToken cancellationToken)
    {
        int latestClassId = 0;

        for (int i = 8; i <= sheet.LastRowNum; i++) 
        {
            // получаем i строку
            var row = sheet.GetRow(i);
            if (row == null)
            {
                continue;
            }
            bool isClass = false;
            string[] cells = new string[7];

            for (int j = 0; j < 7; j++)
            {
                //получаем клетку на позиции i,j
                cells[j] = row.GetCell(j).ToString();
                //если это КЛАСС
                if (cells[j].Contains("КЛАСС "))
                {
                    isClass = true;
                    latestClassId = await ProcessAccountClassAsync(cells[j], cancellationToken); // обработка класса
                }
            }

            if (cells[0] == "")
            {
                continue; // пропускаем пустые строки
            }

            //если не класс
            if (!isClass)
            {
                var account = new AccountEntity
                {
                    Code = cells[0],
                    AccountClassEntityId = latestClassId,
                    OpeningDebit = decimal.Parse(cells[1]),
                    OpeningCredit = decimal.Parse(cells[2]),
                    TurnoverDebit = decimal.Parse(cells[3]),
                    TurnoverCredit = decimal.Parse(cells[4]),
                    ClosingDebit = decimal.Parse(cells[5]),
                    ClosingCredit = decimal.Parse(cells[6]),
                    UploadedFileId = fileId,
                };

                _dbContext.Add(account); //добавляем счёт в контекст
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken); // сохраняем все добавленные счета
    }

    private async Task<int> ProcessAccountClassAsync(string cell, CancellationToken cancellationToken)
    {
        //разбиваем строку в клетке
        var splittedCell = cell.Split("  ");

        var accountClass = new AccountClassEntity
        {
            Code = splittedCell[1],
            Name = splittedCell[2],
        };

        // ищем существующий класс по коду и имени
        var existed = await _dbContext.AccountClasses
            .FirstOrDefaultAsync(ac => ac.Name == accountClass.Name && ac.Code == accountClass.Code, cancellationToken);

        if (existed == null)
        {
            _dbContext.AccountClasses.Add(accountClass); // добавляем новый класс
            await _dbContext.SaveChangesAsync(cancellationToken); //сохраняем изменения
            return accountClass.Id;
        }

        return existed.Id; //если найден возвращаем Id
    }

    public async Task<List<UploadedFilesResponse>> GetUploadedFilesAsync(CancellationToken cancellationToken)
    {
        //получаем список файлов и проецируем в DTO
        return await _dbContext.UploadedFiles
            .Select(u => new UploadedFilesResponse
            {
                Id = u.Id,
                FileName = u.FileName
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<FileReportDto?> GetFileReportAsync(int fileId, CancellationToken cancellationToken)
    {
        // загружаем файл вместе с банком и счетами
        var file = await _dbContext.UploadedFiles
                .Include(f => f.Bank)
                .Include(f => f.Accounts)
                    .ThenInclude(a => a.AccountClass)
                .FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);

        if (file == null)
        {
            return null;
        }

        // группировка счетов по классам
        var grouped = file.Accounts
            .GroupBy(a => a.AccountClass)
            .Select(g => new AccountClassDto
            {
                Code = g.Key.Code,
                Name = g.Key.Name,
                Accounts = g.Select(a => new AccountRowDto
                {
                    Code = a.Code,
                    OpeningDebit = a.OpeningDebit,
                    OpeningCredit = a.OpeningCredit,
                    TurnoverDebit = a.TurnoverDebit,
                    TurnoverCredit = a.TurnoverCredit,
                    ClosingDebit = a.ClosingDebit,
                    ClosingCredit = a.ClosingCredit
                }).ToList()
            }).ToList();

        return new FileReportDto
        {
            BankName = file.Bank.Name,
            FileName = file.FileName,
            UploadDate = file.UploadDate,
            PeriodStart = file.PeriodStart,
            PeriodEnd = file.PeriodEnd,
            Classes = grouped
        };
    }

    public async Task<byte[]?> ExportFileToMarkdownAsync(int fileId, CancellationToken cancellationToken)
    {
        var fileReport = await GetFileReportAsync(fileId, cancellationToken); // получаем FileReport
        
        if (fileReport == null)
        {
            return null;
        }

        var sb = new StringBuilder();

        // формирование Markdown
        sb.AppendLine($"# {fileReport.BankName}");
        sb.AppendLine("## Оборотная ведомость по балансовым счетам");
        sb.AppendLine($"**за период с {fileReport.PeriodStart:dd.MM.yyyy} по {fileReport.PeriodEnd:dd.MM.yyyy}**");
        sb.AppendLine();
        sb.AppendLine($"Дата выгрузки: {fileReport.UploadDate:dd/MM/yyyy H:mm:ss}  \nв рублях");
        sb.AppendLine();

        sb.AppendLine("| Б/сч | Входящее сальдо Актив | Входящее сальдо Пассив | Обороты Дебет | Обороты Кредит | Исходящее сальдо Актив | Исходящее сальдо Пассив |");
        sb.AppendLine("|------|---------------------|-----------------------|---------------|----------------|------------------------|-------------------------|");

        foreach (var accountClass in fileReport.Classes)
        {
            sb.AppendLine($"| **КЛАСС {accountClass.Code} {accountClass.Name}** | | | | | | |");

            foreach (var account in accountClass.Accounts)
            {
                sb.AppendLine($"| {account.Code} | {account.OpeningDebit:N2} | {account.OpeningCredit:N2} | {account.TurnoverDebit:N2} | {account.TurnoverCredit:N2} | {account.ClosingDebit:N2} | {account.ClosingCredit:N2} |");
            }
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString()); // преобразуем в байты
        return bytes;
    }
}
