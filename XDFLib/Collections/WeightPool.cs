using System;
using System.Collections.Generic;

namespace XDFLib.Collections
{
    /// <summary>
    /// 权重池，用于处理基于权重的随机
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeightPool<T>
    {
        public const float MinWeight = 0.001f;
        public struct Entry
        {
            // 在权重链中的起始和结束标记
            public float StartMark;
            public float EndMark;
            // 对应的对象
            public T Obj;
        }

        // 权重链表，基于 _weightDict 生成，每个对象对应一个区间，区间长度与权重成正比，用于随机抽取
        AList<Entry> _weightChain;
        public ReadOnlySpan<Entry> WeightChain => _weightChain.AsReadOnlySpan();

        public float TopWeightMark => _topWeight;
        public int Count => _weightDict.Count;

        /// <summary> 权重图，key是对象，value是权重，决定该对象被随机抽中的概率 </summary>
        Dictionary<T, float> _weightDict;

        bool _needRebuildChain = false;
        float _topWeight = 0;

        public WeightPool()
        {
            _weightDict = new Dictionary<T, float>();
            _weightChain = new AList<Entry>();
        }

        public WeightPool(int capacity)
        {
            _weightDict = new Dictionary<T, float>(capacity);
            _weightChain = new AList<Entry>(capacity);
        }

        public void Clear()
        {
            _weightDict.Clear();
            _weightChain.Clear();
            _needRebuildChain = false;
            _topWeight = 0;
        }

        public bool AddWeight(T obj, float weight)
        {
            if (obj == null) return false;

            weight = MathF.Max(MinWeight, weight);
            var added = _weightDict.TryAdd(obj, weight);
            if (!added) { return false; }

            CreateAndAddNewWeightItem(obj, weight, ref _topWeight);
            return true;
        }

        public void RemoveWeight(T obj)
        {
            if (obj == null) return;

            _weightDict.Remove(obj);
            _needRebuildChain = true;
        }

        public void SetWeight(T obj, float weight)
        {
            if (obj == null) return;

            _weightDict[obj] = weight;
            _needRebuildChain = true;
        }

        public float GetWeight(T obj)
        {
            if (obj == null) return 0;

            if (_weightDict.TryGetValue(obj, out var w))
                return w;
            else
                return 0;
        }

        public float GetWeightPercent(T obj)
        {
            if (obj == null || _topWeight <= 0) { return 0; }

            if (_weightDict.TryGetValue(obj, out var w))
                return w / _topWeight;
            else
                return 0;
        }

        public void RebuildWeightChain()
        {
            _weightChain.Clear();
            _topWeight = 0;

            var iter = _weightDict.GetEnumerator();
            while (iter.MoveNext())
            {
                var weight = iter.Current.Value;
                if (weight <= 0)
                    continue;

                CreateAndAddNewWeightItem(iter.Current.Key, weight, ref _topWeight);
            }
            _needRebuildChain = false;
        }

        public T GetRandomElement()
        {
            var randRate = XRandom.SplitMix32.Random(0f, 1f);
            return GetRandomElementByRate01(randRate);
        }

        public T GetRandomElementByRate01(float randRate)
        {
            if (_needRebuildChain)
            {
                RebuildWeightChain();
            }
            if (_weightChain.Count == 0) { return default(T); }
            else if (_weightChain.Count == 1) { return _weightChain[0].Obj; }
            else
            {
                var randW = randRate * _topWeight;
                var randIndex = BinarySearch(randW);
                return _weightChain[randIndex].Obj;
            }
        }

        void CreateAndAddNewWeightItem(T obj, float weight, ref float topWeightMark)
        {
            var startMark = topWeightMark;
            topWeightMark += weight;
            Entry item = new Entry()
            {
                StartMark = startMark,
                EndMark = topWeightMark,
                Obj = obj
            };
            _weightChain.Add(item);
        }

        int BinarySearch(float weight)
        {
            var chain = WeightChain;
            if (weight > _topWeight)
            {
                return chain.Length - 1;
            }
            int l = 0;
            int r = chain.Length - 1;
            int mid = 0;
            while (l <= r)
            {
                mid = (l + r) / 2;
                ref readonly var midW = ref chain[mid];
                if (weight < midW.StartMark)
                {
                    r = mid - 1;
                }
                else if (weight > midW.EndMark)
                {
                    l = mid + 1;
                }
                else
                    return mid;
            }
            return mid;
        }
    }
}
