using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures
{
    public interface PriorityQueue<T, P> where P: IComparable<P>
    {
        void Enqueue(T item, P priority);
        T Dequeue();
        int Count { get; }
    }
}
