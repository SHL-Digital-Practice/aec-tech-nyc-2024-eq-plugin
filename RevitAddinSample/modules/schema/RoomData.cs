using System.Collections.Generic;

public class RoomData
{
    public string ApplicationId { get; set; }
    public string Type { get; set; } = "room";
    public Dictionary<string, object> Properties { get; set; } = [];

     public RoomData(string applicationId, double area, string name, string department)
    {
        ApplicationId = applicationId;
        Properties["area"] = area;
        Properties["name"] = name;
        Properties["department"] = department;
    }


    public void AddProperty(string key, object value)
    {
        Properties[key] = value;
    }
}
