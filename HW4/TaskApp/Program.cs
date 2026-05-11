var basePath = AppContext.BaseDirectory;
var file1 = Path.Combine(basePath, "file1.xml");
var file2 = Path.Combine(basePath, "file2.xml");
var file3 = Path.Combine(basePath, "file3.txt");

var devices = SampleData.CreateDevices(20);
var worker = new TaskDeviceWorker();

Console.WriteLine("task 1");
await worker.DoTask1(devices, file1, file2);
Console.WriteLine("task 1 done");

Console.WriteLine();
Console.WriteLine("task 2");
await worker.DoTask2(file1, file2, file3);
Console.WriteLine("task 2 done");

Console.WriteLine();
Console.WriteLine("task 3");
await worker.DoTask3(file3);
Console.WriteLine("task 3 done");
