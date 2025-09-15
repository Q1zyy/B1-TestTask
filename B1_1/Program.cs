using B1_1;

string filesFolder = "D:\\C#\\B1\\B1_1";
string mergedFile = "D:\\C#\\B1\\B1_1\\merged.txt";
string generatedFilesFolder = "D:\\C#\\B1\\B1_1\\GeneratedFiles";

//FileGenerator.GenerateFiles(filesFolder, 100, 100_000);
//var removed = FileMerger.MergeFiles(generatedFilesFolder, mergedFile);
//Console.WriteLine(removed);

var filePath = "D:\\C#\\B1\\B1_1\\GeneratedFiles\\file_1.txt";
var importer = new FileImporter();
await importer.ImportFile(filePath);

