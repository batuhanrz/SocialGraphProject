using System;
using Xunit;
using SocialGraph.API.DataStructures;

namespace SocialGraph.Tests
{
    /// <summary>
    /// CustomQueue (Circular Array) birim testleri.
    /// Ozcan tarafindan Sprint 1.2'de yazilan veri yapisinin dogrulanmasi.
    /// </summary>
    public class CustomQueueTests
    {
        [Fact]
        public void Enqueue_Dequeue_ShouldMaintainFIFOOrder()
        {
            var queue = new CustomQueue<int>();

            queue.Enqueue(10);
            queue.Enqueue(20);
            queue.Enqueue(30);

            Assert.Equal(10, queue.Dequeue());
            Assert.Equal(20, queue.Dequeue());
            Assert.Equal(30, queue.Dequeue());
        }

        [Fact]
        public void Dequeue_OnEmptyQueue_ShouldThrowException()
        {
            var queue = new CustomQueue<int>();

            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        }

        [Fact]
        public void DynamicResize_ShouldHandleOver1000Elements()
        {
            var queue = new CustomQueue<int>(8);

            for (int i = 0; i < 1200; i++)
            {
                queue.Enqueue(i);
            }

            Assert.Equal(1200, queue.Count);

            for (int i = 0; i < 1200; i++)
            {
                Assert.Equal(i, queue.Dequeue());
            }

            Assert.True(queue.IsEmpty);
        }
    }
}
