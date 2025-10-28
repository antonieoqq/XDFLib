using System;
using System.Collections.Generic;
using XDFLib.Extensions;

namespace XDFLib.Collections
{
    public class LevelQueue
    {
        // 等级是否0基，否的话等级最低从1开始算，等级 = 已获得等级 + baseLevel(0或1)
        bool _isZeroBased = false;
        public bool IsZeroBased
        {
            get { return _isZeroBased; }
            set { _isZeroBased = value; }
        }

        public int BaseLevel => IsZeroBased ? 0 : 1;

        bool _clampMaxTotalScore = true;
        public bool ClampMaxTotalScore
        {
            get { return _clampMaxTotalScore; }
            set
            {
                if (_clampMaxTotalScore != value)
                {
                    _clampMaxTotalScore = value;
                    TotalScore = TotalScore;
                }
            }
        }

        int _totalScore = 0;
        public int TotalScore
        {
            get { return _totalScore; }
            set
            {
                var validTotalScore = ClampMaxTotalScore ? XMath.Clamp(value, 0, TopLevelTotalScore) : value;
                if (_totalScore != validTotalScore)
                {
                    _totalScore = validTotalScore;
                    var newL = _totalScore >= TopLevelTotalScore ? TopLevelTotalScore : GetLevelAchievedBySocre(_totalScore);
                    if (newL != _levelAchieved)
                    {
                        _levelAchieved = newL;
                        _onLevelChanged?.Invoke(CurrentLevel);
                    }
                }
            }
        }

        int _levelAchieved = 0;
        public int LevelAchieved
        {
            get { return XMath.Clamp(_levelAchieved, 0, XMath.Max(0, _levelScores.Count)); }
            set
            {
                var newV = XMath.Clamp(value, 0, XMath.Max(0, _levelScores.Count));
                if (_levelAchieved != newV)
                {
                    _levelAchieved = value;

                    if (CurrentLevel >= TopLevel) { _totalScore = TopLevelTotalScore; }
                    else { _totalScore = _levelTotalScores[_levelAchieved]; }
                    _onLevelChanged?.Invoke(CurrentLevel);
                }
            }
        }

        /// <summary>当前等级</summary>
        public int CurrentLevel { get { return LevelAchieved + BaseLevel; } }

        /// <summary>在当前等级基础上超出的分数</summary>
        public int ScoreOverCurrentLevel { get { return TotalScore - _levelTotalScores[LevelAchieved]; } }

        /// <summary>当前等级的总分门槛</summary>
        public int TotalScoreOfCurrentLevel { get { return _levelTotalScores[LevelAchieved]; } }

        /// <summary>从第一级到达下一级所需要的总分</summary>
        public int ScoreFrom0ToNextLevel { get { return CurrentLevel == TopLevel ? 0 : _levelTotalScores[LevelAchieved + 1]; } }

        /// <summary>从本级到达下一级所需要的分数</summary>
        public int ScoreFromThisToNextLevel { get { return CurrentLevel == TopLevel ? 0 : ScoreFrom0ToNextLevel - TotalScoreOfCurrentLevel; } }

        /// <summary>距离到达下一级所剩的分数</summary>
        public int ScoreRemainToLevelUp { get { return CurrentLevel == TopLevel ? 0 : ScoreFrom0ToNextLevel - TotalScore; } }

        /// <summary>当前等级下经验到下一级的百分比</summary>
        public float ScorePercentFromThisToNextLevel { get { return ScoreOverCurrentLevel / (float)ScoreFromThisToNextLevel; } }

        /// <summary>到达最高等级总共需要的分数</summary>
        public int TopLevelTotalScore { get { return _levelTotalScores.Count > 0 ? _levelTotalScores[_levelTotalScores.Count - 1] : 0; } }

        /// <summary>最高等级</summary>
        public int TopLevel
        {
            get { return _levelScores.Count + BaseLevel; }
            set
            {
                var destTopLevel = IsZeroBased ? value : value - 1;
                var minLevel = BaseLevel;
                var validTopLevel = XMath.Max(minLevel, destTopLevel);
                _levelScores.Resize(validTopLevel, 1);
                RefreshLevelTotalScores();
            }
        }

        /// <summary>每个等级之间相差的分数，列表长度即角色最多可以升级的次数</summary>
        List<int> _levelScores = new List<int>();

        /// <summary>到达各等级分别总共需要的分数</summary>
        List<int> _levelTotalScores = new List<int>();

        Action<int> _onLevelChanged;

        public LevelQueue(bool zeroBased = false, int topLevel = 1, bool clampMaxScore = true)
        {
            IsZeroBased = zeroBased;
            ClampMaxTotalScore = clampMaxScore;
            TopLevel = topLevel;
        }

        public void AddNewLevel(int score)
        {
            _levelScores.Add(score);
            RefreshLevelTotalScores();
        }

