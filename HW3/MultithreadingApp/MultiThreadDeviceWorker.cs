using System.Diagnostics;
using System.Xml.Serialization;

public class MultiThreadDeviceWorker
{
    private static readonly XmlSerializer Serializer = new(typeof(DeviceCollection));

    public void WriteTwoFilesInParallel(List<Device> devices, string file1, string file2)
    {
        if (devices.Count < 20)
        {
            throw new ArgumentException("min 20 devices.");
        }

        var firstTen = devices.Take(10).ToList();
        var secondTen = devices.Skip(10).Take(10).ToList();

        var t1 = new Thread(() => SerializeToFile(firstTen, file1));
        var t2 = new Thread(() => SerializeToFile(secondTen, file2));

        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();
    }

    public void ReadTwoFilesAndWriteAlternating(string file1, string file2, string file3)
    {
        var list1 = DeserializeFromFile(file1);
        var list2 = DeserializeFromFile(file2);

        if (list1.Count == 0 || list2.Count == 0)
        {
            throw new InvalidOperationException("no data.");
        }

        var writerLock = new object();
        using var writer = new StreamWriter(file3, false);

        using var turn1 = new AutoResetEvent(true);
        using var turn2 = new AutoResetEvent(false);

        var t1 = new Thread(() =>
        {
            for (int i = 0; i < list1.Count; i++)
            {
                turn1.WaitOne();
                lock (writerLock)
                {
                    writer.WriteLine(ToLine(list1[i]));
                }

                turn2.Set();
            }
        });

        var t2 = new Thread(() =>
        {
            for (int i = 0; i < list2.Count; i++)
            {
                turn2.WaitOne();
                lock (writerLock)
                {
                    writer.WriteLine(ToLine(list2[i]));
                }

                turn1.Set();
            }
        });

        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();
    }

    public void ReadFileSingleThread(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("file not found");
            return;
        }

        var sw = Stopwatch.StartNew();
        Console.WriteLine("3.1 one thread");

        foreach (var line in File.ReadLines(filePath))
        {
            Console.WriteLine(line);
        }

        sw.Stop();
        Console.WriteLine($"time: {sw.ElapsedMilliseconds} ms");
    }

    public void ReadFileInTwoThreads(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("file not found");
            return;
        }

        var totalLines = File.ReadAllLines(filePath).Length;
        var middle = (totalLines + 1) / 2;
        var firstHalf = new List<string>();
        var secondHalf = new List<string>();

        var sw = Stopwatch.StartNew();

        var t1 = new Thread(() =>
        {
            using var reader = new StreamReader(filePath);
            for (int i = 0; i < middle && !reader.EndOfStream; i++)
            {
                firstHalf.Add(reader.ReadLine() ?? string.Empty);
            }
        });

        var t2 = new Thread(() =>
        {
            using var reader = new StreamReader(filePath);
            for (int i = 0; i < middle && !reader.EndOfStream; i++)
            {
                reader.ReadLine();
            }

            while (!reader.EndOfStream)
            {
                secondHalf.Add(reader.ReadLine() ?? string.Empty);
            }
        });

        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();

        sw.Stop();

        Console.WriteLine("3.2 two threads");
        Console.WriteLine("part 1:");
        foreach (var line in firstHalf)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine("part 2:");
        foreach (var line in secondHalf)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine($"time: {sw.ElapsedMilliseconds} ms");
    }

    public void ReadFileInTenThreadsWithSemaphore(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("file not found");
            return;
        }

        var semaphore = new Semaphore(5, 5);
        var outputLock = new object();
        var threads = new List<Thread>();

        var sw = Stopwatch.StartNew();

        for (int i = 1; i <= 10; i++)
        {
            var threadIndex = i;
            var thread = new Thread(() =>
            {
                semaphore.WaitOne();
                try
                {
                    var lines = File.ReadAllLines(filePath);
                    lock (outputLock)
                    {
                        var first = lines.Length > 0 ? lines[0] : "";
                        Console.WriteLine($"thread {threadIndex}: lines={lines.Length}, first='{first}'");
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            threads.Add(thread);
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        sw.Stop();
        Console.WriteLine($"3.3 ten threads, semaphore(5), time: {sw.ElapsedMilliseconds} ms");
    }

    private static void SerializeToFile(List<Device> devices, string path)
    {
        var data = new DeviceCollection { Items = devices };
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        Serializer.Serialize(fs, data);
    }

    private static List<Device> DeserializeFromFile(string path)
    {
        if (!File.Exists(path))
        {
            return new List<Device>();
        }

        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        var data = Serializer.Deserialize(fs) as DeviceCollection;
        return data?.Items ?? new List<Device>();
    }

    private static string ToLine(Device device) => $"{device.Id} | {device.Model} | {device.DeviceType}";
}
