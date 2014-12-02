using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures
{
    public class BinaryHeapPriorityQueue<T, P>: PriorityQueue<T, P> where P: IComparable<P>
    {
        private BinaryHeap<Item<T, P>> heap = new BinaryHeap<Item<T, P>>();
        /// <summary>
        /// The sequence number is used to make sure that all items with the same priority 
        /// come out in FIFO order. It essentially gives all items a unique "priority".
        /// If you had the queue running for a long time, you might run out of sequence
        /// numbers. But given that on currently reasonable desktop, that's about 350,000
        /// years, I'll probably be dead before that bug report comes in.
        /// 
        /// Alternatively, I could re-sequence when I run out of numbers.
        /// </summary>
        private uint sequence = 0;

        private class Item<A, B>: IComparable<Item<A, B>> where B: IComparable<B>
        {
            public readonly T item;
            public readonly P priority;
            private readonly ulong sequence;

            public Item(T item, P priority, uint sequence)
            {
                this.item = item;
                this.priority = priority;
                this.sequence = sequence;
            }

            public int CompareTo(Item<A, B> other)
            {
                int result = priority.CompareTo(other.priority);
                if (result == 0)
                    return sequence.CompareTo(other.sequence);
                return result * -1;
            }
        }

        public void Enqueue(T item, P priority)
        {
            Item<T, P> newItem = new Item<T, P>(
                item,
                priority,
                sequence);
            heap.Add(newItem);
            sequence++;
        }

        public T Dequeue()
        {
            return heap.RemoveMin().item;
        }

        public int Count
        {
            get { return heap.Count; }
        }
    }
}
