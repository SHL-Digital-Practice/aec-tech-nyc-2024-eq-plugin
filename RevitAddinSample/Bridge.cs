using Autodesk.Revit.UI;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using PWPluginSamples.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RevitAddinSample
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class Bridge: IBridge
    {
        private readonly WebView2 webView;
        private readonly RevitExternalEventWrapper externalEvent;
        private readonly UIApplication uiApplication;

        public Bridge(WebView2 webView, RevitExternalEventWrapper externalEvent, UIApplication uiApplication)
        {
            this.webView = webView;
            this.externalEvent = externalEvent;
            this.uiApplication = uiApplication;
            webView.CoreWebView2.WebMessageReceived += WebMessageReceived;
        }

        private void WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            Debug.WriteLine(e.WebMessageAsJson);
        }

        public string CreatePerson(string name, int age)
        {
            var person = new { name, age };
            return JsonConvert.SerializeObject(person);
        }

        public void DoSomething(double radius = 10)
        {
            MyExternalEventController.DoSomething(externalEvent, uiApplication, radius);
        }

        public string GetGreeting()
        {
            return "Hello from C#!";

        }

        public string GetMessage(string message)
        {
            return "Message: " + message;
        }

        [System.Runtime.CompilerServices.IndexerName("Items")]
        public string this[int index]
        {
            get { return m_dictionary[index]; }
            set { m_dictionary[index] = value; }
        }
        private Dictionary<int, string> m_dictionary = new Dictionary<int, string>();
    }
}
