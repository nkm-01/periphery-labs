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
		byte[] array = new byte[Unsafe.SizeOf<Payload>()];
		MemoryMarshal.Write(array, ref data);
		stream.Write(array);
	}

	public static Payload ReceiveAnswer(PipeStream stream)
	{
		byte[] array = new byte[Unsafe.SizeOf<Payload>()];
		_ = stream.Read(array,0,array.Length);
		return MemoryMarshal.Read<Payload>(array);
	}
}
