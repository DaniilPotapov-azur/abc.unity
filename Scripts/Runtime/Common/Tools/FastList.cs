using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace abc.unity.Common
{
    [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
    public class ActorFastList<T>
    {
        private T[] _items = Array.Empty<T>();

        public int Count { get; private set; }

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _items.Length;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array.Resize(ref _items, value);
        }

        public Span<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Span<T>(_items, 0, Count);
        }

        public ReadOnlySpan<T> ReadOnlySpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new ReadOnlySpan<T>(_items, 0, Count);
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _items[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            EnsureCapacity(Count + 1);

            _items[Count++] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            var index = IndexOf(item);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            Count -= 1;

            if (index < Count)
                Array.Copy(_items, index + 1, _items, index, Count - index);

            _items[Count] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item) => Array.IndexOf(_items, item, 0, Count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, T item)
        {
            EnsureCapacity(Count + 1);

            if (index < Count)
                Array.Copy(_items, index, _items, index + 1, Count - index);

            _items[index] = item;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(T item) => BinarySearch(0, Count, item, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(T item, IComparer<T> comparer) => BinarySearch(0, Count, item, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) => Array.BinarySearch(_items, index, count, item, comparer ?? Comparer<T>.Default);

        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            var freeIndex = 0;

            while (freeIndex < Count && !match(_items[freeIndex])) 
                freeIndex++;

            if (freeIndex >= Count) 
                return 0;

            var current = freeIndex + 1;

            while (current < Count)
            {
                while (current < Count && match(_items[current])) current++;

                if (current < Count)
                    _items[freeIndex++] = _items[current++];
            }

            Array.Clear(_items, freeIndex, Count - freeIndex);
            var result = Count - freeIndex;
            Count = freeIndex;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int min)
        {
            if (Capacity < min)
                Capacity = MathHelpers.NextPowerOf2(min);
        }
    }
}