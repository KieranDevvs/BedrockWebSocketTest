using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;

namespace BedrockWebSocketTest.Server
{

    public class SessionHandler : ConnectionHandler
    {
        private WebSocketProtocol? _protocol;

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            _protocol = WebSocketProtocol.CreateFromConnection(connection, false, null, TimeSpan.FromSeconds(30));

            if (_protocol.WebSocket.State == WebSocketState.Open)
            {
                var payload = Encoding.UTF8.GetBytes($"This is a {string.Join(" ", Enumerable.Range(0, 20).Select(x => "really"))} long message.");
                await _protocol.WriteAsync(payload, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
    }
}
