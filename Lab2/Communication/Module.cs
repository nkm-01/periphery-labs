using System.Collections;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace Communication;

public struct Payload
{
	public          int    Data       { get; set; }
	public          bool   Answer     { get; set; }
	public override string ToString() => $"Id = {Data}, Answer = {Answer}";
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
		int    bytesRead  = 0;
		while (bytesRead < array.Length)
		{
			if (!stream.IsConnected)
				throw new ChannelClosedException();
			bytesRead += stream.Read(array, bytesRead, array.Length - bytesRead);
			if (bytesRead < array.Length)
				Thread.Sleep(1);
		}

		return MemoryMarshal.Read<Payload>(array);
	}

	public static IEnumerable<Payload> ReceiveAll(PipeStream stream)
	{
		while(stream.IsConnected)
		{
			Payload msg;
			try
			{
				msg = ReceiveAnswer(stream);
			}
			catch (ChannelClosedException)
			{
				yield break;
			}
			yield return msg;
		}
	}
	
	public static void PrintAllAnswers(PipeStream stream)
	{
		foreach (var data in Communicator.ReceiveAll(stream))
		{
			Console.WriteLine(data);
		}
	}
}
