using System.Text;

namespace B1_1;

public class FileGenerator
{
    // Алфавит для генерации английских и русских строк
    private static readonly string _englishAlpahbet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private static readonly string _russianAlpahbet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

    public static void GenerateFiles(string filesFolder, int fileCount = 1, int linesCount = 1)
    {
        var folder = Path.Combine(filesFolder, "GeneratedFiles"); // папка для сохранения файлов
        Directory.CreateDirectory(folder); // создаём директорию при необходимости
        var random = new Random(); // генератор случайных чисел

        for (int i = 0; i < fileCount; i++) 
        {
            string path = Path.Combine(folder, $"file_{i + 1}.txt"); // путь к файлу
            using var streamWriter = new StreamWriter(path); 

            for (int j = 0; j < linesCount; j++) 
            {
                var startDate = DateTime.Now.AddYears(-5); // стартовая дата
                var daysDifference = (DateTime.Now - startDate).Days; // разница в днях
                var randomDate = startDate.AddDays(random.Next(daysDifference)); // случайная дата
                var randomDateFormatted = randomDate.ToString("dd.MM.yyyy"); // формат даты

                var randomRussianLetters = GenerateString(_russianAlpahbet, 10); // случайные буквы (русские)
                var randomEnglishLetters = GenerateString(_englishAlpahbet, 10); // случайные буквы (английские)

                var randomNaturalNumber = random.Next(1, 50_000_000) * 2; // случайное натуральное число (чётное)

                var randomDouble = random.Next(1, 20) + random.NextDouble(); // случайное дробное число
                var randomDobuleString = randomDouble.ToString("F8"); // формат с 8 знаками после запятой

                // формируем строку с разделителями
                var line = $"{randomDateFormatted}||{randomEnglishLetters}||{randomRussianLetters}||{randomNaturalNumber}||{randomDobuleString}||";

                streamWriter.WriteLine(line); // записываем строку в файл
            }
        }
    }

    private static string GenerateString(string alphabet, int length)
    {
        var random = new Random();  
        var result = new StringBuilder(); 

        for (int i = 0; i < length; i++) // генерируем символы
        {
            result.Append(alphabet[random.Next(alphabet.Length)]);
        }

        return result.ToString(); // возвращаем итоговую строку
    }
}
