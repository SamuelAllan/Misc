using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStructures;
using NUnit.Framework;

namespace DataStructures.Tests
{
    [TestFixture]
    public class BinaryHeapPriorityQueueTest
    {
        private PriorityQueue<int, int> queue;

        [SetUp]
        public void Setup()
        {
            queue = new BinaryHeapPriorityQueue<int, int>();
        }

        [Test]
        public void EnqueueLowThenHighDequeueHighThenLow()
        {
            queue.Enqueue(1, 1);
            queue.Enqueue(2, 2);
            Assert.AreEqual(2, queue.Dequeue(), "first");
            Assert.AreEqual(1, queue.Dequeue(), "second");
        }

        [Test]
        public void EnqueueSamePriorityDequeueFIFO()
        {
            for (int i = 1; i <= 10; i++)
            {
                queue.Enqueue(i, 1);
            }
            for (int i = 1; i <= 10; i++)
            {
                Assert.AreEqual(i, queue.Dequeue(), i);
            }
        }

        [Test, Ignore("Takes too long to run")]
        public void HowManyEnqueueDequeueInOneMinute()
        {
            DateTime start = DateTime.Now;
            DateTime end = start.AddSeconds(60);
            ulong i = 0;
            while (end > DateTime.Now)
            {
                queue.Enqueue(0, 1);
                queue.Dequeue();
                i++;
            }
            Console.WriteLine("Interations in a minute: " + i);

            ulong minutes = ulong.MaxValue / i;
            ulong years = minutes / 60 / 24 / 365;
            Console.WriteLine("Years: " + years);
        }

        [Test]
        public void Count()
        {
            Assert.AreEqual(0, queue.Count, "zero");
            queue.Enqueue(1, 1);
            Assert.AreEqual(1, queue.Count, "one");
            queue.Enqueue(2, 2);
            Assert.AreEqual(2, queue.Count, "two");
            queue.Dequeue();
            Assert.AreEqual(1, queue.Count, "three");
            queue.Dequeue();
            Assert.AreEqual(0, queue.Count, "four");
        }
    }
}
