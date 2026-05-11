public static class SampleData
{
    public static List<Device> CreateDevices(int count)
    {
        var list = new List<Device>();
        var types = Enum.GetValues<DeviceType>();

        for (int i = 1; i <= count; i++)
        {
            list.Add(new Device
            {
                Id = $"DEV-{1000 + i}",
                Model = $"Model-{i}",
                DeviceType = types[(i - 1) % types.Length]
            });
        }

        return list;
    }
}
