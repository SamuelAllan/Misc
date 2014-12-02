using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures
{
    public class BinaryHeap<T>: IEnumerable<T> where T : IComparable<T>
    {
        private const int initialCapacity = 64;
        private const int resizeMultiplier = 2;

        private int count;
        private T[] items;
        
        public BinaryHeap()
        {
            count = 0;
            items = new T[initialCapacity];
        }

        public void Add(T item)
        {
            if (NeedToExpandItems())
            {
                ExpandItems();
            }
            items[count] = item;
            BubbleUp(count);
            count++;
        }

        private bool NeedToExpandItems()
        {
            return count == items.Length;
        }

        private void ExpandItems()
        {
            int newSize = items.Length * resizeMultiplier;
            T[] newItems = new T[newSize];
            for (int i = 0; i < items.Length; i++)
            {
                newItems[i] = items[i];
            }
            items = newItems;
        }

        private void BubbleUp(int index)
        {
            if (index <= 0)
                return;

            int parentIndex = (index - 1) / 2;
            if (items[index].CompareTo(items[parentIndex]) < 0)
            {
                SwapItems(index, parentIndex);
                BubbleUp(parentIndex);
            }
        }

        private void SwapItems(int indexA, int indexB)
        {
            T temp = items[indexA];
            items[indexA] = items[indexB];
            items[indexB] = temp;
        }

        public T RemoveMin()
        {
            if (count == 0)
                throw new InvalidOperationException("Heap empty.");

            T result = items[0];
            count--;
            items[0] = items[count];
            BubbleDown(0);
            items[count] = default(T); // Remove reference to object to prevent memory leak.
            return result;
        }

        private void BubbleDown(int index)
        {
            int leftChild = GetLeftChildIndex(index);
            int rightChild = GetRightChildIndex(index);
            int lastIndex = count - 1;

            if (leftChild > lastIndex)
            {
                return;
            }
            if (rightChild > lastIndex)
            {
                if (items[leftChild].CompareTo(items[index]) < 0)
                    SwapItems(index, leftChild);
                return;
            }
            int minChildIndex = IndexWithMinItem(leftChild, rightChild);
            if (items[minChildIndex].CompareTo(items[index]) < 0)
            {
                SwapItems(minChildIndex, index);
                BubbleDown(minChildIndex);
            }
        }

        private int IndexWithMinItem(int indexA, int indexB)
        {
            if (items[indexA].CompareTo(items[indexB]) < 0)
                return indexA;
            return indexB;
        }

        private static int GetLeftChildIndex(int parentIndex)
        {
            return parentIndex * 2 + 1;
        }

        private static int GetRightChildIndex(int parentIndex)
        {
            return parentIndex * 2 + 2;
        }

        public int Count
        {
            get { return count; }
        }

        public bool IsEmpty
        {
            get { return count == 0; }
        }

        public T Min()
        {
            if (count == 0)
                throw new InvalidOperationException("Heap empty.");
            return items[0];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return items[i];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}