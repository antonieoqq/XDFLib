using System;
using System.Collections.Generic;
using System.Numerics;

namespace XDFLib.Collections
{
    /// <summary>
    /// 权重池，用于处理基于权重的随机
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeightPool<T>
    {
        /// <summary>
        /// 权重图，key是对象，value是权重，决定该对象被随机抽中的概率
        /// </summary>
        Dictionary<T, float> _weightDict = new Dictionary<T, float>();

        // 随机表，基于权重图生成了2个List，_weightChain是基于权重连起来的概率分布区间表，每个区间对应_objChain表中的一个对象
        // 这2个List用于随机抽取过程时的查询，在权重图发生改变后，使用时要Rebuild
        List<Vector2> _weightChain = new List<Vector2>();
        List<T> _objChain = new List<T>();
        public float TopWeightMark => TopWeightMark;
        public int Count => _weightDict.Count;

        bool _shouldRebuildRandomChains = false;
        float _topWeightMark = 0;

        public void Clear()
        {
            _weightDict.Clear();
            _weightChain.Clear();
            _objChain.Clear();
            _shouldRebuildRandomChains = false;
            _topWeightMark = 0;
        }

        public void AddWeight(T obj, float weight)
        {
            weight = MathF.Max(0.001f, weight);
            _weightDict[obj] = weight;

            _shouldRebuildRandomChains = true;
        }

        public void RemoveWeight(T obj)
        {
            _weightDict.Remove(obj);
            _shouldRebuildRandomChains = true;
        }

        public void SetWeight(T obj, float weight)
        {
            float w = 0;
            if (_weightDict.TryGetValue(obj, out w))
            {
                _weightDict[obj] = weight;
                _shouldRebuildRandomChains = true;
            }
        }

        public float GetWeight(T obj)
        {
            if (_weightDict.TryGetValue(obj, out var w))
                return w;
            else
                return 0;
        }

        public float GetWeightPercent(T obj)
        {
            if (TopWeightMark <= 0) { return 0; }

            if (_weightDict.TryGetValue(obj, out var w))
                return w / _topWeightMark;
            else
                return 0;
        }

        public void ForEachWeight(Action<T, float> act)
        {
            foreach (var w in _weightDict)
            {
                act(w.Key, w.Value);
            }
        }

        public void RebuildRandomChains()
        {
            _weightChain.Clear();
            _objChain.Clear();

            var iter = _weightDict.GetEnumerator();
            _topWeightMark = 0;
            while (iter.MoveNext())
            {
                if (iter.Current.Value > 0)
                {
                    var prevTop = _topWeightMark;
                    _topWeightMark += iter.Current.Value;
                    _weightChain.Add(new Vector2(prevTop, _topWeightMark));
                    _objChain.Add(iter.Current.Key);
                }
            }
            _shouldRebuildRandomChains = false;
        }

        public T GetRandomElement()
        {
            var randRate = XRandom.SplitMix32.Random(0f, 1f);
            return GetRandomElementByRate01(randRate);
        }

        public T GetRandomElementByRate01(float randRate)
        {
            if (_shouldRebuildRandomChains)
            {
                RebuildRandomChains();
            }
            if (_objChain.Count == 0) { return default(T); }
            else if (_objChain.Count == 1) { return _objChain[0]; }
            else
            {
                var randW = randRate * _topWeightMark;
                var randIndex = BinarySearch(randW);
                return _objChain[randIndex];
            }
        }

        int BinarySearch(float weight)
        {
            if (weight > _topWeightMark)
            {
                return _weightChain.Count - 1;
            }
            int l = 0;
            int r = _weightChain.Count - 1;
            int mid = 0;
            while (l <= r)
            {
                mid = (l + r) / 2;
                var midW = _weightChain[mid];
                if (weight < midW.X)
                {
                    r = mid - 1;
                }
                else if (weight > midW.Y)
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
