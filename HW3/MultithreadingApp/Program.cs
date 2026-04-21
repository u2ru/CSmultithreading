var basePath = AppContext.BaseDirectory;
var file1 = Path.Combine(basePath, "file1.xml");
var file2 = Path.Combine(basePath, "file2.xml");
var file3 = Path.Combine(basePath, "file3.txt");

var devices = SampleData.CreateDevices(20);
var worker = new MultiThreadDeviceWorker();

Console.WriteLine("Task 1: Parallel serialization to file1 and file2...");
worker.WriteTwoFilesInParallel(devices, file1, file2);
Console.WriteLine("task 1 done");

Console.WriteLine("\nTask 2: Parallel read + alternating write to file3...");
worker.ReadTwoFilesAndWriteAlternating(file1, file2, file3);
Console.WriteLine("task 2 done");

Console.WriteLine();
worker.ReadFileSingleThread(file3);

Console.WriteLine();
worker.ReadFileInTwoThreads(file3);

Console.WriteLine();
worker.ReadFileInTenThreadsWithSemaphore(file3);
