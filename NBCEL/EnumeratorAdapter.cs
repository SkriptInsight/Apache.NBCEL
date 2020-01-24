using System;
using System.Collections;
using System.Collections.Generic;

namespace Apache.NBCEL
{
// // Mimics Java's Iterable<T> interface
    public interface IIterable<T>
    {
        IIterator<T> Iterator();
    }

// Mimics Java's Iterator interface - but
// implements IDisposable for the sake of
// parity with IEnumerator.
    public interface IIterator<T> : IDisposable
    {
        bool HasNext { get; }
        T Next();
        void Remove();
    }

    public sealed class EnumerableAdapter<T> : IIterable<T>
    {
        private readonly IEnumerable<T> enumerable;

        public EnumerableAdapter(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }

        public IIterator<T> Iterator()
        {
            return new EnumeratorAdapter<T>(enumerable.GetEnumerator());
        }
    }

    public sealed class EnumeratorAdapter<T> : IIterator<T>
    {
        private readonly IEnumerator<T> enumerator;

        private bool fetchedNext;
        private T next;
        private bool nextAvailable;

        public EnumeratorAdapter(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public EnumeratorAdapter(HashSet<T>.Enumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        public bool HasNext
        {
            get
            {
                CheckNext();
                return nextAvailable;
            }
        }

        public T Next()
        {
            CheckNext();
            if (!nextAvailable) throw new InvalidOperationException();

            fetchedNext = false; // We've consumed this now
            return next;
        }

        public void Remove()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            enumerator.Dispose();
        }

        private void CheckNext()
        {
            if (!fetchedNext)
            {
                nextAvailable = enumerator.MoveNext();
                if (nextAvailable) next = enumerator.Current;

                fetchedNext = true;
            }
        }
    }

    public sealed class IterableAdapter<T> : IEnumerable<T>
    {
        private readonly IIterable<T> iterable;

        public IterableAdapter(IIterable<T> iterable)
        {
            this.iterable = iterable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new IteratorAdapter<T>(iterable.Iterator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public sealed class IteratorAdapter<T> : IEnumerator<T>
    {
        private readonly IIterator<T> iterator;
        private T current;

        private bool gotCurrent;

        public IteratorAdapter(IIterator<T> iterator)
        {
            this.iterator = iterator;
        }

        public T Current
        {
            get
            {
                if (!gotCurrent) throw new InvalidOperationException();

                return current;
            }
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            gotCurrent = iterator.HasNext;
            if (gotCurrent) current = iterator.Next();

            return gotCurrent;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            iterator.Dispose();
        }
    }
}