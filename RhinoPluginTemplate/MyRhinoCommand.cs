using Rhino;
using Rhino.Commands;
using Rhino.UI;

namespace RhinoPluginTemplate
{
    public class MyRhinoCommand : Command
    {
        public MyRhinoCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
            Panels.RegisterPanel(
            MyRhinoPlugin.Instance,
            typeof(MyPanelHost),
            "MyRhinoPluginTemplateWpfPanel",
            System.Drawing.SystemIcons.WinLogo,
            PanelType.System
            );
        }

        ///<summary>The only instance of this command.</summary>
        public static MyRhinoCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "MyRhinoCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Console.WriteLine("MyRhinoCommand is called.");
            var panelId = typeof(MyPanelHost).GUID;

            if (mode == RunMode.Interactive)
            {
                Panels.OpenPanel(panelId);
                RhinoApp.WriteLine("The {0} command opened panel successfully.", EnglishName);
                return Result.Success;
            }

            return Result.Success;
        }
    }
}
