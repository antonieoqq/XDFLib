using System;

namespace XDFLib
{
    public class NewableProperty<T> where T : new()
    {
        T _value = new T();
        public T Value
        {
            get { return _value; }
            set
            {
                if (_value != null && !_value.Equals(value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(_value);
                }
            }
        }

        public event Action<T> OnValueChanged;

        public NewableProperty() { }

        public NewableProperty(Action<T> init)
        {
            init?.Invoke(Value);
        }

    }
}
