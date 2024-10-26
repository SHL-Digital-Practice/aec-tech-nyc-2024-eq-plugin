using System.Linq;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Messages;
using Microsoft.Web.WebView2.Wpf;

namespace Arrow.Revit.Document
{
    public class DocumentProvider
    {
        private readonly IMessageProvider messageProvider;
        public WebView2 WebView { get; set; }

        public DocumentProvider(UIControlledApplication uiControlledApplication, IMessageProvider messageProvider)
        {
            uiControlledApplication.ControlledApplication.DocumentChanged += OnDocumentChanged;
            this.messageProvider = messageProvider;
        }

        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            // if (!messageProvider.Enabled) { return; }

            var addedElementIds = e.GetAddedElementIds();
            var modifiedElementIds = e.GetModifiedElementIds();
            var deletedElementIds = e.GetDeletedElementIds();

            var message = $"{addedElementIds.Count} elements added. {modifiedElementIds.Count} elements modified. {deletedElementIds.Count} elements deleted.";

            messageProvider.Send(
                message,
                "Model Changed",
                addedElementIds.Select(i => i.ToString()),
                modifiedElementIds.Select(i => i.ToString()),
                deletedElementIds.Select(i => i.ToString())
            );



            SendMessageToWebView();
        }

        private void SendMessageToWebView()
        {
            if (WebView == null) { return; }

            WebView.CoreWebView2.ExecuteScriptAsync($"console.log('hello from native');");
        }
    }
}