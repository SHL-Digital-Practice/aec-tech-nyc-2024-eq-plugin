using Newtonsoft.Json;
using Rhino.Geometry;
using Rhino;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace RhinoPluginTemplate
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class MyRhinoBridge : PWPluginSamples.Shared.IBridge
    {
        public string GetGreeting()
        {
            return "Hello from C#!";
        }

        public string GetMessage(string message)
        {
            return "Message: " + message;
        }

        public string CreatePerson(string name, int age)
        {
            var person = new { name, age };
            return JsonConvert.SerializeObject(person);
        }

        public void DoSomething(double radius = 10)
        {
            Sphere sphere = new Sphere(new Point3d(0, 0, 0), radius);
            RhinoDoc.ActiveDoc.Objects.AddSphere(sphere);


            // Zoom to extents rhino active viewport
            RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ZoomExtents();
        }

        // Sample indexed property.
        [System.Runtime.CompilerServices.IndexerName("Items")]
        public string this[int index]
        {
            get { return m_dictionary[index]; }
            set { m_dictionary[index] = value; }
        }
        private Dictionary<int, string> m_dictionary = new Dictionary<int, string>();
    }
}
