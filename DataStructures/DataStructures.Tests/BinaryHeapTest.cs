using System;
using System.Collections.Generic;
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
        public void AddThenRemoveMinGivesOriginalItemBack()
        {
            heap.Add(5);
            int result = heap.RemoveMin();
            Assert.AreEqual(5, result, "first");

            heap.Add(6);
            result = heap.RemoveMin();
            Assert.AreEqual(6, result, "second");
        }

        [Test]
        public void AddThreeItemsThenRemoveMinGivesItemsBackInAscendingOrder()
        {
            heap.Add(5);
            heap.Add(7);
            heap.Add(6);

            int first = heap.RemoveMin();
            int second = heap.RemoveMin();
            int third = heap.RemoveMin();

            Assert.AreEqual(5, first, "first");
            Assert.AreEqual(6, second, "second");
            Assert.AreEqual(7, third, "third");
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void RemoveMinWhenEmptyThrows()
        {
            Assert.AreEqual(0, heap.Count, "Precondition: heap should be empty");
            heap.RemoveMin();
        }

        [Test]
        public void IfYouAddTwoItemsOfSameValueBothCanBeRemoved()
        {
            heap.Add(5);
            heap.Add(5);

            int first = heap.RemoveMin();
            int second = heap.RemoveMin();

            Assert.AreEqual(5, first, "first");
            Assert.AreEqual(5, second, "second");
        }

        [Test]
        public void CountIsEqualToTheNumberOfItemsAddedButNotRemoved()
        {
            Assert.AreEqual(0, heap.Count, "first");

            heap.Add(5);
            Assert.AreEqual(1, heap.Count, "second");

            heap.Add(5);
            Assert.AreEqual(2, heap.Count, "third");

            heap.RemoveMin();
            Assert.AreEqual(1, heap.Count, "fourth");

            heap.Add(5);
            Assert.AreEqual(2, heap.Count, "fifth");
        }

        [Test]
        public void IsEmptyOnlyWhenThereAreNoItemsAdded()
        {
            Assert.IsTrue(heap.IsEmpty);
            heap.Add(5);
            Assert.IsFalse(heap.IsEmpty);
            heap.RemoveMin();
            Assert.IsTrue(heap.IsEmpty);
        }

        [Test]
        public void YouCanAddAnArbitrarilyLargeNumberOfItemsThenRemoveThemAllQuickly()
        {
            const int numberOfItems = 100000;
            const int maxValue = 100000;

            DateTime start = DateTime.Now;
            
            Random random = new Random();
            for (int i = 0; i < numberOfItems; i++)
            {
                int item = random.Next(1, maxValue);
                heap.Add(item);
            }

            while (!heap.IsEmpty)
            {
                heap.RemoveMin();
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
                heap.Add(item);
            }

            List<int> removedItems = new List<int>();
            while (!heap.IsEmpty)
            {
                int item = heap.RemoveMin();
                removedItems.Add(item);
            }

            items.Sort();
            for (int i = 0; i < items.Count; i++)
            {
                Assert.AreEqual(items[i], removedItems[i], "Expected {0} but got {1} at index {2}", items[i], removedItems[i], i);
            }
        }

        [Test]
        public void WhenRemoveMinKeepsParentsLessThanChildren()
        {
            int[] items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            foreach (int item in items)
            {
                heap.Add(item);
            }

            List<int> dequeuedItems = new List<int>();
            while (!heap.IsEmpty)
            {
                int item = heap.RemoveMin();
                dequeuedItems.Add(item);
            }

            int[] expectedItems = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            for (int i = 0; i < expectedItems.Length; i++)
            {
                Assert.AreEqual(expectedItems[i], dequeuedItems[i], "Expected {0} but got {1} at index {2}", expectedItems[i], dequeuedItems[i], i);
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void MinWhenEmptyThrows()
        {
            Assert.AreEqual(0, heap.Count, "Precondition: heap should be empty");
            heap.Min();
        }

        [Test]
        public void MinAlwaysReturnsTheMinimumItemContained()
        {
            heap.Add(3);
            Assert.AreEqual(3, heap.Min(), "A");

            heap.Add(5);
            Assert.AreEqual(3, heap.Min(), "B");

            heap.Add(2);
            Assert.AreEqual(2, heap.Min(), "C");

            heap.RemoveMin();
            Assert.AreEqual(3, heap.Min(), "D");

            heap.RemoveMin();
            Assert.AreEqual(5, heap.Min(), "E");            
        }

        public void MinDoesNotChangeTheContents()
        {
            heap.Add(3);
            heap.Add(5);
            heap.Add(2);
            Assert.AreEqual(2, heap.Min(), "A");
            Assert.AreEqual(2, heap.Min(), "B");
            Assert.AreEqual(2, heap.Min(), "C");
            CollectionAssert.AreEquivalent(new[] { 2, 3, 5 }, heap, "D");
        }

        [Test]
        public void IsEnumerable()
        {
            CollectionAssert.AreEquivalent(new int[0], heap, "Empty");

            int[] items = new[] { 10, 7, 4, 3, 6, 2, 1, 8 };
            foreach (int item in items)
            {
                heap.Add(item);
            }

            CollectionAssert.AreEquivalent(items, heap, "Full");
        }
    }
}
