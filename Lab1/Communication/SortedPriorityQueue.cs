using System.Collections;
using System.Collections.Specialized;

namespace Communication;

public class SortedPriorityQueue<T>
{
    private SortedDictionary<int, Queue<T>> _sortedQueues; 

    public T Dequeue()
    {
        var (priority, queue) = _sortedQueues.First();

        var value = queue.Dequeue();
        
        if (queue.Count == 0)
            _sortedQueues.Remove(priority);

        return value;
    }

    public void Enqueue(T value, int priority)
    {
        if (!_sortedQueues.ContainsKey(priority))
        {
            _sortedQueues.Add(priority, new Queue<T>());
        }
        _sortedQueues[priority].Enqueue(value);
    }
}
