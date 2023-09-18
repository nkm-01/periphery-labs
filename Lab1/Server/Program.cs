using System.IO.Pipes;
using Communication;

namespace Server;

internal class Program
{
	public static void Main()
	{
		var data   = new Payload { Id = 123, Answer = false };
		
		Console.WriteLine("Awaiting client...");
		var stream = Establish("test");
		
		Console.WriteLine($"Sending {data} to client...");
		Communicator.DumpToPipe(stream, data);
		
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
