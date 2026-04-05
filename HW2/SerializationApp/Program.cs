var xmlPath = Path.Combine(AppContext.BaseDirectory, "devices.xml");
var devices = new List<Device>();
var xmlService = new XmlService();

while (true)
{
    PrintMenu();
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            devices = SampleData.CreateDevices(10);
            PrintDevices(devices);
            break;
        case "2":
            if (devices.Count == 0)
            {
                devices = SampleData.CreateDevices(10);
            }

            xmlService.Serialize(xmlPath, devices);
            Console.WriteLine($"Saved to: {xmlPath}");
            break;
        case "3":
            PrintRawXml(xmlPath);
            break;
        case "4":
            devices = xmlService.Deserialize(xmlPath);
            if (devices.Count == 0)
            {
                Console.WriteLine("No data loaded.");
            }
            else
            {
                PrintDevices(devices);
            }

            break;
        case "5":
            PrintModels(xmlService.GetModelsWithXDocument(xmlPath), "XDocument");
            break;
        case "6":
            PrintModels(xmlService.GetModelsWithXmlDocument(xmlPath), "XmlDocument");
            break;
        case "7":
            ChangeAttribute(xmlService.ChangeAttributeWithXDocument, xmlPath, "XDocument");
            break;
        case "8":
            ChangeAttribute(xmlService.ChangeAttributeWithXmlDocument, xmlPath, "XmlDocument");
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }
}

void PrintMenu()
{
    Console.WriteLine("\nMenu:");
    Console.WriteLine("1. Create 10 objects and print");
    Console.WriteLine("2. Serialize objects to XML");
    Console.WriteLine("3. Read XML file");
    Console.WriteLine("4. Deserialize objects");
    Console.WriteLine("5. Find Model values (XDocument)");
    Console.WriteLine("6. Find Model values (XmlDocument)");
    Console.WriteLine("7. Change attribute (XDocument)");
    Console.WriteLine("8. Change attribute (XmlDocument)");
    Console.WriteLine("0. Exit");
    Console.Write("Choose: ");
}

void PrintDevices(List<Device> list)
{
    for (int i = 0; i < list.Count; i++)
    {
        var d = list[i];
        Console.WriteLine($"{i + 1}. Id={d.Id}, Model={d.Model}, DeviceType={d.DeviceType}");
    }
}

void PrintRawXml(string path)
{
    if (!File.Exists(path))
    {
        Console.WriteLine("XML file not found.");
        return;
    }

    Console.WriteLine(File.ReadAllText(path));
}

void PrintModels(List<string> models, string sourceName)
{
    if (models.Count == 0)
    {
        Console.WriteLine("No models found.");
        return;
    }

    Console.WriteLine($"Models ({sourceName}):");
    for (int i = 0; i < models.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {models[i]}");
    }
}

void ChangeAttribute(Func<string, string, int, string, bool> changer, string path, string sourceName)
{
    if (!File.Exists(path))
    {
        Console.WriteLine("XML file not found.");
        return;
    }

    Console.Write("Attribute name: ");
    var name = Console.ReadLine() ?? string.Empty;

    Console.Write("Element number (1..n): ");
    var numberText = Console.ReadLine();
    if (!int.TryParse(numberText, out var number) || number < 1)
    {
        Console.WriteLine("Wrong number.");
        return;
    }

    Console.Write("New value: ");
    var newValue = Console.ReadLine() ?? string.Empty;

    var ok = changer(path, name, number, newValue);
    Console.WriteLine(ok ? $"Updated with {sourceName}." : "Update failed.");
}
