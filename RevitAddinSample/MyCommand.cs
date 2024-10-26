using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace RevitAddinSample
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class MyCommand : IExternalCommand
    {
        public static MyWindow myWindow = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (myWindow == null)
            {
                myWindow = new MyWindow(commandData.Application);
                myWindow.Show();
                myWindow.Activate();
            }

            else
            {
                myWindow.Activate();
            }

            return Result.Succeeded;
        }
    }
}
