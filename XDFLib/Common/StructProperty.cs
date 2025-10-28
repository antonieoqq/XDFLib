using System;

namespace XDFLib
{
    public class StructProperty<T> where T : struct
    {
        T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (!_value.Equals(value))
                {
                    var prev = _value;
                    _value = value;
                    OnValueChanged?.Invoke(prev, _value);
                }
            }
        }
        public event Action<T, T> OnValueChanged;

        public StructProperty()
        {
            _value = default;
        }

        public StructProperty(T defaultValue)
        {
            _value = defaultValue;
        }

        public StructProperty(T defaultValue, Action<T, T> valueChangedCallBack)
        {
            _value = defaultValue;

            if (valueChangedCallBack != null)
            {
                OnValueChanged += valueChangedCallBack;
            }
        }

    }
}
