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
        /// The problem is that if you had the queue running for a long time, you might run
        /// out of sequence numbers.
        /// 
        /// There is another problem that you might run out of memory, but that is probably
        /// gonna be fatal anyways.
        /// </summary>
        private uint sequence = 0;

        private class Item<A, B>: IComparable<Item<A, B>> where B: IComparable<B>
        {
            public readonly T item;
            public readonly P priority;
            private readonly uint sequence;

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
            if (RunOutOfSequenceNumbers())
            {
                Resequence();
            }
            Item<T, P> newItem = new Item<T, P>(
                item,
                priority,
                sequence);
            heap.Enqueue(newItem);
            sequence++;
        }

        private bool RunOutOfSequenceNumbers()
        {
            return sequence == uint.MaxValue;
        }

        /// <summary>
        /// We're compacting the sequence numbers. This isn't strictly following
        /// the old sequence order, but all items with the same priority will have
        /// relatively identical sequence numbers, and as the sequence number is
        /// supposed to differentiate at that level, I reckon it's okay. I think it
        /// also results in no bubbling up during insert, as everything is inserted
        /// first to last, so that makes the resequence O(log(N)), based on re-sizing
        /// the new BinaryHeap's internal array.
        /// </summary>
        private void Resequence()
        {
            uint newSequence = 0;
            BinaryHeap<Item<T, P>> newHeap = new BinaryHeap<Item<T, P>>();
            while (!heap.IsEmpty)
            {
                Item<T, P> oldItem = heap.Dequeue();
                Item<T, P> newItem = new Item<T, P>(
                    oldItem.item,
                    oldItem.priority,
                    newSequence);
                newSequence++;
                newHeap.Enqueue(newItem);
            }
            heap = newHeap;
            sequence = newSequence;
        }

        public T Dequeue()
        {
            return heap.Dequeue().item;
        }
    }
}
