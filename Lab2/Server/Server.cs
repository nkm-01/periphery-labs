using System.IO.Pipes;
using System.Runtime.InteropServices.JavaScript;
using Communication;

namespace Server;

public class Server
{
    #region Private Fields 
    private SortedPriorityQueue<Payload> _queue = new();
    private NamedPipeServerStream        _stream;
    private CancellationToken            _stopReadingToken;
    private CancellationTokenSource      _stopReadingCancellation;
    
    private Task _inputThread;
    private Task _processThread;
    
    private Mutex _queueMutex = new();
    #endregion

    public void Start(Func<String>    getElement,
                      Action          atGetParseError,
                      Action<Payload> atAnswer)
    {
        Establish("peripheral-lab2");

        _stopReadingCancellation = new();
        _stopReadingToken        = _stopReadingCancellation.Token;

        Status = ServerStatus.READING;

        _inputThread   = Task.Run(() => ReaderTask(getElement, atGetParseError), _stopReadingToken);
        _processThread = Task.Run(() => SenderTask(atAnswer), _stopReadingToken);
    }

    public void WaitUntilStopped()
    {
        Task.WaitAll(_inputThread, _processThread);
    }
    
    public void Stop()
    {
        Status = ServerStatus.SENDING_LAST;
        _stopReadingCancellation.Cancel();
    }

    public ServerStatus Status { get; private set; } = ServerStatus.STOPPED;

    public enum ServerStatus
    {
        STOPPED,
        READING,
        SENDING_LAST
    };
        
    #region Private Methods
    private void SenderTask(Action<Payload> writeData)
    {
        _stream.WaitForConnection();
        while (_queue.Count > 0 || Status != ServerStatus.STOPPED)
        {
            _queueMutex.WaitOne();
            var     hasElements = _queue.Count > 0;
            Payload payload = new();
            if (hasElements) 
                payload   = _queue.Dequeue();
            _queueMutex.ReleaseMutex();
            
            if (hasElements)
            {
                Communicator.DumpToPipe(_stream, payload);
                writeData(Communicator.ReceiveAnswer(_stream));
            }
            else
            {
                if (Status == ServerStatus.SENDING_LAST)
                    Status = ServerStatus.STOPPED;
                Thread.Sleep(1);
            }
        }
        
    }

    private void ReaderTask(Func<String> getElement, Action atError)
    {
        while (!_stopReadingToken.IsCancellationRequested)
        {
            var (data, priority, isValid) = ParseInput(getElement());
            if (!isValid)
            {
                atError();
                continue;
            }

            _queueMutex.WaitOne();
            _queue.Enqueue(new Payload {Data = data, Answer = false}, 
                priority);
            _queueMutex.ReleaseMutex();
        }
    }

    /// <summary>
    /// Parse string to retrieve data and priority from it
    /// </summary>
    /// <param name="input">
    ///     String to parse in [data]\n[priority] format;
    ///     priority can be omitted (but new line cannot)
    /// </param>
    /// <returns>
    ///     Tuple from 3 elements.
    ///     First: data,
    ///     second: priority,
    ///     third: is data valid
    /// </returns>
    private static (int, int, bool) ParseInput(String input)
    {
        var error = (0, 0, false);
        var words = input.Split('\n');

        if (words.Length != 1 && words.Length != 2)
            return error;
        
        if (!Int32.TryParse(words[0], out var data))
            return error;

        if (!Int32.TryParse(words[1], out var priority))
            priority = 0;

        return (data, priority, true);
    }
    
    private void Establish(String pipe)
    {
        _stream = new NamedPipeServerStream(pipe, PipeDirection.InOut);
        Console.WriteLine("Awaiting client...");
    }
    #endregion
}
