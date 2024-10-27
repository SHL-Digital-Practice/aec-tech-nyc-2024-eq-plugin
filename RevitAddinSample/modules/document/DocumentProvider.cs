using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Messages;
using Microsoft.Web.WebView2.Wpf;

namespace Document
{
    public class DocumentProvider
    {
        private readonly IMessageProvider messageProvider;
        public WebView2 WebView { get; set; }

        public DocumentProvider(UIControlledApplication uiControlledApplication, IMessageProvider messageProvider)
        {
            uiControlledApplication.ControlledApplication.DocumentChanged += OnDocumentChanged;
            uiControlledApplication.ControlledApplication.DocumentSaved += OnDocumentSaved;
            uiControlledApplication.ControlledApplication.DocumentOpened += OnDocumentOpened;

            this.messageProvider = messageProvider;
        }

        private async void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            await InitialSync(e.Document);
        }

        private void OnDocumentSaved(object sender, DocumentSavedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            await UpdateElements(e);
            SendMessageToWebView();
        }

        public async Task InitialSync(Autodesk.Revit.DB.Document doc)
        {
            var roomDataList = RoomHelpers.GetAllRoomData(doc);
            await ElementProvider.CreateElementsAsync(roomDataList);
            messageProvider.Send("Initial sync completed with all room IDs and areas.", "Initial Sync", roomDataList.Select(r => r.ApplicationId).ToList());
        }

        private async Task UpdateElements(DocumentChangedEventArgs e)
        {
            Autodesk.Revit.DB.Document doc = e.GetDocument();

            var addedRoomData = RoomHelpers.GetRoomDataByIds(doc, e.GetAddedElementIds().ToList());
            var modifiedRoomData = RoomHelpers.GetRoomDataByIds(doc, e.GetModifiedElementIds().ToList());
            var deletedRoomIds = RoomHelpers.GetRoomDataByIds(doc, e.GetDeletedElementIds().ToList()).Select(r => r.ApplicationId).ToList();
            
            var tasks = new List<Task>();

            if (addedRoomData.Any())
            {
                tasks.Add(ElementProvider.CreateElementsAsync(addedRoomData));
            }

            if (modifiedRoomData.Any())
            {
                tasks.Add(ElementProvider.UpdateElementsAsync(modifiedRoomData));
            }

            if (deletedRoomIds.Any())
            {
                tasks.Add(ElementProvider.DeleteElementsAsync(deletedRoomIds));
            }

            await Task.WhenAll(tasks);

            var message = $"{addedRoomData.Count} elements added, {modifiedRoomData.Count} elements modified, {deletedRoomIds.Count} elements deleted.";
            messageProvider.Send(message, "Model Changed", addedRoomData.Select(r => r.ApplicationId).ToList(), modifiedRoomData.Select(r => r.ApplicationId).ToList(), deletedRoomIds);
        }

        private void SendMessageToWebView()
        {
            if (WebView?.CoreWebView2 != null)
            {
                WebView.CoreWebView2.ExecuteScriptAsync("console.log('hello from native');");
            }
        }
    }
}
