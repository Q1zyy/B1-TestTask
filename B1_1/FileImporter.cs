using System.Globalization;
using System.Text;

namespace B1_1;

public class FileImporter
{
    private readonly RecordsContext _context;

    public FileImporter()
    {
        _context = new RecordsContext();
    }

    public async Task ImportFile(string filePath, int batchSize = 10_000)
    {
        int processed = 0;
        var buffer = new List<Record>(batchSize);

        int totalLines = 0;
        using var counter = new StreamReader(filePath);
        while ((await counter.ReadLineAsync()) != null)
        {
            totalLines++;
        }

        using var reader = new StreamReader(filePath);

        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            var parts = line.Split("||", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5) continue;

            DateTime date = DateTime.ParseExact(parts[0], "dd.MM.yyyy", null);
            string englishString = parts[1];
            string russianString = parts[2];
            int integerValue = int.Parse(parts[3]);
            double doubleValue = double.Parse(parts[4].Replace(',', '.'), CultureInfo.InvariantCulture);

            buffer.Add(new Record
            {
                DateField = date,
                EnglishString = englishString,
                RussinaString = russianString,
                PositiveEvenInteger = integerValue,
                DoubleValue = doubleValue
            });

            if (buffer.Count >= batchSize)
            {
                await _context.Records.AddRangeAsync(buffer);
                await _context.SaveChangesAsync();
                processed += buffer.Count;
                buffer.Clear();
                Console.WriteLine($"Импортировано {processed} строк. Осталось {totalLines - processed}");
            }
        }

        if (buffer.Count > 0)
        {
            await _context.Records.AddRangeAsync(buffer);
            await _context.SaveChangesAsync();
            processed += buffer.Count;
        }

        Console.WriteLine($"Импорт завершён. Всего: {processed} строк.");
    }
}
