using System.IO.Pipes;
using Communication;

namespace Client;

class Program
{
	private static bool _isLoggingEnabled = false;
	
	public static int Main(string[] args)
	{
		if (args.Length != 1 && args.Length != 2)
		{
			Console.Error.WriteLine("You must pass pipe name as an argument. Consider launching via server.");
			
            return 1;
		}
  
		if (args is [_, "--verbose"])
		{
			_isLoggingEnabled = true;
		}
		
		Log("Connecting to server...");
		var stream = Connect(args[0]);
		Log("Connected to server!");
  
		var request = Communicator.ReceiveAnswer<Request>(stream);
		var result = Integrate(request.From, request.To);
		
		Log($"Result: {result}. Sending...");
		Communicator.DumpToPipe(stream, new Response {Result = result});
		Log("Sent. Client will be closed.");
		
		return 0;
	}

	private static double Integrate(double from, double to)
	{
		double Func(double x) => 3 * Math.Pow(x, 3);
		var step = to - from < 0.1 ? (to - from) / 1000.0 : 0.0001;

		var sum = 0.0;
		
		for (var (a, b) = (from, from + step); b < to; a += step, b += step)
		{
			Log($"{a}...{b} :: {sum}");
			sum += (Func(a) + Func(b)) / 2.0 * step;
		}

		return sum;
	}
	
	private static void Log(string str)
	{
		if (_isLoggingEnabled)
		{
			Console.WriteLine(str);
		}
	}
	
	private static NamedPipeClientStream Connect(String pipe)
	{
		var stream = new NamedPipeClientStream(".", pipe, PipeDirection.InOut);
		stream.Connect();
		
		return stream;
	}
}

