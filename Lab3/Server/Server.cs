using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Cryptography;
using Communication;

namespace Server;

public class Server
{
    private readonly Action<double, double, double> _writer;
    
    private readonly string _clientExeFileName;
    private          int    _nextId;

    private readonly Queue<(double, double)> _queue = new();
    private readonly Mutex                   _mutex = new();

    private readonly CancellationTokenSource _cts   = new();
    private readonly CancellationToken       _token;

    public Server(string filename,
                  Action<double, double, double> writer)
    {
        _clientExeFileName = filename;
        _writer = writer;
        _token = _cts.Token;
    }

    public async void StartAsync()
    {
        while (!_token.IsCancellationRequested)
        {
            _mutex.WaitOne();
            var empty = _queue.Count == 0;
            _mutex.ReleaseMutex();
            
            if (empty)
                await Task.Delay(1, _token);
            else
            {
                _mutex.WaitOne();
                AssignTaskAsync(_queue.Dequeue());
                _mutex.ReleaseMutex();
            }
        }
    }

    public async Task EnqueueAsync(double from, double to)
    {
        await Task.Run(() =>
        {
            _mutex.WaitOne();
            _queue.Enqueue((from, to));
            _mutex.ReleaseMutex();
        }, _token);
    }

    public void Enqueue(double from, double to) => EnqueueAsync(from, to);

    private async void AssignTaskAsync((double, double) t)
    {
        var (from, to) = t;
        var controller = new ClientController(_nextId, from, to, _clientExeFileName);
        _nextId++;
        TaskCompleted(await controller.CalculateAsync());
    }

    private void TaskCompleted(ClientController controller)
    {
        var from   = controller.From;
        var to     = controller.To;
        var result = controller.Result;
        _writer(from, to, result);
    }
    
    private class ClientController
    {
        private readonly NamedPipeServerStream _stream;
        private readonly string                _clientExeFileName;
        private double                         _result;
        private readonly int                   _id;     
        
        public double From { get; }
        public double To   { get; }
        public double Result => _result;
        

        public ClientController(int id,
                                double from, double to,
                                string exeName)
        {
            if (from >= to)
                throw new ArgumentException("Wrong range!");

            From               = from;
            To                 = to;
            _clientExeFileName = exeName;
            _id = id;
            
            _stream = new NamedPipeServerStream($"lab3-{_id}");
        }

        public async Task<ClientController> CalculateAsync()
        {
            SendData();
            _result = await WaitAndGetResultAsync();

            return this;
        }

        private void SendData()
        {
            using (var client = new Process())
            {
                client.StartInfo.FileName  = _clientExeFileName;
                client.StartInfo.Arguments = $"lab3-{_id}";
                client.Start();
            }
            
            _stream.WaitForConnection();
            Communicator.DumpToPipe(_stream, new Request {From = From, To = To});
        }

        private async Task<double> WaitAndGetResultAsync()
        {
            var response = await Communicator.ReceiveAnswerAsync<Response>(_stream);
            
            return response.Result;
        }
    }
    
}
