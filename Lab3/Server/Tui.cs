using System.Diagnostics;

namespace Server;

public class Tui
{
    private readonly Server _server;
    
    private readonly CancellationTokenSource _cancelInput = new();
    private readonly CancellationToken       _cancelInputToken;
    
    public Tui(string clientExe)
    {
        _cancelInputToken = _cancelInput.Token;
        _server           = new Server(clientExe, PrintResult);
    }

    public void Stop()
    {
        _cancelInput.Cancel();
    }
    
    public async Task Start()
    {
        _server.StartAsync();
        await InputThread();
    }

    private void PrintResult(double from, double to, double result)
        => Console.WriteLine($"Integral [{from}, {to}] is {result}");
    
    private async Task InputThread()
    {
        while (!_cancelInputToken.IsCancellationRequested)
        {
            try
            {
                var (from, to) = await GetInput();
                _ = _server.EnqueueAsync(from, to);
            }
            catch (TaskCanceledException)
            { }
        }
    }
    
    private async Task<(double, double)> GetInput()
    {
        while (!_cancelInputToken.IsCancellationRequested)
        {
            try
            {
                Console.Write("From: ");
                var a = Double.Parse(await ReadLineAsync(_cancelInputToken) ?? string.Empty);
                Console.Write("  To: ");
                var b = Double.Parse(await ReadLineAsync(_cancelInputToken) ?? string.Empty);
                if (a >= b)
                    throw new ArgumentOutOfRangeException();
                
                return (a, b);
            }
            catch (Exception e)
            {
                if (e is NullReferenceException or FormatException or ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Wrong input!");
                }
                else
                    throw;
            }
        }

        throw new TaskCanceledException();
    }

    private async Task<string?> ReadLineAsync(CancellationToken token = default)
    {
        try
        {
            return await Task.Run(Console.ReadLine, token);
        }
        catch (NullReferenceException)
        {
            _cancelInput.Cancel();
        }

        return null;
    }

} 