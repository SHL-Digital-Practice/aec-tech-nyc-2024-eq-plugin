using Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Messages
{
    public class MessageProvider: IMessageProvider
    {
        private readonly SocketIOClient.SocketIO _socket;
        private string _sessionId;
        public bool Enabled { get; private set; } = false;

        public MessageProvider(string sessionId = null)
        {
            _socket = new SocketIOClient.SocketIO("http://localhost:3001/");
            _socket.OnConnected += OnConnected;
            _socket.OnDisconnected += OnDisconnected;
            if (sessionId != null)
            {
                Enable(sessionId);
            }
        }

        public void Enable(string sessionId)
        {
            _sessionId = sessionId;
            _socket.ConnectAsync();
            Enabled = true;
        }

        public void Disable()
        {
            _sessionId = null;
            _socket.DisconnectAsync();
            Enabled = false;
        }

        public void Send(string message, string title = null, IEnumerable<string> added = null, IEnumerable<string> modified = null, IEnumerable<string> deleted = null)
        {
            if (!_socket.Connected || _sessionId == null) { return; }

            var messageObject = new { title, message, data = new { created = added, updated = modified, deleted } };
            var serialized = JsonConvert.SerializeObject(messageObject);
            _socket.EmitAsync("model:updated", serialized);
        }

        private void OnDisconnected(object sender, string e)
        {
            Debug.WriteLine("Disconnected from server.");
        }

        private void OnConnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Connected to server.");
        }

    }
}
