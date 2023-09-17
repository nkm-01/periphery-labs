using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Communication;

public struct Payload
{
	public          int    Id         { get; set; }
	public          bool   Answer     { get; set; }
	public override string ToString() => $"Id = {Id}, Answer = {Answer}";
}

public static class Communicator
{
	public static void DumpToPipe(PipeStream stream, Payload data)
	{
		unsafe
		{
			var array = new Payload[1];
			array[0] = data;
			var bytesSpan = MemoryMarshal.AsBytes(new Span<Payload>(array));
			stream.Write(bytesSpan);
		}
		Console.WriteLine($"Sent: {data}");
	}

	public static Payload ReceiveAnswer(PipeStream stream)
	{
		var data = new Payload[1];
		data[0] = new Payload();
		var span = MemoryMarshal.AsBytes(new Span<Payload>(data));
		stream.ReadExactly(span);

		return data[0];
	}
}
