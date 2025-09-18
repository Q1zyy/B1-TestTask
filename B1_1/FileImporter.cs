using System.Globalization;
using System.Text;

namespace B1_1;

public class FileImporter
{
    private readonly RecordsContext _context; // контекст БД (EF Core)

    public FileImporter()
    {
        _context = new RecordsContext(); // создаём новый DbContext
    }

    public async Task ImportFile(string filePath, int batchSize = 10_000)
    {
        int processed = 0; // сколько строк уже импортировано
        var buffer = new List<Record>(batchSize);

        int totalLines = 0;
        using var counter = new StreamReader(filePath); // первый проход по файлу
        while ((await counter.ReadLineAsync()) != null)
        {
            totalLines++; // считаем общее количество строк
        }

        using var reader = new StreamReader(filePath); // второй проход для чтения данных

        string line;
        while ((line = await reader.ReadLineAsync()) != null) // читаем построчно
        {
            var parts = line.Split("||", StringSplitOptions.RemoveEmptyEntries); // разбиваем строку
            if (parts.Length < 5)
            {
                continue;
            }
            // парсим значения из файла
            DateTime date = DateTime.ParseExact(parts[0], "dd.MM.yyyy", null);
            string englishString = parts[1];
            string russianString = parts[2];
            int integerValue = int.Parse(parts[3]);
            double doubleValue = double.Parse(parts[4].Replace(',', '.'), CultureInfo.InvariantCulture);

            // создаём сущность для EF Core
            buffer.Add(new Record
            {
                DateField = date,
                EnglishString = englishString,
                RussinaString = russianString,
                PositiveEvenInteger = integerValue,
                DoubleValue = doubleValue
            });

            if (buffer.Count >= batchSize) // если буфер заполнился
            {
                await _context.Records.AddRangeAsync(buffer); // добавляем пачку записей в EF
                await _context.SaveChangesAsync(); // сохраняем в БД
                processed += buffer.Count; // увеличиваем счётчик
                buffer.Clear(); // очищаем буфер
                Console.WriteLine($"Импортировано {processed} строк. Осталось {totalLines - processed}");
            }
        }

        if (buffer.Count > 0) // сохраняем оставшиеся строки
        {
            await _context.Records.AddRangeAsync(buffer); // добавляем в EF
            await _context.SaveChangesAsync(); // сохраняем в БД
            processed += buffer.Count;
        }

        Console.WriteLine($"Импорт завершён. Всего: {processed} строк."); // лог по завершении
    }
}
