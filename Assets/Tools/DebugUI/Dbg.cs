using System;
using Examples.UnitaskFbtExample.example2a;

namespace Tools.DebugUI
{
    public static class Dbg
    {
        public static void Display(string category, string name, object value) 
            => DebugParams.Instance.Display(category, name, value);

        public static void Slider(string category, string name, float value, Action<float> setter, float min, float max) 
            => DebugParams.Instance.Slider(category, name, value, setter, min, max);
    }

}