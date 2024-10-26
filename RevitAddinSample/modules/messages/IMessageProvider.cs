using System.Collections.Generic;

namespace Messages
{
    public interface IMessageProvider
    {
        bool Enabled { get; }

        void Send(
            string message,
            string title = null,
            IEnumerable<string> added = null,
            IEnumerable<string> modified = null,
            IEnumerable<string> deleted = null
        );

        void Enable(string sessionId);
        void Disable();
    }
}
