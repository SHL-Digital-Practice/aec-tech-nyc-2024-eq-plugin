using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using Rhino;
using Rhino.Geometry;
using System.Runtime.InteropServices;
using UserControl = System.Windows.Controls.UserControl;

namespace RhinoPluginTemplate
{
    /// <summary>
    /// Interaction logic for MyWindow.xaml
    /// </summary>
    public partial class MyWindow : UserControl
    {
        public MyWindow()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pluginDataFolderPath = localApplicationData + "\\.pw\\rhino-plugin-template";

            var environment = await CoreWebView2Environment.CreateAsync(null, pluginDataFolderPath, null);

            await webView.EnsureCoreWebView2Async(environment);
            webView.CoreWebView2.OpenDevToolsWindow();
            webView.CoreWebView2.AddHostObjectToScript("bridge", new MyRhinoBridge());
            webView.CoreWebView2.Navigate("https://localhost:3000");
        }
    }
}
