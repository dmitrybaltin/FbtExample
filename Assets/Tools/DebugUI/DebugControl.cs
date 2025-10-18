using System;

namespace Examples.UnitaskFbtExample.example2a
{
    /// <summary>
    /// Represents a runtime-adjustable debug parameter.
    /// Can be used for sliders, toggles, or other interactive UI elements.
    /// </summary>
    public class DebugControl<T>
    {
        public string Category { get; }

        public string Name { get; }

        private T _value;

        private readonly Action<T> _setter;

        public T Min { get; }

        public T Max { get; }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _setter?.Invoke(_value);
            }
        }

        public DebugControl(string category, string name, T initialValue, Action<T> setter)
        {
            Category = category;
            Name = name;
            _value = initialValue;
            _setter = setter;
        }

        public DebugControl(string category, string name, T initialValue, Action<T> setter, T min, T max)
        {
            Category = category;
            Name = name;
            _value = initialValue;
            _setter = setter;
            Min = min;
            Max = max;
        }
    }
}