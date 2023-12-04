using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Communication;

public struct Request
{
	public          double From       { get; set; }
	public          double To         { get; set; }
	public override string ToString() => $"Request: [{From}, {To}]";
}

public struct Response
{
	public          double Result     { get; set; }
	public override string ToString() => $"Response: {Result}";
}

public static class Communicator
{
	public static void DumpToPipe<TStruct>(PipeStream stream, TStruct data) where TStruct:struct
	{
		byte[] array = new byte[Unsafe.SizeOf<TStruct>()];
		Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference<byte>(array), data);
		stream.Write(array);
	}

	public static async Task<TStruct> ReceiveAnswerAsync<TStruct>(PipeStream stream) where TStruct:struct
	{
		byte[] array     = new byte[Unsafe.SizeOf<TStruct>()];
		int    bytesRead = 0;
		while (bytesRead < array.Length)
		{
			bytesRead += await stream.ReadAsync(array, bytesRead, array.Length - bytesRead);
			if (bytesRead < array.Length)
				Thread.Sleep(1);
		}

		return MemoryMarshal.Read<TStruct>(array);
	}

	public static TStruct ReceiveAnswer<TStruct>(PipeStream stream) where TStruct : struct
		=> ReceiveAnswerAsync<TStruct>(stream).GetAwaiter().GetResult();

}
