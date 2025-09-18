using B1_1;

string filesFolder = "D:\\C#\\B1\\B1_1";
string mergedFile = "D:\\C#\\B1\\B1_1\\merged.txt";
string generatedFilesFolder = "D:\\C#\\B1\\B1_1\\GeneratedFiles";

while (true)
{
    string s = Console.ReadLine();
    if (s == "1")
    {
        FileGenerator.GenerateFiles(filesFolder, 100, 100_000);
    }
    else if (s == "2")
    {
        var removed = FileMerger.MergeFiles(generatedFilesFolder, mergedFile, "abc");
        Console.WriteLine($"Removed {removed}");
    }
    else if (s == "3")
    {
        var filePath = "D:\\C#\\B1\\B1_1\\GeneratedFiles\\file_1.txt";
        var importer = new FileImporter();
        await importer.ImportFile(filePath);
    }
}
