using System;
using System.Collections;
using System.Collections.Generic;

public class HashDeque<T> : ICollection, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
{
    private LinkedList<T> deque;
    private HashSet<T> hashSet;

    public int Count => hashSet.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public HashDeque()
    {
        deque = new LinkedList<T>();
        hashSet = new HashSet<T>();
    }

    public HashDeque(IEnumerable<T> collection)
    {
        deque = new LinkedList<T>(collection);
        hashSet = new HashSet<T>(collection);
    }

    public void Clear()
    {
        deque.Clear();
        hashSet.Clear();
    }

    public bool AddFirst(T item)
    {
        if (hashSet.Add(item))
        {
            deque.AddFirst(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AddLast(T item)
    {
        if(hashSet.Add(item))
        {
            deque.AddLast(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public T RemoveFirst()
    {
        T item = deque.First.Value;
        deque.RemoveFirst();
        if (!hashSet.Remove(item))
        {
            throw new Exception("HashDeque in inconsistent state");
        }
        return item;
    }

    public T GetFirst()
    {
        return deque.First.Value;
    }

    public T RemoveLast()
    {
        T item = deque.Last.Value;
        deque.RemoveFirst();
        if (!hashSet.Remove(item))
        {
            throw new Exception("HashDeque in inconsistent state");
        }
        return item;
    }

    public T GetLast()
    {
        return deque.Last.Value;
    }

    public bool Contains(T item)
    {
        return hashSet.Contains(item);
    }

    public void CopyTo(T[] array, int index)
    {
        deque.CopyTo(array, index);
    }

    public void CopyTo(Array array, int index)
    {
        ((ICollection)deque).CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
        return deque.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return deque.GetEnumerator();
    }
}