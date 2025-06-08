using System.Collections.Generic;
using UnityEngine;

    public class PriorityQueue
    {
        private readonly List<BuffInstance> _heap = new List<BuffInstance>();

        public int Count => _heap.Count;

        public void Enqueue(BuffInstance item)
        {
            if (item == null || item.Buff == null)
                return;

            if (_heap.Count >= 20)
            {
                Debug.LogWarning($"[PriorityQueue] Cannot add '{item.Buff.buffName}'. Maximum number of buffs (20) reached.");
                return;
            }

            if (!item.Buff.isStackable)
            {
                foreach (var existing in _heap)
                {
                    if (existing.Buff == item.Buff)
                    {
                        Debug.Log($"[PriorityQueue] Buff '{item.Buff.buffName}' already active and is not stackable. Skipping enqueue.");
                        return; // Prevent adding if a non-stackable buff is already present
                    }
                }
            }

            _heap.Add(item);
            HeapifyUp(_heap.Count - 1);
        }

        public BuffInstance Dequeue()
        {
            if (_heap.Count == 0) return null;
            var root = _heap[0];
            _heap[0] = _heap[^1];
            _heap.RemoveAt(_heap.Count - 1);
            HeapifyDown(0);
            return root;
        }

        public bool Contains(BuffInstance item)
        {
            return _heap.Contains(item);
        }

        public BuffInstance Peek()
        {
            if (_heap.Count == 0) return null;
            return _heap[0];
        }

        public void Awake()
        {
            Debug.Log($"[PriorityQueue] Awaking {_heap.Count} buffs.");
            for (var i = 0; i < _heap.Count; i++)
            {
                _heap[i].Awake();
            }
        }
        
        public void Start()
        {
            Debug.Log($"[PriorityQueue] Starting {_heap.Count} buffs.");
            for (var i = 0; i < _heap.Count; i++)
            {
                _heap[i].Start();
            }
        }

        public void Update(float deltaTime, bool debug = false)
        {
            if(debug)Debug.Log($"[PriorityQueue] Updating {_heap.Count} buffs.");
            for (var i = _heap.Count - 1; i >= 0; i--)
            {
                Debug.Log($"[PriorityQueue] Updating BuffInstance {i}: {_heap[i].Buff.buffName}");
                _heap[i].Update(deltaTime);
                if (_heap[i].IsExpired())
                {
                    Debug.Log($"[PriorityQueue] BuffInstance {i} ({_heap[i].Buff.buffName}) expired. Removing.");
                    RemoveAt(i);
                }
            }
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (_heap[index].timeRemaining >= _heap[parentIndex].timeRemaining) break;
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = _heap.Count - 1;
            while (true)
            {
                var leftChild = 2 * index + 1;
                var rightChild = 2 * index + 2;
                var smallest = index;

                if (leftChild <= lastIndex && _heap[leftChild].timeRemaining < _heap[smallest].timeRemaining)
                    smallest = leftChild;
                if (rightChild <= lastIndex && _heap[rightChild].timeRemaining < _heap[smallest].timeRemaining)
                    smallest = rightChild;
                if (smallest == index) break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        }

        private void RemoveAt(int index)
        {
            if (index >= _heap.Count) return;
            _heap[index] = _heap[^1];
            _heap.RemoveAt(_heap.Count - 1);
            HeapifyDown(index);
        }

        public void RemoveBuff(Buff buff)
        {
            for (var i = 0; i < _heap.Count; i++)
            {
                if (_heap[i].Buff == buff)
                {
                    RemoveAt(i);
                    return;
                }
            }
        }
        public List<BuffInstance> GetAllBuffs()
        {
            return new List<BuffInstance>(_heap);
        }
        
    }
