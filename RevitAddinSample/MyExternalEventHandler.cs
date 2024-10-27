using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace RevitAddinSample
{
    public class MyExternalEventController
    {
        internal static void DoSomething(RevitExternalEventWrapper externalEvent, UIApplication uiApplication, double radius = 10)
        {
            externalEvent.Run(app =>
            {
               Autodesk.Revit.DB.Document doc = uiApplication.ActiveUIDocument.Document;

                // Define the start and end points for the wall
                XYZ startPoint = new XYZ(0, 0, 0); // Start at the origin
                XYZ endPoint = new XYZ(UnitUtils.ConvertToInternalUnits(10, UnitTypeId.Meters), 0, 0); // 10 meters along the X-axis

                // First level
                Level level = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .FirstElement() as Level;

                // Start a transaction to modify the document
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Create Wall");

                    // Create a new wall
                    Wall.Create(doc, Line.CreateBound(startPoint, endPoint), level.Id, false);

                    // Commit the transaction
                    tx.Commit();
                }

            });
        }
    }
}
