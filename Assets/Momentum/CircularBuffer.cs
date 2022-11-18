using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirage.Momentum
{
    // a Circular buffer that can insert and remove elements from the front and back
    // and can be iterated from the front to the back
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] buffer;
        private int start;
        private int end;

        public int Count { get; private set; }

        public CircularBuffer(int capacity)
        {
            buffer = new T[capacity];
            start = 0;
            end = 0;
            Count = 0;
        }

        public void AddFront(T item)
        {
            if (Count == buffer.Length)
            {
                throw new InvalidOperationException("CircularBuffer is full");
            }

            start = (start - 1 + buffer.Length) % buffer.Length;
            buffer[start] = item;
            Count++;
        }

        public void AddBack(T item)
        {
            if (Count == buffer.Length)
            {
                throw new InvalidOperationException("CircularBuffer is full");
            }

            buffer[end] = item;
            end = (end + 1) % buffer.Length;
            Count++;
        }

        public T RemoveFront()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("CircularBuffer is empty");
            }

            T item = buffer[start];
            start = (start + 1) % buffer.Length;
            Count--;
            return item;
        }

        public T RemoveBack()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("CircularBuffer is empty");
            }

            end = (end - 1 + buffer.Length) % buffer.Length;
            T item = buffer[end];
            Count--;
            return item;
        }

        public T Front
        {
            get
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("CircularBuffer is empty");
                }

                return buffer[start];
            }
        }

        public T Back
        {
            get
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("CircularBuffer is empty");
                }

                return buffer[(end - 1 + buffer.Length) % buffer.Length];
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return buffer[(start + index) % buffer.Length];
            }
        }

        public void Clear()
        {
            start = 0;
            end = 0;
            Count = 0;
        }

        public CircularBufferEnumerator GetEnumerator() => new CircularBufferEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public struct CircularBufferEnumerator : IEnumerator<T>
        {
            private readonly CircularBuffer<T> buffer;
            private int index;

            public CircularBufferEnumerator(CircularBuffer<T> buffer)
            {
                this.buffer = buffer;
                index = -1;
            }

            public T Current => buffer[index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
            public bool MoveNext()
            {
                index++;
                return index < buffer.Count;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
}