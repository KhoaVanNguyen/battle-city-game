using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleCity
{
    public class BinaryHeap<T> where T : IComparable<T>
    {
        private List<T> heap;

        public BinaryHeap()
        {
            this.heap = new List<T>();
        }

        public void Push(T item)
        {
            heap.Add(item);
            int ci = heap.Count - 1;
            while (ci > 0)
            {
                int pi = (ci - 1) / 2;
                if (heap[ci].CompareTo(heap[pi]) >= 0) break;
                T tmp = heap[ci]; heap[ci] = heap[pi]; heap[pi] = tmp;
                ci = pi;
            }
        }

        public T Pop()
        {
            int li = heap.Count - 1;
            T frontItem = heap[0];
            heap[0] = heap[li];
            heap.RemoveAt(li);
            --li;
            int pi = 0;
            while (true)
            {
                int ci = pi * 2 + 1;
                if (ci > li) break;
                int rc = ci + 1;
                if (rc <= li && heap[rc].CompareTo(heap[ci]) < 0)
                    ci = rc;
                if (heap[pi].CompareTo(heap[ci]) <= 0) break;
                T tmp = heap[pi]; heap[pi] = heap[ci]; heap[ci] = tmp;
                pi = ci;
            }
            return frontItem;
        }

        public int Count()
        {
            return heap.Count;
        }
    }
}
