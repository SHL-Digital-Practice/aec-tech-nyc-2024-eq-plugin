using Autodesk.Revit.UI;
using Document;
using System;
using System.Linq;

namespace RevitAddinSample
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "PW";
            string panelName = "Tools";
            string commandName = "MyCommand";

            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch { } 

            RibbonPanel panel = application.GetRibbonPanels(tabName).FirstOrDefault(p => p.Name == panelName);
            if (panel == null) panel = application.CreateRibbonPanel(tabName, panelName);

            PushButtonData buttonData = new PushButtonData(commandName, "My Command",
                System.Reflection.Assembly.GetExecutingAssembly().Location,
                typeof(MyCommand).FullName);

            panel.AddItem(buttonData);

            var documentProvider = new DocumentProvider(application);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
