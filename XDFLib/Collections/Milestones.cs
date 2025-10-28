using System;

namespace XDFLib.Collections
{
    public class Milestones
    {
        AList<double> _sections = new AList<double>();
        AList<double> _mileages = new AList<double>();

        public double TotalMileage => _mileages.Count == 0 ? 0 : _mileages[_mileages.Count - 1];

        public int SectionCount => _sections.Count;

        public ReadOnlySpan<double> Sections => _sections.AsReadOnlySpan();
        public ReadOnlySpan<double> Mileages => _mileages.AsReadOnlySpan();

        public bool AddSection(double section)
        {
            if (section <= 0) return false;
            var newIndex = _sections.Count;
            _sections.Add(section);
            _mileages.Add(TotalMileage + section);
            return true;
        }

        public bool RemoveSection(int index)
        {
            if (index < 0 || index >= _sections.Count) return false;
            var section = _sections[index];
            _sections.RemoveAt(index);
            _mileages.RemoveAt(index);
            RecalculateMileages(index);
            return true;
        }

        public bool ChangeSection(int index, double newSection)
        {
            if (newSection < 0 || index < 0 || index >= _sections.Count) return false;
            var section = _sections[index];
            _sections[index] = newSection;
            RecalculateMileages(index);
            return true;
        }

        public int GetSectionIndexOfMileage(double mileage)
        {
            if (SectionCount == 0 || mileage < 0) return -1;
            else if (mileage >= TotalMileage) return _sections.Count - 1;

            var left = 0;
            var right = _sections.Count - 1;
            var index = -1;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                if (_mileages[mid] > mileage)
                {
                    index = mid;
                    right = mid - 1; // 继续在左侧查找更小的符合条件的元素
                }
                else
                {
                    left = mid + 1;
                }
            }

            return index;
        }

        public bool TryGetMileageInDestSection(double mileage, out double mileageInSection)
        {
            mileageInSection = 0;
            var index = GetSectionIndexOfMileage(mileage);
            if (index == -1) return false;

            var prevMil = index == 0 ? 0 : _mileages[index - 1];
            mileageInSection = mileage - prevMil;
            return true;
        }

        public bool TryGetPercentInDestSection(double mileage, out double percent)
        {
            percent = 0;
            var index = GetSectionIndexOfMileage(mileage);
            if (index == -1) return false;

            var prevMil = index == 0 ? 0 : _mileages[index - 1];
            var mileageInSection = mileage - prevMil;
            var section = _sections[index];
            percent = mileageInSection / section;
            return true;
        }

        void RecalculateMileages(int fromIndex = 0)
        {
            if (_sections.Count == 0 || fromIndex < 0 || fromIndex >= _sections.Count) return;

            for (int i = fromIndex; i < _sections.Count; i++)
            {
                if (i == 0)
                {
                    _mileages[i] = _sections[i];
                }
                else
                {
                    var sec = _sections[i];
                    var prevMil = _mileages[i - 1];
                    _mileages[i] = prevMil + sec;
                }
            }
        }
    }
}
