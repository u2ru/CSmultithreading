using System.Xml.Serialization;

public class Device
{
    [XmlAttribute("Id")]
    public string Id { get; set; } = string.Empty;

    [XmlAttribute("Model")]
    public string Model { get; set; } = string.Empty;

    [XmlAttribute("DeviceType")]
    public DeviceType DeviceType { get; set; }
}
