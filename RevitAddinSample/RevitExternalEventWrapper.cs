using Autodesk.Revit.UI;
using System;
using System.Diagnostics;

namespace RevitAddinSample
{
    public class RevitExternalEventWrapper : IExternalEventHandler
    {
        private readonly ExternalEvent externalEvent;
        private Action<UIApplication> action;

        public RevitExternalEventWrapper()
        {
            externalEvent = ExternalEvent.Create(this);
        }

        public void Run (Action<UIApplication> action)
        {
            this.action = action;
            var request = externalEvent.Raise();

            if (request != ExternalEventRequest.Accepted)
            {
                // Do nothing
            }
        }

        public void Execute(UIApplication app)
        {
            try
            {
                action?.Invoke(app);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public string GetName() => typeof(RevitExternalEventWrapper).Name;

        public void Dispose()
        {
            externalEvent.Dispose();
        }
    }
}
