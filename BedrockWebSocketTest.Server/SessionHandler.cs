using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;

namespace BedrockWebSocketTest.Server
{

    public class SessionHandler : ConnectionHandler
    {
        private readonly FixedLengthProtobufProtocol _protocol;

        public SessionHandler()
        {
            _protocol = new FixedLengthProtobufProtocol();
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var reader = connection.CreateReader();
            var writer = connection.CreateWriter();

            var payload = Encoding.UTF8.GetBytes($"This is a {string.Join(" ", Enumerable.Range(0, 14).Select(x => "really"))} long message.");
            await writer.WriteAsync(_protocol, payload);

            reader.Advance();
        }
    }
}