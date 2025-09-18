namespace B1_1;

public class FileMerger
{
    public static int MergeFiles(string folder, string outputFile, string? filter = null)
    {
        int removed = 0; // счётчик удалённых строк (по фильтру)

        using var streamWriter = new StreamWriter(outputFile, true); // открываем выходной файл для

        foreach (var file in Directory.GetFiles(folder)) // перебираем все файлы в папке
        {
            foreach (var line in File.ReadLines(file)) // читаем строки из файла
            {
                if (filter != null && line.Contains(filter)) // если строка подходит под фильтр
                {
                    removed++; // увеличиваем счётчик удалённых
                }
                else
                {
                    streamWriter.WriteLine(line); // иначе пишем строку в выходной файл
                }
            }
        }

        return removed; // возвращаем количество удалённых строк
    }
}
