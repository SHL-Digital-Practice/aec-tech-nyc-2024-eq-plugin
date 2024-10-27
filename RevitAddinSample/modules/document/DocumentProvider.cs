using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Microsoft.Web.WebView2.Wpf;

namespace Document
{
    public class DocumentProvider
    {
        public WebView2 WebView { get; set; }

        public DocumentProvider(UIControlledApplication uiControlledApplication)
        {
            uiControlledApplication.ControlledApplication.DocumentChanged += OnDocumentChanged;
            uiControlledApplication.ControlledApplication.DocumentSaved += OnDocumentSaved;
            uiControlledApplication.ControlledApplication.DocumentOpened += OnDocumentOpened;
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
        }

        private async Task UpdateElements(DocumentChangedEventArgs e)
        {
            Autodesk.Revit.DB.Document doc = e.GetDocument();

            var addedElements = e.GetAddedElementIds();
            var modifiedElements = e.GetModifiedElementIds();
            var deletedElementsIds = e.GetDeletedElementIds().Select(id => id.ToString()).ToList();

            var addedRoomData = RoomHelpers.GetRoomDataByIds(doc, addedElements.ToList());
            var modifiedRoomData = RoomHelpers.GetRoomDataByIds(doc, modifiedElements.ToList());
            
            var tasks = new List<Task>();

            if (addedRoomData.Any())
            {
                tasks.Add(ElementProvider.CreateElementsAsync(addedRoomData));
            }

            if (modifiedRoomData.Any())
            {
                tasks.Add(ElementProvider.UpdateElementsAsync(modifiedRoomData));
            }

            if (deletedElementsIds.Any())
            {
                tasks.Add(ElementProvider.DeleteElementsAsync(deletedElementsIds));
            }

            await Task.WhenAll(tasks);

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