        public void RemoveLevel(int level)
        {
            var levelToRemove = XMath.Clamp(level, BaseLevel, TopLevel);
            var lvIndex = GetLevelIndex(levelToRemove);
            _levelScores.RemoveAt(lvIndex);
            RefreshLevelTotalScores();
        }

        public void ClearLevels()
        {
            _levelScores.Clear();
            RefreshLevelTotalScores();
        }

        public bool SetLevelScore(int level, int score)
        {
            var lvIndex = GetLevelIndex(level);
            if (IsLevelIndexValid(lvIndex) && score > 0)
            {
                if (score != _levelScores[lvIndex])
                {
                    _levelScores[lvIndex] = score;
                    RefreshLevelTotalScores();
                }
                return true;
            }
            return false;
        }

        public int GetScoreOfLevel(int level)
        {
            var lvIndex = GetLevelIndex(level);
            if (IsLevelIndexValid(lvIndex))
            {
                return _levelScores[lvIndex];
            }
            else return 0;
        }

        public int GetTotalScoreFrom0ToLevel(int level)
        {
            var lvIndex = GetLevelIndex(level);
            if (IsLevelIndexValid(lvIndex))
            {
                return _levelTotalScores[lvIndex];
            }
            else return 0;
        }

        public void RefreshLevelTotalScores()
        {
            _levelTotalScores.Resize(_levelScores.Count + 1);
            var currTotalScoreRequst = 0;
            _levelTotalScores[0] = 0;
            for (int i = 0; i < _levelScores.Count; i++)
            {
                var currLevelScore = _levelScores[i];
                currTotalScoreRequst += currLevelScore;
                _levelTotalScores[i + 1] = currTotalScoreRequst;
            }
            //SetLevelByTotalScore(TotalScore);
        }

        public void AddLevelChangeListoner(Action<int> act)
        {
            _onLevelChanged -= act;
            _onLevelChanged += act;
        }

        public void RemoveLevelChangeListoner(Action<int> act)
        {
            _onLevelChanged -= act;
        }

        public string LevelInfo()
        {
            return $"等级{CurrentLevel}/{TopLevel}, 经验{ScoreOverCurrentLevel} + [{ScoreRemainToLevelUp}]/{ScoreFromThisToNextLevel},经验总量{TotalScore}/{ScoreFrom0ToNextLevel}";
        }

        int GetLevelIndex(int level)
        {
            return IsZeroBased ? level - 1 : level - 2;
        }

        bool IsLevelIndexValid(int lvIndex)
        {
            return lvIndex >= 0 && lvIndex < TopLevel;
        }

        /// <summary>
        /// score大于列表中最右（最大）值的情况已被提前处理
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        int GetLevelAchievedBySocre(int score)
        {
            // 列表长度不超过8项时二分查找效率不高，直接遍历
            if (_levelTotalScores.Count < 9)
            {
                for (int i = 0; i < _levelTotalScores.Count; i++)
                {
                    var currLevelTotalScore = _levelTotalScores[i];
                    if (score < currLevelTotalScore)
                    {
                        return XMath.Max(i - 1, 0);
                    }
                }
                return _levelTotalScores.Count - 1;
            }
            else
            {
                return BinarySearchLevelTotalScoreIndex(score, _levelTotalScores, 0, _levelTotalScores.Count - 1);
            }
        }

        /// <summary>
        /// v大于列表中最右（最大）值的情况已被提前处理
        /// </summary>
        /// <param name="v"></param>
        /// <param name="l"></param>
        /// <param name="leftIndex"></param>
        /// <param name="rightIndex"></param>
        /// <returns></returns>
        int BinarySearchLevelTotalScoreIndex(int v, List<int> l, int leftIndex, int rightIndex)
        {
            if (leftIndex == rightIndex || leftIndex + 1 == rightIndex)
            {
                return leftIndex;       // 向左偏序返回
            }

            var midIndex = (leftIndex + rightIndex) / 2;
            var midV = l[midIndex];

            if (v == midV)
            {
                return midIndex;
            }
            else if (v < midV)
            {
                rightIndex = midIndex;
            }
            else
            {
                leftIndex = midIndex;
            }

            return BinarySearchLevelTotalScoreIndex(v, l, leftIndex, rightIndex);
        }

        // 为防止循环调用，在此函数中直接操作 _currentLevelIndex
        void SetLevelByTotalScore(int score)
        {
            score = XMath.Max(0, score);

            var newL = score >= TopLevelTotalScore ? TopLevelTotalScore : GetLevelAchievedBySocre(score);

            if (newL != _levelAchieved)
            {
                _levelAchieved = newL;
                _onLevelChanged?.Invoke(CurrentLevel);
            }
        }

        // 为防止循环调用，在此函数中直接操作 _totalScore
        void SetTotalScoreByLevel(int level)
        {
            level = XMath.Max(BaseLevel, level);

            if (level >= TopLevel)
            {
                _totalScore = TopLevelTotalScore;
            }
            else
            {
                _totalScore = _levelTotalScores[LevelAchieved];
            }
        }

    }

}
