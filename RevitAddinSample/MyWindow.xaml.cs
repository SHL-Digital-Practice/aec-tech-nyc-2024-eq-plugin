using Autodesk.Revit.UI;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.Windows;


namespace RevitAddinSample
{
    /// <summary>
    /// Interaction logic for MyWindow.xaml
    /// </summary>
    public partial class MyWindow : Window
    {
        private readonly RevitExternalEventWrapper myExternalEventWrapper;
        private readonly UIApplication uiApplication;

        public MyWindow(UIApplication uiApplication)
        {
            // Instantiate the event handler for external events coming from the UI.
            myExternalEventWrapper = new RevitExternalEventWrapper();

            InitializeComponent();
            InitializeAsync();
            this.uiApplication = uiApplication;
        }

        private async void InitializeAsync()
        {
            var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pluginDataFolderPath = localApplicationData + "\\.pw\\revit-plugin-template";

            var environment = await CoreWebView2Environment.CreateAsync(userDataFolder: pluginDataFolderPath);

            await webView.EnsureCoreWebView2Async(environment);
            webView.CoreWebView2.OpenDevToolsWindow();

            var bridge = new Bridge(webView, myExternalEventWrapper, uiApplication);
            webView.CoreWebView2.AddHostObjectToScript("bridge", bridge);
            webView.CoreWebView2.Navigate("http://localhost:3000");
        }

        protected override void OnClosed(EventArgs e)
        {
            myExternalEventWrapper.Dispose();
            base.OnClosed(e);
        }
    }
}
