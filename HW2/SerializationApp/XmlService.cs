using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

public class XmlService
{
    public void Serialize(string path, List<Device> devices)
    {
        var serializer = new XmlSerializer(typeof(DeviceCollection));
        var data = new DeviceCollection { Items = devices };
        using var fs = new FileStream(path, FileMode.Create);
        serializer.Serialize(fs, data);
    }

    public List<Device> Deserialize(string path)
    {
        if (!File.Exists(path))
        {
            return new List<Device>();
        }

        var serializer = new XmlSerializer(typeof(DeviceCollection));
        using var fs = new FileStream(path, FileMode.Open);
        var obj = serializer.Deserialize(fs) as DeviceCollection;
        return obj?.Items ?? new List<Device>();
    }

    public List<string> GetModelsWithXDocument(string path)
    {
        if (!File.Exists(path))
        {
            return new List<string>();
        }

        var result = new List<string>();
        var doc = XDocument.Load(path);
        var devices = doc.Descendants("Device");

        foreach (var device in devices)
        {
            var model = device.Attribute("Model")?.Value;
            if (!string.IsNullOrWhiteSpace(model))
            {
                result.Add(model);
            }
        }

        return result;
    }

    public List<string> GetModelsWithXmlDocument(string path)
    {
        if (!File.Exists(path))
        {
            return new List<string>();
        }

        var result = new List<string>();
        var doc = new XmlDocument();
        doc.Load(path);
        var nodes = doc.SelectNodes("//Device");

        if (nodes == null)
        {
            return result;
        }

        foreach (XmlNode node in nodes)
        {
            var model = node.Attributes?["Model"]?.Value;
            if (!string.IsNullOrWhiteSpace(model))
            {
                result.Add(model);
            }
        }

        return result;
    }

    public bool ChangeAttributeWithXDocument(string path, string attributeName, int elementNumber, string newValue)
    {
        var doc = XDocument.Load(path);
        var element = doc.Descendants("Device").ElementAtOrDefault(elementNumber - 1);
        if (element == null)
        {
            return false;
        }

        var attr = element.Attribute(attributeName);
        if (attr == null)
        {
            return false;
        }

        attr.Value = newValue;
        doc.Save(path);
        return true;
    }

    public bool ChangeAttributeWithXmlDocument(string path, string attributeName, int elementNumber, string newValue)
    {
        var doc = new XmlDocument();
        doc.Load(path);
        var nodes = doc.SelectNodes("//Device");

        if (nodes == null || elementNumber < 1 || elementNumber > nodes.Count)
        {
            return false;
        }

        var node = nodes[elementNumber - 1];
        var attr = node?.Attributes?[attributeName];
        if (attr == null)
        {
            return false;
        }

        attr.Value = newValue;
        doc.Save(path);
        return true;
    }
}
