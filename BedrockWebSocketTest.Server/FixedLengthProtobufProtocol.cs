using Bedrock.Framework.Protocols;
using System;
using System.Buffers;
using System.Buffers.Binary;

namespace BedrockWebSocketTest.Server
{

    public class FixedLengthProtobufProtocol : IMessageReader<byte[]?>, IMessageWriter<byte[]>
    {

        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out byte[]? message)
        {
            if (input.Length == 0)
            {
                message = null;
                return false;
            }

            var reader = new SequenceReader<byte>(input);
            if (!reader.TryReadLittleEndian(out int length) || input.Length < length)
            {
                message = null;
                return false;
            }

            var payload = input.Slice(reader.Position, length);
            message = payload.ToArray();

            consumed = payload.End;
            examined = consumed;
            return true;
        }

        public void WriteMessage(byte[] message, IBufferWriter<byte> output)
        {
            var lengthBuffer = output.GetSpan(4);
            var packetPayload = message;

            BinaryPrimitives.WriteInt32LittleEndian(lengthBuffer, packetPayload.Length);
            output.Advance(4);

            output.Write(packetPayload);
        }

    }
}
