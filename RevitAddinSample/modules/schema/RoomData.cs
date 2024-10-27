using System.Collections.Generic;

public class RoomData
{
    public string ApplicationId { get; set; }
    public string Type { get; set; } = "room";
    public Dictionary<string, object> Properties { get; set; } = [];

    public RoomData(string applicationId, double area)
    {
        ApplicationId = applicationId;
        Properties["area"] = area;
    }

    public void AddProperty(string key, object value)
    {
        Properties[key] = value;
    }
}
