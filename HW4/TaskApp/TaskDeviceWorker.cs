using System.Text;
using System.Xml.Serialization;

public class TaskDeviceWorker
{
    private static readonly XmlSerializer Serializer = new(typeof(DeviceCollection));

    public async Task DoTask1(List<Device> devices, string file1, string file2)
    {
        var firstTen = devices.Take(10).ToList();
        var secondTen = devices.Skip(10).Take(10).ToList();

        var t1 = Task.Run(() =>
        {
            var data = new DeviceCollection { Items = firstTen };
            using var fs = new FileStream(file1, FileMode.Create, FileAccess.Write, FileShare.None);
            Serializer.Serialize(fs, data);
        });

        var t2 = Task.Run(() =>
        {
            var data = new DeviceCollection { Items = secondTen };
            using var fs = new FileStream(file2, FileMode.Create, FileAccess.Write, FileShare.None);
            Serializer.Serialize(fs, data);
        });

        await Task.WhenAll(t1, t2);
    }

    public async Task DoTask2(string file1, string file2, string file3)
    {
        var readLock = new object();
        List<Device> list1 = new();
        List<Device> list2 = new();

        var t1 = Task.Run(() =>
        {
            lock (readLock)
            {
                if (!File.Exists(file1))
                {
                    list1 = new List<Device>();
                    return;
                }

                using var fs = new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.Read);
                var data = Serializer.Deserialize(fs) as DeviceCollection;
                list1 = data?.Items ?? new List<Device>();
            }
        });

        var t2 = Task.Run(() =>
        {
            lock (readLock)
            {
                if (!File.Exists(file2))
                {
                    list2 = new List<Device>();
                    return;
                }

                using var fs = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read);
                var data = Serializer.Deserialize(fs) as DeviceCollection;
                list2 = data?.Items ?? new List<Device>();
            }
        });

        await Task.WhenAll(t1, t2);

        var sb = new StringBuilder();
        foreach (var d in list1)
        {
            sb.AppendLine($"{d.Id} | {d.Model} | {d.DeviceType}");
        }

        foreach (var d in list2)
        {
            sb.AppendLine($"{d.Id} | {d.Model} | {d.DeviceType}");
        }

        await File.WriteAllTextAsync(file3, sb.ToString());
    }

    public async Task DoTask3(string file3)
    {
        if (!File.Exists(file3))
        {
            Console.WriteLine("file3 not found");
            return;
        }

        var lines = await File.ReadAllLinesAsync(file3);
        var printLock = new object();
        var tasks = new List<Task>();

        foreach (var line in lines)
        {
            tasks.Add(Task.Run(() =>
            {
                lock (printLock)
                {
                    Console.WriteLine(line);
                }
            }));
        }

        await Task.WhenAll(tasks);
    }
}
