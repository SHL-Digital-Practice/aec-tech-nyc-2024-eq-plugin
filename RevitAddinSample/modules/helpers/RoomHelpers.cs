using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Collections.Generic;
using System.Linq;

public static class RoomHelpers
{
    public static List<RoomData> GetAllRoomData(Autodesk.Revit.DB.Document doc)
    {
        if (doc == null) return new List<RoomData>();
        
        return new FilteredElementCollector(doc)
            .OfClass(typeof(SpatialElement))
            .WhereElementIsNotElementType()
            .OfType<Room>()
            .Select(room => new RoomData(
                room.Id.ToString(),
                room.Area,
                room.Name,
                room.LookupParameter("Department")?.AsString() ?? string.Empty
            ))
            .ToList();
    }

    public static RoomData GetRoomDataById(Autodesk.Revit.DB.Document doc, ElementId roomId)
    {
        if (roomId == null) return null;

        var room = new FilteredElementCollector(doc)
            .WherePasses(new ElementIdSetFilter(new HashSet<ElementId> { roomId }))
            .OfType<Room>()
            .FirstOrDefault();

        return room != null 
            ? new RoomData(
                room.Id.ToString(), 
                room.Area, 
                room.Name, 
                room.LookupParameter("Department")?.AsString() ?? string.Empty
            ) 
            : null;
    }

    public static List<RoomData> GetRoomDataByIds(Autodesk.Revit.DB.Document doc, List<ElementId> roomIds)
    {
        if (roomIds == null || !roomIds.Any()) return new List<RoomData>();

        var filter = new ElementIdSetFilter(new HashSet<ElementId>(roomIds));
        return new FilteredElementCollector(doc)
            .WherePasses(filter)
            .OfType<Room>()
            .Select(room => new RoomData(
                room.Id.ToString(),
                room.Area,
                room.Name,
                room.LookupParameter("Department")?.AsString() ?? string.Empty
            ))
            .ToList();
    }
}
