using System.Xml.Serialization;

[XmlRoot("Devices")]
public class DeviceCollection
{
    [XmlElement("Device")]
    public List<Device> Items { get; set; } = new();
}
