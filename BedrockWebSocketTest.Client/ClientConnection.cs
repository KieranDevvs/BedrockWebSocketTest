using System.Buffers;
using System.Buffers.Binary;
using System.Net.WebSockets;

public class ClientConnection
{
    private readonly ClientWebSocket _websocket;

    public ClientConnection(ClientWebSocket websocket)
    {
        _websocket = websocket;
    }

    public Task ConnectAsync(Uri websocketUrl, CancellationToken cancellationToken = default)
    {
        return _websocket.ConnectAsync(websocketUrl, cancellationToken);
    }

    public async Task ListenAsync(CancellationToken cancellationToken = default)
    {
        var lengthBuffer = new byte[4];
        var lengthResult = await _websocket.ReceiveAsync(lengthBuffer, cancellationToken);

        var payloadLendth = BitConverter.ToInt32(lengthBuffer);

        var buffer = new byte[payloadLendth];

        var result = await _websocket.ReceiveAsync(buffer, cancellationToken);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            return;
        }

        Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer));
    }

    private static ArraySegment<byte> SerializePacket(byte[] packet)
    {
        var buffer = new ArrayBufferWriter<byte>();

        var lengthBuffer = buffer.GetSpan(4);
        var packetPayload = packet;

        BinaryPrimitives.WriteInt32LittleEndian(lengthBuffer, packetPayload.Length);
        buffer.Advance(4);

        buffer.Write(packetPayload);

        var a = buffer.WrittenMemory.ToArray();
        return a;
    }

    public bool IsConnected()
    {
        return _websocket.State == WebSocketState.Open;
    }

    public Task SendAsync(byte[] packet, CancellationToken cancellationToken = default)
    {
        return _websocket.SendAsync(SerializePacket(packet), WebSocketMessageType.Binary, endOfMessage: true, cancellationToken);
    }
}