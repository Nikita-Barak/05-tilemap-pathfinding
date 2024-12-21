using System;
using System.Collections.Generic;

/// <summary>
/// A simple priority queue implementation using a binary heap.
/// </summary>
/// <typeparam name="TElement">The type of the elements in the queue.</typeparam>
/// <typeparam name="TPriority">The type of the priority values.</typeparam>
public class PriorityQueue<TElement, TPriority> {
    private List<(TElement Element, TPriority Priority)> _heap;
    private Comparer<TPriority> _comparer;

    public PriorityQueue() {
        _heap = new List<(TElement, TPriority)>();
        _comparer = Comparer<TPriority>.Default;
    }

    public int Count => _heap.Count;

    /// <summary>
    /// Enqueues an element with a given priority.
    /// </summary>
    public void Enqueue(TElement element, TPriority priority) {
        _heap.Add((element, priority));
        BubbleUp(_heap.Count - 1);
    }

    /// <summary>
    /// Dequeues the element with the lowest priority.
    /// </summary>
    public TElement Dequeue() {
        if (Count == 0)
            throw new InvalidOperationException("The queue is empty.");
        
        var root = _heap[0];
        var last = _heap[^1];
        _heap.RemoveAt(_heap.Count - 1);
        if (_heap.Count > 0) {
            _heap[0] = last;
            BubbleDown(0);
        }
        return root.Element;
    }

    private void BubbleUp(int index) {
        while (index > 0) {
            int parentIndex = (index - 1) / 2;
            if (_comparer.Compare(_heap[index].Priority, _heap[parentIndex].Priority) >= 0)
                break;
            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void BubbleDown(int index) {
        while (true) {
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;
            int smallest = index;

            if (leftChild < _heap.Count && _comparer.Compare(_heap[leftChild].Priority, _heap[smallest].Priority) < 0)
                smallest = leftChild;

            if (rightChild < _heap.Count && _comparer.Compare(_heap[rightChild].Priority, _heap[smallest].Priority) < 0)
                smallest = rightChild;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int i, int j) {
        (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
    }
}