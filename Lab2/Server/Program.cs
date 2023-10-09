using System.Data;
using System.IO.Pipes;
using System.Text;
using Communication;

namespace Server;

internal class Program
{
	public static void Main()
	{
		var server = new Server();
		Console.CancelKeyPress += (_, args) =>
		{
			args.Cancel = true; // Do not kill app
			server.Stop();
		};
		
		server.Start(ReadInput, 
			() => Console.WriteLine("Can't parse your input"),
			(p) => Console.WriteLine(p));
		server.WaitUntilStopped();
	}

	public static String ReadInput()
	{
		Console.Write("Data (number): ");
		var data = Console.ReadLine();
		Console.Write("Priority (empty for 0): ");
		var priority = Console.ReadLine();

		return $"{data}\n{priority}";
	}
}
