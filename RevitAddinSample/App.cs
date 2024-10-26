using Arrow.Revit.Document;
using Autodesk.Revit.UI;
using Messages;
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

            // Create a custom ribbon tab
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch { } // Tab already exists

            // Get or Create a custom ribbon panel
            RibbonPanel panel = application.GetRibbonPanels(tabName).FirstOrDefault(panel => panel.Name == panelName);
            if (panel == null) panel = application.CreateRibbonPanel(tabName, panelName);

            // Create a push button for the command
            PushButtonData buttonData = new PushButtonData(commandName, "My Command",
                System.Reflection.Assembly.GetExecutingAssembly().Location,
                typeof(MyCommand).FullName);

            // Add the button to the panel
            panel.AddItem(buttonData);

            var messageProvider = new MessageProvider();
            var documentProvider = new DocumentProvider(application, messageProvider);

            // documentProvider.WebView = WebViewContainer.webView;


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}
