using System.IO.Pipes;
using Communication;

namespace Server;

internal class Program
{
	public static void Main()
	{
		var stream = Establish("test");
		Communicator.DumpToPipe(stream, new Payload{Id = 123, Answer = false});
		Console.WriteLine($"Answer: {Communicator.ReceiveAnswer(stream)}");
	}

	private static NamedPipeServerStream Establish(String pipe)
	{
		var stream = new NamedPipeServerStream("test-stream", PipeDirection.InOut);
		stream.WaitForConnection();
		
		Console.WriteLine("Client connected!");
		
		return stream;
	}
}
