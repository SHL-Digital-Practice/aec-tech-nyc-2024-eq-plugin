using System.Runtime.InteropServices;

namespace RhinoPluginTemplate
{
    [Guid("8B604987-9E66-4B7A-BE4C-F3FC550FBD65")]
    public class MyPanelHost: RhinoWindows.Controls.WpfElementHost
    {
        public MyPanelHost(uint docSn) : base (new MyWindow(), null)
        {
            
        }
    }
}
