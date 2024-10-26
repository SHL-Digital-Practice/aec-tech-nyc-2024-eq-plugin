using Arrow.Revit.Shared;
using Bridge;
using Messages;
using System.Runtime.InteropServices;


namespace Messages
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class MessagesBridge : IBridge
    {
        private readonly IMessageProvider messageProvider;

        public MessagesBridge(IMessageProvider messageProvider)
        {
            this.messageProvider = messageProvider;
        }

        public bool Connect(string sessionId)
        {
            messageProvider.Enable(sessionId);
            return true;
        }

        public void Disconnect()
        {
            messageProvider.Disable();
        }
    }
}
