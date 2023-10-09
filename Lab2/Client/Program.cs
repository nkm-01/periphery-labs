using System.IO.Pipes;
using Communication;

namespace Client;

class Program
{
	public static void Main()
	{
		Console.WriteLine("Connecting to server...");
		var stream = Connect("peripheral-lab2");
		Console.WriteLine("Connected to server!");

		Console.WriteLine("Reading messages...");
		foreach (var payload in Communicator.ReceiveAll(stream))
		{
			Console.WriteLine($"Received: {payload}");
			var answerPayload = payload;
			answerPayload.Answer = true;
			Communicator.DumpToPipe(stream, answerPayload);
		}
		
		Console.WriteLine("Server disconnected");
	}

	private static NamedPipeClientStream Connect(String pipe)
	{
		var stream = new NamedPipeClientStream(".", pipe, PipeDirection.InOut);
		stream.Connect();
		
		return stream;
	}
}

