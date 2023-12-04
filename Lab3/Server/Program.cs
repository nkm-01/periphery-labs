using System.Data;
using System.IO.Pipes;
using System.Text;
using Communication;

namespace Server;

internal class Program
{
	public static async Task Main()
	{
		#if DEBUG
		const string exeName = "../../../../../Client/bin/Debug/net8.0/linux-x64/Client";
		#else
		const string exeName = "../../../../../Client/bin/Release/net8.0/linux-x64/Client";
		#endif
		var app = new Tui(exeName);
		Console.CancelKeyPress += (_, args) =>
		{
			args.Cancel = true; // Do not kill app
			app.Stop();
		};
		await app.Start();
	}
}
