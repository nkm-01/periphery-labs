using System.IO.Pipes;
using Communication;

namespace Client;

class Program
{
	public static void Main()
	{
		var stream = Connect("test");
		var answer = Communicator.ReceiveAnswer(stream);
		Console.WriteLine($"Answer: {answer}");
		answer.Answer = true;
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

