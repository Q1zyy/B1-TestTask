using System.Text;

namespace B1_1;

public class FileGenerator
{
    private static readonly string _englishAlpahbet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private static readonly string _russianAlpahbet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

    public static void GenerateFiles(string filesFolder, int fileCount = 1, int linesCount = 1)
    {
        var folder = Path.Combine(filesFolder, "GeneratedFiles");

        Directory.CreateDirectory(folder);
        var random = new Random();

        for (int i = 0; i < fileCount; i++)
        {
            string path = Path.Combine(folder, $"file_{i + 1}.txt");
            using var streamWriter = new StreamWriter(path);

            for (int j = 0; j < linesCount; j++)
            {
                var startDate = DateTime.Now.AddYears(-5);
                var daysDifference = (DateTime.Now - startDate).Days;
                var randomDate = startDate.AddDays(random.Next(daysDifference));
                var randomDateFormatted = randomDate.ToString("dd.MM.yyyy");

                var randomRussianLetters = GenerateString(_russianAlpahbet, 10);

                var randomEnglishLetters = GenerateString(_englishAlpahbet, 10);
        
                var randomNaturalNumber = random.Next(1, 50_000_000) * 2;   

                var randomDouble = random.Next(1, 20) + random.NextDouble();
                var randomDobuleString = randomDouble.ToString("F8");

                var line = $"{randomDateFormatted}||{randomEnglishLetters}||{randomRussianLetters}||{randomNaturalNumber}||{randomDobuleString}||";

                streamWriter.WriteLine(line);
            }
        }
    }

    private static string GenerateString(string alphabet, int length)
    {
        var random = new Random();  
        var result = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            result.Append(alphabet[random.Next(alphabet.Length)]);
        }

        return result.ToString();
    }
}
