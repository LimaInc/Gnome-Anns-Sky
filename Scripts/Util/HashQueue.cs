using System;
using System.Collections;
using System.Collections.Generic;

public class HashQueue<T> : ICollection, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
{
    private Queue<T> queue;
    private HashSet<T> hashSet;

    public int Count => hashSet.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public HashQueue()
    {
        queue = new Queue<T>();
        hashSet = new HashSet<T>();
    }

    public void Clear()
    {
        queue.Clear();
        hashSet.Clear();
    }

    public bool Enqueue(T item)
    {
        if(hashSet.Add(item))
        {
            queue.Enqueue(item);
            return true;
        }
        else
            return false;
    }

    public T Dequeue()
    {
        T item = queue.Dequeue();
        hashSet.Remove(item);

        return item;
    }

    public T Peek()
    {
        return queue.Peek();
    }

    public bool Contains(T item)
    {
        return hashSet.Contains(item);
    }

    public void CopyTo(T[] array, int index)
    {
        queue.CopyTo(array, index);
    }

    public void CopyTo(Array array, int index)
    {
        ((ICollection)queue).CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
        return queue.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return queue.GetEnumerator();
    }
}