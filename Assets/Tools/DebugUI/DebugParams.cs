using System;
using System.Collections.Generic;

namespace Examples.UnitaskFbtExample.example2a
{
    /// <summary>
    /// Central storage for debug parameters: display-only and adjustable.
    /// Implemented as a singleton.
    /// </summary>
    public class DebugParams
    {
        // Singleton instance
        public static DebugParams Instance { get; } = new DebugParams();

        // Display-only parameters
        private readonly Dictionary<string, DebugDisplay> _displays = new();

        // Adjustable parameters (float sliders for now)
        private readonly Dictionary<string, DebugControl<float>> _sliders = new();

        // Private constructor to enforce singleton
        private DebugParams() { }

        /// <summary>
        /// Display-only value (read-only)
        /// </summary>
        public void Display(string category, string name, object value)
        {
            var key = $"{category}/{name}";
            if (_displays.TryGetValue(key, out var p))
                p.Value = value;
            else
                _displays[key] = new DebugDisplay(category, name, value);
        }

        /// <summary>
        /// Register or update a float slider parameter
        /// </summary>
        public void Slider(string category, string name, float value, Action<float> setter, float min = 0f, float max = 1f)
        {
            var key = $"{category}/{name}";
            if (_sliders.TryGetValue(key, out var ctrl))
            {
                ctrl.Value = value;
            }
            else
            {
                var ctrlNew = new DebugControl<float>(category, name, value, setter, min, max);
                _sliders[key] = ctrlNew;
            }
        }

        /// <summary>
        /// Get all display-only parameters
        /// </summary>
        public IEnumerable<DebugDisplay> GetAllDisplays() => _displays.Values;

        /// <summary>
        /// Get all float slider controls
        /// </summary>
        public IEnumerable<DebugControl<float>> GetAllControls() => _sliders.Values;
    }
}
