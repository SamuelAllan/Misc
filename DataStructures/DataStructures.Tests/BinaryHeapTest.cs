using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStructures;
using NUnit.Framework;

namespace DataStructures.Tests
{
    [TestFixture]
    public class BinaryHeapTest
    {
        private BinaryHeap<int> heap;

        [SetUp]
        public void Setup()
        {
            heap = new BinaryHeap<int>();
        }

        [Test]
        public void EnqueueThenDequeueGivesOriginalItemBack()
        {
            heap.Enqueue(5);
            int result = heap.Dequeue();
            Assert.AreEqual(5, result, "first");

            heap.Enqueue(6);
            result = heap.Dequeue();
            Assert.AreEqual(6, result, "second");
        }

        [Test]
        public void EnqueueThreeItemsThenDequeueGivesItemsBackInAscendingOrder()
        {
            heap.Enqueue(5);
            heap.Enqueue(7);
            heap.Enqueue(6);

            int first = heap.Dequeue();
            int second = heap.Dequeue();
            int third = heap.Dequeue();

            Assert.AreEqual(5, first, "first");
            Assert.AreEqual(6, second, "second");
            Assert.AreEqual(7, third, "third");
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DequeueWhenEmptyThrows()
        {
            Assert.AreEqual(0, heap.Count, "Precondition: heap should be empty");
            heap.Dequeue();
        }

        [Test]
        public void IfYouAddTwoItemsOfSameValueBothCanBeDequeued()
        {
            heap.Enqueue(5);
            heap.Enqueue(5);

            int first = heap.Dequeue();
            int second = heap.Dequeue();

            Assert.AreEqual(5, first, "first");
            Assert.AreEqual(5, second, "second");
        }

        [Test]
        public void CountIsEqualToTheNumberOfItemsEnqueuedButNotDequeued()
        {
            Assert.AreEqual(0, heap.Count, "first");

            heap.Enqueue(5);
            Assert.AreEqual(1, heap.Count, "second");

            heap.Enqueue(5);
            Assert.AreEqual(2, heap.Count, "third");

            heap.Dequeue();
            Assert.AreEqual(1, heap.Count, "fourth");

            heap.Enqueue(5);
            Assert.AreEqual(2, heap.Count, "fifth");
        }

        [Test]
        public void IsEmptyOnlyWhenThereAreNoItemsEnqueued()
        {
            Assert.IsTrue(heap.IsEmpty);
            heap.Enqueue(5);
            Assert.IsFalse(heap.IsEmpty);
            heap.Dequeue();
            Assert.IsTrue(heap.IsEmpty);
        }

        [Test]
        public void YouCanAddAnArbitrarilyLargeNumberOfItemsThenRemoveThemAllQuickly()
        {
            DateTime start = DateTime.Now;

            Random random = new Random();
            for (int i = 0; i < 10000; i++)
            {
                int item = random.Next(1, 10000);
                heap.Enqueue(item);
            }

            while (!heap.IsEmpty)
            {
                heap.Dequeue();
            }

            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;

            Console.WriteLine("Duration: {0}", duration.TotalMilliseconds);
            Assert.LessOrEqual(duration.TotalMilliseconds, 1000);
        }

        [Test]
        public void YouCanAddAnArbitrarilyLargeNumberOfItemsThenRemoveThemInAscendingOrder()
        {
            List<int> items = new List<int>();

            Random random = new Random();
            for (int i = 0; i < 20; i++)
            {
                int item = random.Next(1, 10000);
                items.Add(item);
                heap.Enqueue(item);
            }

            List<int> dequeuedItems = new List<int>();
            while (!heap.IsEmpty)
            {
                int item = heap.Dequeue();
                dequeuedItems.Add(item);
            }

            items.Sort();
            for (int i = 0; i < items.Count; i++)
            {
                Assert.AreEqual(items[i], dequeuedItems[i], "Expected {0} but got {1} at index {2}", items[i], dequeuedItems[i], i);
            }
        }

        [Test]
        public void WhenDequeueKeepsParentsLessThanChildren()
        {
            int[] items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            foreach (int item in items)
            {
                heap.Enqueue(item);
            }

            List<int> dequeuedItems = new List<int>();
            while (!heap.IsEmpty)
            {
                int item = heap.Dequeue();
                dequeuedItems.Add(item);
            }

            int[] expectedItems = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            for (int i = 0; i < expectedItems.Length; i++)
            {
                Assert.AreEqual(expectedItems[i], dequeuedItems[i], "Expected {0} but got {1} at index {2}", expectedItems[i], dequeuedItems[i], i);
            }
        }
    }
}
