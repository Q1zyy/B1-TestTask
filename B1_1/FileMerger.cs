namespace B1_1;

public class FileMerger
{
    public static int MergeFiles(string folder, string outputFile, string? filter = null)
    {
        int removed = 0;

        using var streamWriter = new StreamWriter(outputFile, true);

        foreach (var file in Directory.GetFiles(folder))
        {
            foreach (var line in File.ReadLines(file))
            {
                if (filter != null && line.Contains(filter))
                {
                    removed++;
                }
                else
                {
                    streamWriter.WriteLine(line);
                }
            }
        }

        return removed;
    }
}
