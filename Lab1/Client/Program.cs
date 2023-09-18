using System.IO.Pipes;
using Communication;

namespace Client;

class Program
{
	public static void Main()
	{
		Console.WriteLine("Connecting to server...");
		var stream = Connect("test");
		
		Console.WriteLine("Reading data...");
		var answer = Communicator.ReceiveAnswer(stream);
		Console.WriteLine($"Received: {answer}");
		
		answer.Answer = true;
		Console.WriteLine($"Sending {answer}...");
		Communicator.DumpToPipe(stream, answer);
	}

	private static NamedPipeClientStream Connect(String pipe)
	{
		var stream = new NamedPipeClientStream(".", "test-stream", PipeDirection.InOut);
		stream.Connect();
		Console.WriteLine("Connected to server!");
		
		return stream;
	}
}

